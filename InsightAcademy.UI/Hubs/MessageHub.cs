using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.ApplicationLayer.Services;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Enums;
using InsightAcademy.UI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NuGet.DependencyResolver;
using NuGet.Protocol.Plugins;
using System;
using System.Security.Policy;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace InsightAcademy.UI.Hubs
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;
        private readonly IHubContext<PresenceHub> _presenceHub;

        public MessageHub(IChatService chatService, IUserService userService, IHubContext<PresenceHub> presenceHub)
        {
            _chatService = chatService;
            _userService = userService;
            _presenceHub = presenceHub;
        }

        public override async Task OnConnectedAsync()
        {
            var httpcontext = Context.GetHttpContext();

            var otherUser = httpcontext.Request.Query["user"];

            var groupName = GetGroupName(Context.User.GetUserId(), otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await AddToGroup(groupName);

            var messages = await _chatService.GetMessageThread(Context.User.GetUserId(), otherUser);
            var otherUserDetail = await _userService.GetUserDetailById(otherUser);
            await Clients.Caller.SendAsync("ReceiveMessageThread", Context.User.GetUserId(), messages, groupName, otherUserDetail.FullName, otherUserDetail.ProfileImageUrl);
        }

        public async Task SendMessage(Chat chat)
        {
            Regex UrlRegex = new Regex(@"\b((https?://)?[^\s]+\.[^\s]+)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (Context.User.GetUserId() == chat.RecipientId)
            {
                throw new HubException("You can not send message to yourself");
            }

            chat.SenderId = Context.User.GetUserId();

            var recipient = await _userService.GetUserDetailById(chat.RecipientId);
            var sender = await _userService.GetUserDetailById(chat.SenderId);

            chat.RecipientId = recipient.Id;
            chat.SenderId = sender.Id;
            chat.RecipientUserName = recipient.FullName;
            chat.SenderUserName = sender.FullName;
            chat.SenderUserProfileUrl = sender.ProfileImageUrl;
            chat.RecipientUserProfileUrl = recipient.ProfileImageUrl;
            chat.CreatedDateTime = DateTime.Now;
            chat.Type = MessageTypeEnum.Text;
            var isLink = UrlRegex.IsMatch(chat.Message);
            if (isLink)
            {
                chat.Type = MessageTypeEnum.Link;
                if (!chat.Message.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !chat.Message.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    chat.Message = "http://" + chat.Message;
                }
                chat.Message = $"<a href=\"{chat.Message}\" target=\"_blank\">{chat.Message}</a>";
            }
            var groupName = GetGroupName(chat.SenderId, chat.RecipientId);

            var group = await _chatService.GetMessageGroup(groupName);

            if (group.Connections.Any(z => z.UserName == chat.RecipientId))
            {
                chat.DateRead = DateTime.UtcNow;
                chat.IsRead = true;
            }
            else
            {
                var connections = await PresenceTracker.GetConnectionsForUser(chat.RecipientId);
                if (connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                        new { username = chat.SenderUserName, knownAs = chat.SenderUserName });
                }
            }

            //check if message contain any link

            var chatObj = await _chatService.AddMessage(chat);

            if (chatObj != null)
            {
                await Clients.Caller.SendAsync("SendMessage", Context.User.GetUserId(), chat);
                await Clients.OthersInGroup(groupName).SendAsync("ReceiveMessage", Context.User.GetUserId(), chat);
            }
        }

        public async Task SendFile(Chat chat)
        {
            string[] imageExtensions = { "jpg", "jpeg", "png", "gif", "bmp", "webp", "svg" };
            string[] videoExtensions = { "mp4", "mov", "wmv", "flv", "avi", "mkv", "webm" };

            chat.SenderId = Context.User.GetUserId();

            var recipient = await _userService.GetUserDetailById(chat.RecipientId);
            var sender = await _userService.GetUserDetailById(chat.SenderId);

            chat.RecipientId = recipient.Id;
            chat.SenderId = sender.Id;
            chat.RecipientUserName = recipient.FullName;
            chat.SenderUserName = sender.FullName;
            chat.CreatedDateTime = DateTime.Now;
            chat.Message = "sent a file";
            chat.File = chat.Base64File;

            if (imageExtensions.Contains(chat.Extension.ToLower()))
            {
                chat.Type = MessageTypeEnum.Image;
            }
            else if (videoExtensions.Contains(chat.Extension.ToLower()))
            {
                chat.Type = MessageTypeEnum.Video;
            }
            else
            {
                chat.Type = MessageTypeEnum.File;
            }

            var groupName = GetGroupName(chat.SenderId, chat.RecipientId);

            var group = await _chatService.GetMessageGroup(groupName);

            if (group.Connections.Any(z => z.UserName == chat.RecipientId))
            {
                chat.DateRead = DateTime.UtcNow;
                chat.IsRead = true;
            }
            else
            {
                var connections = await PresenceTracker.GetConnectionsForUser(chat.RecipientId);
                if (connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                        new { username = chat.SenderUserName, knownAs = chat.SenderUserName });
                }
            }
            var chatObj = await _chatService.AddMessage(chat);

            if (chatObj != null)
            {
                await Clients.Caller.SendAsync("SendMessage", Context.User.GetUserId(), chat);
                await Clients.Group(groupName).SendAsync("ReceiveMessage", Context.User.GetUserId(), chat);

                //await Clients.Group(groupName).SendAsync("NewMessage", Context.User.GetUserId(), chat);
            }
        }

        public async Task Typing(string otherUserId)
        {
            var groupName = GetGroupName(Context.User.GetUserId(), otherUserId);
            await Clients.OthersInGroup(groupName).SendAsync("UserTyping");
        }

        public async Task StopTyping(string otherUserId)
        {
            var groupName = GetGroupName(Context.User.GetUserId(), otherUserId);
            await Clients.OthersInGroup(groupName).SendAsync("UserStoppedTyping");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveFromMessageGroup();
            await base.OnDisconnectedAsync(exception);
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        private async Task<DomainLayer.Entities.Group> AddToGroup(string groupName)
        {
            var group = await _chatService.GetMessageGroup(groupName);
            var connection = new DomainLayer.Entities.Connection(Context.ConnectionId, Context.User.GetUserId());

            if (group == null)
            {
                group = new DomainLayer.Entities.Group(groupName);
                await _chatService.AddGroup(group);
            }
            else
            {
                // add new connection in existing group
                connection.GroupName = group.Name;
                await _chatService.AddConnection(connection);
            }

            return group;
        }

        private async Task<DomainLayer.Entities.Group> RemoveFromMessageGroup()
        {
            var group = await _chatService.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            await _chatService.RemoveConnection(connection);
            return group;
        }
    }
}