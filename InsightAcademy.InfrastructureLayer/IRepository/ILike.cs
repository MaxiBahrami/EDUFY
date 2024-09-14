using InsightAcademy.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Repository
{
    public interface ILike
    {
        Task<bool> AddLike(Guid tutorId);

        Task<bool> DeleteLike(Guid tutorId);

        Task<IEnumerable<Like>> GetAllLike();

        Task<string> AddTeacherToLike(string username);

        Task<bool> IfTeacherExist(string userName);
    }
}