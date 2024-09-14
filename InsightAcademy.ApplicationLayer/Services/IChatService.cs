using InsightAcademy.DomainLayer.Dtos;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using File = InsightAcademy.DomainLayer.ViewModel.File;

namespace InsightAcademy.ApplicationLayer.Services
{
    public interface IChatService
    {
        Task<Chat> AddMessage(Chat chat);

        Task<IEnumerable<Chat>> GetMessageThread(string currentUser, string recipientUsername);

        void DeleteMessage(Chat message);

        Task<DomainLayer.Entities.Group> GetMessageGroup(string groupName);

        Task<DomainLayer.Entities.Group> GetGroupForConnection(string connectionId);

        Task<DomainLayer.Entities.Group> AddGroup(DomainLayer.Entities.Group group);

        Task<List<RecepientsDto>> GetRecepients(string userId);

        Task<bool> RemoveConnection(Connection connection);

        Task<Connection> AddConnection(DomainLayer.Entities.Connection connection);

        Task<List<File>> GetMediaFile(string senderId);

        Task<List<Image>> GetMediaImages(string recepientId);

        Task<List<Video>> GetMediaVideos(string groupName);

        Task<List<Link>> GetMediaLinks(string groupName);

        //void RemoveConnection(Connection connection);
        //Task<Connection> GetConnection(string connectionId);
        //Task<Chat> GetMessagesForUser(MessageParams messageParams);
    }
}