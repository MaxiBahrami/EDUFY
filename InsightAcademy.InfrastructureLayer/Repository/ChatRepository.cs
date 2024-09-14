using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Dtos;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.ViewModel;
using InsightAcademy.InfrastructureLayer.Data;
using InsightAcademy.InfrastructureLayer.Data.Seed;
using InsightAcademy.UI.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.InfrastructureLayer.Implementation
{
    public class ChatRepository : IChat
    {
        public readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public ChatRepository(AppDbContext context, IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
            _context = context;
        }

        public async Task<DomainLayer.Entities.Connection> AddConnection(DomainLayer.Entities.Connection connection)
        {
            _context.Connections.Add(connection);
            await _context.SaveChangesAsync();
            return connection;
        }

        public async Task<Group> AddGroup(Group group)
        {
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
            return group;
        }

        public async Task<bool> AddMessage(Chat chat)
        {
            await _context.Chat.AddAsync(chat);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<DomainLayer.Entities.Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups
                .Include(c => c.Connections)
                .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }

        public async Task<List<DomainLayer.ViewModel.File>> GetMediaFile(string groupName)
        {
            var files = await _context.Chat.Where(z => z.GroupName == groupName && z.Type == DomainLayer.Enums.MessageTypeEnum.File && z.File != null).Select(z => new DomainLayer.ViewModel.File()
            {
                Name = z.FileName,
                FileBase64 = z.File,
                Extension = z.Extension,
                Size = z.Size,
            }).ToListAsync();
            return files;
        }

        public async Task<List<Image>> GetMediaImages(string groupName)
        {
            var images = await _context.Chat.Where(z => z.GroupName == groupName && z.Type == DomainLayer.Enums.MessageTypeEnum.Image && z.File != null).Select(z => new DomainLayer.ViewModel.Image()
            {
                File = z.File,
            }).ToListAsync();
            return images;
        }

        public async Task<List<Link>> GetMediaLinks(string groupName)
        {
            var images = await _context.Chat.Where(z => z.GroupName == groupName && z.Type == DomainLayer.Enums.MessageTypeEnum.Link).Select(z => new DomainLayer.ViewModel.Link()
            {
                Url = z.Message,
            }).ToListAsync();
            return images;
        }

        public async Task<List<Video>> GetMediaVideos(string groupName)
        {
            var videos = await _context.Chat.Where(z => z.GroupName == groupName && z.Type == DomainLayer.Enums.MessageTypeEnum.Video && z.File != null).Select(z => new DomainLayer.ViewModel.Video()
            {
                File = z.File,
            }).ToListAsync();
            return videos;
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<IEnumerable<Chat>> GetMessageThread(string currentUserId, string recipientUserId)
        {
            var messages = await _context.Chat
               .Where(m => m.Recipient.Id == currentUserId && m.RecipientDeleted == false
                       && m.Sender.Id == recipientUserId
                       || m.Recipient.Id == recipientUserId
                       && m.Sender.Id == currentUserId && m.SenderDeleted == false
               )
               .MarkUnreadAsRead(currentUserId)
               .OrderByDescending(m => m.CreatedDateTime)

               .ToListAsync();

            return messages;
        }

        public async Task<List<RecepientsDto>> GetRecepients(string userId, string role)
        {
            IQueryable<RecepientsDto> recipientsQuery;
            if (role == Roles.Student)
            {
                recipientsQuery = _context.Chat
                .Where(m => m.RecipientId == userId)
               .GroupBy(m => m.Sender)
               .Select(g => new RecepientsDto
               {
                   Id = new Guid(g.Key.Id),
                   RecepientId = g.FirstOrDefault().RecipientId,

                   Name = g.Key.FullName,
                   ImageUrl = g.FirstOrDefault().Sender.ProfileImageUrl,
                   LastMessage = g.OrderByDescending(m => m.CreatedDateTime).FirstOrDefault().Message,
                   TimeSpan = g.OrderByDescending(m => m.CreatedDateTime).FirstOrDefault().CreatedDateTime.Value
               });
            }
            else
            {
                recipientsQuery = _context.Chat
                .Where(m => m.RecipientId == userId)
               .GroupBy(m => m.Sender)
               .Select(g => new RecepientsDto
               {
                   Id = new Guid(g.Key.Id),
                   RecepientId = g.FirstOrDefault().SenderId,
                   Name = g.Key.FullName,
                   ImageUrl = g.FirstOrDefault().Sender.ProfileImageUrl,
                   LastMessage = g.OrderByDescending(m => m.CreatedDateTime).FirstOrDefault().Message,
                   TimeSpan = g.OrderByDescending(m => m.CreatedDateTime).FirstOrDefault().CreatedDateTime.Value
               });
            }

            return await recipientsQuery.ToListAsync();
        }

        public async Task<ChatMedia> GetSenderMedia(string recepientId)
        {
            ChatMedia chatMedia = new ChatMedia();
            var messages = await _context.Chat.Where(z => z.RecipientId == recepientId).ToListAsync();

            if (messages != null)
            {
                chatMedia.Files = messages.Where(z => z.Type == DomainLayer.Enums.MessageTypeEnum.File && z.File != null).Select(z => new DomainLayer.ViewModel.File()
                {
                    Name = z.FileName,
                    FileBase64 = z.File,
                    Extension = z.Extension,
                    Size = z.Size,
                }).ToList();

                chatMedia.Videos = messages.Where(z => z.Type == DomainLayer.Enums.MessageTypeEnum.Video && z.File != null).Select(z => new DomainLayer.ViewModel.Video()
                {
                    File = z.File,
                }).ToList();

                chatMedia.Links = messages.Where(z => z.Type == DomainLayer.Enums.MessageTypeEnum.Link && z.File != null).Select(z => new DomainLayer.ViewModel.Link()
                {
                    Url = z.File,
                }).ToList();

                chatMedia.Images = messages.Where(z => z.Type == DomainLayer.Enums.MessageTypeEnum.Image && z.File != null).Select(z => new DomainLayer.ViewModel.Image()
                {
                    File = z.File,
                }).ToList();
            }
            return chatMedia;
        }

        public async Task<bool> RemoveConnection(DomainLayer.Entities.Connection connection)
        {
            _context.Connections.Remove(connection);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}