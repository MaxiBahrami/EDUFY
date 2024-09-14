using InsightAcademy.DomainLayer.Dtos;
using InsightAcademy.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Repository
{
    public interface ISubject
    {
        Task<IEnumerable<Subject>> GetAllSubject();

        Task<IEnumerable<NewSubjectRequestDto>> GetAllSubjectRequsts();

        //Task<List<string>> GetAllTags();

        Task<bool> AddSubject(Subject subject);

        Task<bool> EditSubject(Subject subject);

        Task<Subject> GetSubjectById(Guid Id);

        Task<bool> DeleteSubjectById(Guid Id);
    }
}