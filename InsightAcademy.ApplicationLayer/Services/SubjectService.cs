using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Dtos;
using InsightAcademy.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Services
{
    public class SubjectService : ISubjectService
    {
        private ISubject _subject;

        public SubjectService(ISubject subject)
        {
            _subject = subject;
        }

        public async Task<bool> AddSubject(Subject subject)
        {
            return await _subject.AddSubject(subject);
        }

        public async Task<bool> EditSubject(Subject subject)
        {
            return await _subject.EditSubject(subject);
        }

        public async Task<IEnumerable<Subject>> GetAllSubject()
        {
            return await _subject.GetAllSubject();
        }

        public async Task<Subject> GetSubjectById(Guid Id)
        {
            return await _subject.GetSubjectById(Id);
        }

        public async Task<bool> DeleteSubjectById(Guid Id)
        {
            return await _subject.DeleteSubjectById(Id);
        }

        public async Task<IEnumerable<NewSubjectRequestDto>> GetAllNewSubjectRequest()
        {
            return await _subject.GetAllSubjectRequsts();
        }
    }
}