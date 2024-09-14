using InsightAcademy.DomainLayer.Entities;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Repository
{
    public interface IMessage
    {
        Task<bool> AddMessageAsync(Chat chat);

        Task<bool> DeleteMessageAsync(Chat chat);

        Task<Chat> GetMessage(Guid id);
    }
}