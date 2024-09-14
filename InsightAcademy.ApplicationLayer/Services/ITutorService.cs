using InsightAcademy.ApplicationLayer.Models;
using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.ApplicationLayer.Services
{
    public interface ITutorService
    {
        Task<Guid> AddPersonalDetialAsync(Tutor profile);

        Task<bool> AddContactDetialAsync(Contact contact);

        Task<bool> AddEducationDetialAsync(Education education);

        Task<bool> AddSubjectDetialAsync();

        Task<bool> AddGalleryDetialAsync();

        Task<Tutor> GetPersonalDetail(string userId);

        Task<Contact> GetContactDetails(Guid tutorId);

        Task<bool> EditPersonalDetails(Tutor tutor);

        Task<bool> EditContactDetails(Contact contact);

        Task<bool> EditEducationDetails(Education education);

        Task<IEnumerable<Education>> GetEducationDetails(Guid tutorId);

        Task<Education> GetEducationById(Guid id);

        Task<bool> DeleteEducation(Guid id);

        Task<Tutor> tutorProfile(Guid id);

        Task<bool> AddSubjectAsync(Subject subject);

        //Task<List<Tutor>> TutorSubject(string subject);
        Task<List<Subject>> GetAllSubject();

        Task<IEnumerable<TutorSubject>> GetTutorsBySubject(Guid subject);

        Task<List<Education>> GetAllDegree();

        Task<List<Tutor>> TutorList();

        Task<bool> ApproveTutor(Guid tutorId);

        Task<bool> UnapproveTutor(Guid tutorId);

        Task<Pager<Tutor>> Filter(TutorFilter filter, int pageNumber);

        Task<IEnumerable<TutorSubject>> GetMySubjects(Guid tutorId);

        Task<TutorSubject> GetSubjectById(Guid subjectId);

        Task<bool> EditTutorSubjectDetails(TutorSubject subject);

        Task<bool> DeleteSubject(Guid id);

        Task<bool> UploadMedia(Guid tutorId, string url);

        Task<IEnumerable<Media>> GetTutorMedia(Guid tutorId);

        Task<bool> SaveMediaUrl(Guid tutorId, string url);

        Task<bool> DeleteMediaUrl(Guid id);

        Task<IEnumerable<Tutor>> GetRelatedTutorsBySubject(Tutor tutor);

        Task<Tutor> tutorProfileByUserName(string email);

        Task<List<Tutor>> GetTutorsByStatus(int status);

        Task<bool> AddTutorSubjectAsync(TutorSubject subject);

        Task<RatingBreakdownViewModel> GetTutorRatingBreakDown(Guid tutorId);

        Task<bool> AddNewSubjectRequest(NewSubjectRequest newSubjectRequest);

        Task<bool> EditTutorByAdmin(Tutor tutor);

        Task<Tutor> GetTutor(Guid tutorId);
    }
}