using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Dtos;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Services
{
    public class ChatService : IChatService
    {
        private IChat _chat;
        private IUserService _userService;
        private IEmailService _emailService;
        private IHttpContextAccessor _httpContextAccessor;
        private IWebHostEnvironment _webHostEnvironment;

        public ChatService(IChat chat, IUserService userService, IEmailService emailService, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _chat = chat;
            _userService = userService;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<Connection> AddConnection(Connection connection)
        {
            return await _chat.AddConnection(connection);
        }

        public async Task<DomainLayer.Entities.Group> AddGroup(DomainLayer.Entities.Group group)
        {
            return await _chat.AddGroup(group);
        }

        public async Task<Chat> AddMessage(Chat chat)
        {
            var status = await _chat.AddMessage(chat);
            //if (status)
            //{
            //    await _emailService.NotifyMessageSend(recipient.Email, recipient.FullName, sender.ProfileImageUrl, chat.Message);
            //}
            return chat;
        }

        public void DeleteMessage(Chat message)
        {
            throw new NotImplementedException();
        }

        public async Task<DomainLayer.Entities.Group> GetGroupForConnection(string connectionId)
        {
            return await _chat.GetGroupForConnection(connectionId);
        }

        public async Task<DomainLayer.Entities.Group> GetMessageGroup(string groupName)
        {
            return await _chat.GetMessageGroup(groupName);
        }

        public async Task<IEnumerable<Chat>> GetMessageThread(string currentUser, string recipientUsername)
        {
            return await _chat.GetMessageThread(currentUser, recipientUsername);
        }

        public async Task<List<RecepientsDto>> GetRecepients(string userId)
        {
            var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            return await _chat.GetRecepients(userId, role);
        }

        public async Task<bool> RemoveConnection(Connection connection)
        {
            return await _chat.RemoveConnection(connection);
        }

        public async Task<List<DomainLayer.ViewModel.File>> GetMediaFile(string groupName)
        {
            return await _chat.GetMediaFile(groupName);
        }

        public async Task<List<Image>> GetMediaImages(string groupName)
        {
            return await _chat.GetMediaImages(groupName);
        }

        public async Task<List<Video>> GetMediaVideos(string groupName)
        {
            return await _chat.GetMediaVideos(groupName);
        }

        public async Task<List<Link>> GetMediaLinks(string groupName)
        {
            return await _chat.GetMediaLinks(groupName);
        }
    }
}