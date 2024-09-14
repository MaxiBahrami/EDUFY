using InsightAcademy.ApplicationLayer.Models;
using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.ViewModel;
using InsightAcademy.InfrastructureLayer.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Subject = InsightAcademy.DomainLayer.Entities.Subject;

namespace InsightAcademy.ApplicationLayer.Services
{
    public class TutorServices : ITutorService
    {
        private readonly ITutor _tutor;

        public TutorServices(ITutor profile)
        {
            _tutor = profile;
        }

        public async Task<bool> AddContactDetialAsync(Contact contact)
        {
            await _tutor.AddContactDetialAsync(contact);
            return true;
        }

        public async Task<bool> AddEducationDetialAsync(Education education)
        {
            education.StartDate = Convert.ToDateTime(education.StartDateString);

            if (!string.IsNullOrEmpty(education.EndDateString))
            {
                education.EndDate = Convert.ToDateTime(education.EndDateString);
            }
            await _tutor.AddEducationDetialAsync(education);
            return true;
        }

        public async Task<bool> AddGalleryDetialAsync()
        {
            await _tutor.AddGalleryDetialAsync();
            return true;
        }

        public async Task<Guid> AddPersonalDetialAsync(Tutor profile)
        {
            if (profile.SelectedServices.Count > 0)
            {
                profile.Services = string.Join(",", profile.SelectedServices);
            }
            var id = await _tutor.AddPersonalDetialAsync(profile);
            return id;
        }

        public async Task<bool> AddSubjectDetialAsync()
        {
            await _tutor.AddSubjectDetialAsync();
            return true;
        }

        public async Task<Contact> GetContactDetails(Guid tutorId)
        {
            return await _tutor.GetContactDetails(tutorId);
        }

        public async Task<Tutor> GetPersonalDetail(string userId)
        {
            return await _tutor.GetProfileDetail(userId);
        }

        public async Task<bool> EditPersonalDetails(Tutor tutor)
        {
            if (tutor.SelectedServices.Count > 0)
            {
                tutor.Services = string.Join(",", tutor.SelectedServices);
            }
            return await _tutor.EditPersonalDetails(tutor);
        }

        public async Task<bool> EditContactDetails(Contact contact)
        {
            return await _tutor.EditContactDetails(contact);
        }

        public async Task<bool> EditEducationDetails(Education education)
        {
            education.StartDate = Convert.ToDateTime(education.StartDateString);

            if (!string.IsNullOrEmpty(education.EndDateString))
            {
                education.EndDate = Convert.ToDateTime(education.EndDateString);
            }
            return await _tutor.EditEducationDetails(education);
        }

        public async Task<IEnumerable<Education>> GetEducationDetails(Guid tutorId)
        {
            return await _tutor.GetEducationDetails(tutorId);
        }

        public async Task<Education> GetEducationById(Guid id)
        {
            return await _tutor.GetEducationById(id);
        }

        public async Task<bool> DeleteEducation(Guid id)
        {
            return await _tutor.DeleteEducation(id);
        }

        public async Task<Tutor> tutorProfile(Guid id)
        {
            return await _tutor.tutorProfile(id);
        }

        public async Task<bool> AddSubjectAsync(Subject subject)
        {
            return await _tutor.AddSubjectAsync(subject);
        }

        public async Task<List<Subject>> GetAllSubject()
        {
            return await _tutor.GetAllSubject();
        }

        public async Task<IEnumerable<TutorSubject>> GetTutorsBySubject(Guid subject)
        {
            return await _tutor.GetTutorsBySubject(subject);
        }

        public async Task<List<Education>> GetAllDegree()
        {
            return await _tutor.GetAllDegree();
        }

        public async Task<List<Tutor>> TutorList()
        {
            return await _tutor.TutorList();
        }

        public async Task<bool> ApproveTutor(Guid tutorId)
        {
            return await _tutor.ApproveTutor(tutorId);
        }

        public async Task<bool> UnapproveTutor(Guid tutorId)
        {
            return await _tutor.UnapproveTutor(tutorId);
        }

        public async Task<Pager<Tutor>> Filter(TutorFilter filter, int pageNumber)
        {
            return await _tutor.Filter(filter, pageNumber);
        }

        public async Task<IEnumerable<TutorSubject>> GetMySubjects(Guid tutorId)
        {
            return await _tutor.GetMySubjects(tutorId);
        }

        public async Task<TutorSubject> GetSubjectById(Guid GetSubjectById)
        {
            return await _tutor.GetSubjectById(GetSubjectById);
        }

        public async Task<bool> EditTutorSubjectDetails(TutorSubject subject)
        {
            return await _tutor.EditTutorSubjectDetails(subject);
        }

        public async Task<bool> DeleteSubject(Guid id)
        {
            return await _tutor.DeleteSubject(id);
        }

        public async Task<bool> UploadMedia(Guid tutorId, string url)
        {
            return await _tutor.UploadMedia(tutorId, url);
        }

        public async Task<IEnumerable<Media>> GetTutorMedia(Guid tutorId)
        {
            return await _tutor.GetTutorMedia(tutorId);
        }

        public async Task<bool> SaveMediaUrl(Guid tutorId, string url)
        {
            return await _tutor.SaveMediaUrl(tutorId, url);
        }

        public async Task<bool> DeleteMediaUrl(Guid id)
        {
            return await _tutor.DeleteMediaUrl(id);
        }

        public async Task<IEnumerable<Tutor>> GetRelatedTutorsBySubject(Tutor tutor)
        {
            return await _tutor.GetRelatedTutorsBySubject(tutor);
        }

        public async Task<Tutor> tutorProfileByUserName(string email)
        {
            return await _tutor.TutorProfileByUserName(email);
        }

        public async Task<List<Tutor>> GetTutorsByStatus(int status)
        {
            return await _tutor.GetTutorsByStatus(status);
        }

        public async Task<bool> AddTutorSubjectAsync(TutorSubject subject)
        {
            return await _tutor.AddTutorSubjectAsync(subject);
        }

        public async Task<RatingBreakdownViewModel> GetTutorRatingBreakDown(Guid tutorId)
        {
            return await _tutor.GetTutorRatingBreakDown(tutorId);
        }

        public async Task<bool> AddNewSubjectRequest(NewSubjectRequest newSubjectRequest)
        {
            return await _tutor.AddNewSubjectRequest(newSubjectRequest);
        }

        public async Task<bool> EditTutorByAdmin(Tutor tutor)
        {
            return await _tutor.EditTutorByAdmin(tutor);
        }

        public async Task<Tutor> GetTutor(Guid tutorId)
        {
            return await _tutor.GetTutor(tutorId);
        }
    }
}