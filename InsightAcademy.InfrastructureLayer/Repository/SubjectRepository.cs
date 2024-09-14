using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Dtos;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.InfrastructureLayer.Implementation
{
    public class SubjectRepository : ISubject
    {
        public readonly AppDbContext _context;

        public SubjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subject>> GetAllSubject()
        {
            return await _context.Subject.ToListAsync();
        }

        public async Task<bool> AddSubject(Subject subject)
        {
            await _context.Subject.AddAsync(subject);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EditSubject(Subject subject)
        {
            var subjectFromDb = await _context.Subject.FirstOrDefaultAsync(x => x.Id == subject.Id);
            if (subjectFromDb != null)
            {
                subjectFromDb.Name = subject.Name;
                _context.Subject.Update(subject);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<Subject> GetSubjectById(Guid Id)
        {
            var subjectFromDb = await _context.Subject.FirstOrDefaultAsync(x => x.Id == Id);
            if (subjectFromDb != null)
            {
                return subjectFromDb;
            }
            return null;
        }

        public async Task<bool> DeleteSubjectById(Guid Id)
        {
            var subjectFromDb = await _context.Subject.FirstOrDefaultAsync(x => x.Id == Id);
            if (subjectFromDb != null)
            {
                _context.Remove(subjectFromDb);
                return _context.SaveChanges() > 0;
            }
            return false;
        }

        public async Task<IEnumerable<NewSubjectRequestDto>> GetAllSubjectRequsts()
        {
            return await _context.NewSubjectRequest.Where(z => z.IsApproved == false).Select(z => new NewSubjectRequestDto()
            {
                Id = z.Id,
                Subject = _context.Subject.FirstOrDefault(s => s.Id == s.Id).Name,
                Category = _context.Category.FirstOrDefault(s => s.Id == s.Id).Name,
                Status = z.IsApproved,
            }).ToListAsync();
        }
    }
}