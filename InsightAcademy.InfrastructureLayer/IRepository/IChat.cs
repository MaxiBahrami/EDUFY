using InsightAcademy.DomainLayer.Dtos;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Repository
{
    public interface IChat
    {
        Task<Group> AddGroup(Group group);

        Task<bool> RemoveConnection(Connection connection);

        Task<Connection> GetConnection(string connectionId);

        Task<Group> GetMessageGroup(string groupName);

        Task<bool> AddMessage(Chat chat);

        Task<IEnumerable<Chat>> GetMessageThread(string currentUser, string recipientUsername);

        Task<List<RecepientsDto>> GetRecepients(string userId, string role);

        Task<Connection> AddConnection(Connection connection);

        Task<Group> GetGroupForConnection(string connectionId);

        Task<ChatMedia> GetSenderMedia(string recepientId);

        Task<List<DomainLayer.ViewModel.File>> GetMediaFile(string recepientId);

        Task<List<Image>> GetMediaImages(string recepientId);

        Task<List<Video>> GetMediaVideos(string groupName);
       Task<List<Link>> GetMediaLinks(string groupName);
    }
}