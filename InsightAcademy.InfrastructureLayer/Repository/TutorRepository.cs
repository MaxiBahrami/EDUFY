using InsightAcademy.ApplicationLayer.Models;
using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.DomainLayer.Entities.Identity;
using InsightAcademy.DomainLayer.ViewModel;
using InsightAcademy.InfrastructureLayer.Data;
using InsightAcademy.InfrastructureLayer.Migrations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Subject = InsightAcademy.DomainLayer.Entities.Subject;

namespace InsightAcademy.InfrastructureLayer.Implementation
{
    public class TutorRepository : ITutor
    {
        private readonly AppDbContext _contaxt;
        //private readonly IApplicationEmailSender _emailSender;

        public TutorRepository(AppDbContext context)
        {
            _contaxt = context;
        }

        public async Task<bool> AddContactDetialAsync(Contact contact)
        {
            await _contaxt.Contact.AddAsync(contact);
            await _contaxt.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddEducationDetialAsync(Education education)
        {
            _contaxt.Education.Add(education);
            return await _contaxt.SaveChangesAsync() > 0;
        }

        public Task<bool> AddGalleryDetialAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Guid> AddPersonalDetialAsync(Tutor profile)
        {
            await _contaxt.Tutor.AddAsync(profile);
            await _contaxt.SaveChangesAsync();
            return profile.Id;
        }

        public Task<bool> AddSubjectDetialAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Contact> GetContactDetails(Guid tutorId)
        {
            return await _contaxt.Contact.FirstOrDefaultAsync(z => z.TutorId == tutorId);
        }

        public async Task<Tutor> GetProfileDetail(string userId)
        {
            return await _contaxt.Tutor.Include(x => x.Likes).FirstOrDefaultAsync(z => z.ApplicationUserId == userId);
        }

        public async Task<bool> EditPersonalDetails(Tutor tutorObj)
        {
            var tutor = await _contaxt.Tutor.FindAsync(tutorObj.Id);

            tutor.FirstName = tutorObj.FirstName;
            tutor.LastName = tutorObj.LastName;
            tutor.Introduction = tutorObj.Introduction;
            tutor.Tagline = tutorObj.Tagline;

            tutor.City = tutorObj.City;
            tutor.Country = tutorObj.Country;
            tutor.HourlyRate = tutorObj.HourlyRate;
            tutor.ZipCode = tutorObj.ZipCode;

            tutor.Services = tutorObj.Services;
            tutor.Language = tutorObj.Language;
            _contaxt.Tutor.Update(tutor);
            return await _contaxt.SaveChangesAsync() > 0;
        }

        public async Task<bool> EditContactDetails(Contact contact)
        {
            var oldTuor = await _contaxt.Contact.FindAsync(contact.Id);

            oldTuor.PhoneNumber = contact.PhoneNumber;
            oldTuor.Email = contact.Email;
            oldTuor.SkypeId = contact.SkypeId;
            oldTuor.WhatsappNumber = contact.WhatsappNumber;
            oldTuor.Website = contact.Website;
            _contaxt.Contact.Update(oldTuor);
            return await _contaxt.SaveChangesAsync() > 0;
        }

        public async Task<bool> EditEducationDetails(Education education)
        {
            var oldEducation = await _contaxt.Education.FindAsync(education.Id);

            oldEducation.Degree = education.Degree;
            oldEducation.University = education.University;
            oldEducation.Location = education.Location;
            oldEducation.StartDate = education.StartDate;
            oldEducation.EndDate = education.EndDate;
            oldEducation.Description = education.Description;
            _contaxt.Education.Update(oldEducation);
            return await _contaxt.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Education>> GetEducationDetails(Guid tutorId)
        {
            return await _contaxt.Education.Where(z => z.TutorId == tutorId).OrderByDescending(z => z.StartDate).ToListAsync();
        }

        public async Task<Education> GetEducationById(Guid id)
        {
            return await _contaxt.Education.FindAsync(id);
        }

        public async Task<bool> DeleteEducation(Guid id)
        {
            var education = await _contaxt.Education.FindAsync(id);
            _contaxt.Education.Remove(education);
            return await _contaxt.SaveChangesAsync() > 0;
        }

        public async Task<Tutor> tutorProfile(Guid id)
        {
            var tutor = await _contaxt.Tutor.Include(x => x.WishLists).Include(x => x.Reviews).ThenInclude(x => x.Student).Include(x => x.Likes).Include(z => z.Media).Include(z => z.Contact).Include(z => z.TutorSubject).ThenInclude(tutorSubject => tutorSubject.Subject).Include(z => z.TutorSubject).ThenInclude(tutorSubject => tutorSubject.Category).Include(z => z.ApplicationUser).Include(z => z.Education).FirstOrDefaultAsync(x => x.Id == id);
            return tutor;
        }

        public async Task<bool> AddSubjectAsync(Subject subject)
        {
            await _contaxt.Subject.AddAsync(subject);
            return await _contaxt.SaveChangesAsync() > 0;
        }

        //public async Task<IEnumerable<Subject>> SubjectsList(Guid subjectId)
        //      {
        //         return await _contaxt.Subject.Where(z => z.Id == subjectId).ToListAsync();

        //}
        public async Task<List<Subject>> GetAllSubject()
        {
            var result = await _contaxt.Subject.ToListAsync();
            return result;
        }

        public async Task<IEnumerable<TutorSubject>> GetTutorsBySubject(Guid subject)
        {
            var result = await _contaxt.TutorSubject.Include(x => x.Tutor).ThenInclude(x => x.WishLists).Include(z => z.Tutor).ThenInclude(x => x.ApplicationUser).ToListAsync();

            return result;
        }

        public async Task<List<Education>> GetAllDegree()
        {
            var result = await _contaxt.Education.ToListAsync();
            return result;
        }

        public async Task<List<Tutor>> TutorList()
        {
            var tutorList = await _contaxt.Tutor
                                        .Include(tutor => tutor.Contact)
                                        .Include(tutor => tutor.ApplicationUser)
                                        .Include(tutor => tutor.WishLists)
                                        .Include(tutor => tutor.Media)
                                        .Include(tutor => tutor.TutorSubject)
                                        .Include(tutor => tutor.Reviews)
                                        .ToListAsync();

            return tutorList;
        }

        public async Task<bool> ApproveTutor(Guid tutorId)
        {
            var tutor = _contaxt.Tutor.FirstOrDefault(x => x.Id == tutorId);

            if (tutor != null)
            {
                tutor.IsVerified = true;
                _contaxt.Update(tutor);
                await _contaxt.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task<bool> UnapproveTutor(Guid tutorId)
        {
            var tutor = _contaxt.Tutor.FirstOrDefault(x => x.Id == tutorId);

            if (tutor != null)
            {
                tutor.IsVerified = false;
                _contaxt.Update(tutor);

                await _contaxt.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Pager<Tutor>> Filter(TutorFilter filter, int pageNumber)
        {
            var pageSize = 10;
            var skipAmount = (pageNumber - 1) * pageSize;

            IQueryable<Tutor> query = _contaxt.Tutor.Where(x => x.IsVerified == true && x.IsBlock == false)
                                                    .Include(x => x.ApplicationUser)
                                                    .Include(x => x.WishLists)
                                                    .Include(x => x.Contact)
                                                    .Include(z => z.TutorSubject)
                                                    .Include(z => z.Media)
                                                    .Include(z => z.Likes)
                                                    .Include(x => x.Reviews);
            if (filter.IsLocal)
            {
                query = query.Where(z => z.City == filter.CurrentUserCountryId);
            }

            if (filter.SelectedCategories.Count > 0)
            {
                query = query.Where(t => t.TutorSubject.Any(ts => filter.SelectedCategories.Contains(ts.Category.Id)));
            }
            if (filter.SelectedRating.Count > 0)
            {
                var selectedRatings = filter.SelectedRating;

                // Calculate average ratings and filter based on selected ratings
                var tutorWithAvgRatings = _contaxt.Review
                    .GroupBy(r => r.TutorId)
                    .Select(g => new
                    {
                        TutorId = g.Key,
                        AverageRating = g.Average(r => r.Rating)
                    })
                    .Where(t => selectedRatings.Contains((int)Math.Ceiling(t.AverageRating)));

                // Join the result with tutors and apply the filter
                query = query.Join(tutorWithAvgRatings,
                                   tutor => tutor.Id,
                                   avgRating => avgRating.TutorId,
                                   (tutor, avgRating) => tutor);
            }

            if (filter.Category != Guid.Empty)
            {
                query = query.Where(t => t.TutorSubject.Any(ts => ts.Category.Id == filter.Category));
            }
            if (!string.IsNullOrEmpty(filter.Education))
            {
                // Use Any() with the list of selected category IDs
                query = query.Where(s => s.Education.Any(d => d.Degree == filter.Education));
            }

            if (!string.IsNullOrEmpty(filter.Tags))
            {
                query = query.Where(t => t.TutorSubject.Any(z => z.Tags.Contains(filter.Tags)));
            }
            if (!string.IsNullOrEmpty(filter.Subject))
            {
                query = query.Where(t => t.TutorSubject.Any(z => z.Subject.Name.Contains(filter.Subject)));
            }
            if (filter.Order == "ASC")
            {
                query = query.OrderBy(z => z.HourlyRate);
            }
            else if (filter.Order == "DESC")
            {
                query = query.OrderByDescending(z => z.HourlyRate);
            }
            else if (filter.Order == "RHL")
            {
                query = query
                .Select(t => new
                {
                    Tutor = t,
                    AverageRating = _contaxt.Review
                        .Where(r => r.TutorId == t.Id)
                        .Average(r => (double?)r.Rating) ?? 0 // Handle cases where there are no reviews
                })
                .OrderByDescending(t => t.AverageRating)
                .Select(t => t.Tutor);
            }
            else if (filter.Order == "RLH")
            {
                query = query
                .Select(t => new
                {
                    Tutor = t,
                    AverageRating = _contaxt.Review
                        .Where(r => r.TutorId == t.Id)
                        .Average(r => (double?)r.Rating) ?? 0 // Handle cases where there are no reviews
                })
                .OrderBy(t => t.AverageRating)
                .Select(t => t.Tutor);
            }
            if (!string.IsNullOrEmpty(filter.Location))
            {
                query = query.Where(z => z.City.Contains(filter.Location));
            }
            if (filter.MinPrice > 0 && filter.MaxPrice > 0)
            {
                query = query.Where(z => z.HourlyRate >= filter.MinPrice && z.HourlyRate <= filter.MaxPrice);
            }

            if (!string.IsNullOrEmpty(filter.service))
            {
                query = query.Where(z => z.Services.Contains(filter.service));
            }
            if (!string.IsNullOrEmpty(filter.Photo))
            {
                query = query.Where(z => z.ApplicationUser.ProfileImageUrl != "");
            }
            if (!string.IsNullOrEmpty(filter.Gender))
            {
                query = query.Where(z => z.ApplicationUser.Gender.ToLower() == filter.Gender);
            }
            if (filter.Rating != null && filter.Rating != 0)
            {
                query = query.Where(x => x.Reviews.Any());
                query = query.Where(x => x.Reviews.Average(r => r.Rating) == filter.Rating);
            }

            //if (filter.SubjectIds.Count > 0)
            //{
            //    query = query.Where(t => t.TutorSubject.Any(ts => filter.SubjectIds.Contains(filter.Category)));
            //}
            query = query.Skip(skipAmount).Take(pageSize);

            var data = await query.ToListAsync();

            return new Pager<Tutor> { Items = data, PageNumber = pageNumber, PageSize = pageSize, TotalItems = data.Count, };
        }

        public async Task<IEnumerable<TutorSubject>> GetMySubjects(Guid tutorId)
        {
            var result = await _contaxt.TutorSubject.Include(z => z.Subject).Include(z => z.Category).Where(z => z.TutorId == tutorId).ToListAsync();
            return result;
        }

        public async Task<bool> EditTutorSubjectDetails(TutorSubject subject)
        {
            var oldSubject = await _contaxt.TutorSubject.FirstOrDefaultAsync(z => z.Id == subject.Id && z.TutorId == subject.TutorId);

            oldSubject.Tags = subject.Tags;

            _contaxt.TutorSubject.Update(subject);
            return await _contaxt.SaveChangesAsync() > 0;
        }

        public async Task<TutorSubject> GetSubjectById(Guid subjectId)
        {
            var result = await _contaxt.TutorSubject.FirstOrDefaultAsync(z => z.Id == subjectId);
            return result;
        }

        public async Task<bool> DeleteSubject(Guid id)
        {
            var subject = await _contaxt.TutorSubject.FirstOrDefaultAsync(z => z.Id == id);
            _contaxt.TutorSubject.Remove(subject);
            return await _contaxt.SaveChangesAsync() > 0;
        }

        public async Task<bool> UploadMedia(Guid tutorId, string url)
        {
            Media media = new Media()
            {
                Type = "1",
                Url = url,
                TutorId = tutorId,
            };
            _contaxt.Media.Add(media);
            return await _contaxt.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Media>> GetTutorMedia(Guid tutorId)
        {
            return await _contaxt.Media.Where(z => z.TutorId == tutorId).ToListAsync();
        }

        public async Task<bool> SaveMediaUrl(Guid tutorId, string url)
        {
            Media media = new Media()
            {
                Type = "2",
                Url = url,
                TutorId = tutorId,
            };
            _contaxt.Media.Add(media);
            return await _contaxt.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteMediaUrl(Guid id)
        {
            var findUrl = _contaxt.Media.Where(z => z.Id == id).FirstOrDefault();
            if (findUrl != null)
            {
                _contaxt.Media.Remove(findUrl);
                return await _contaxt.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<IEnumerable<Tutor>> GetRelatedTutorsBySubject(Tutor tutor)
        {
            // Fetch tutors excluding the input tutor
            IQueryable<Tutor> query = _contaxt.Tutor.Include(x => x.Likes).Include(a => a.ApplicationUser).Include(e => e.Education).Include(w => w.WishLists).Include(c => c.Contact).Include(z => z.Media).Where(t => t.Id != tutor.Id);

            // Get the subject category IDs of the input tutor
            var categoryIds = tutor.TutorSubject.Select(ts => ts.SubjectId).ToList();
            var tags = tutor.TutorSubject.Select(ts => ts.Tags).ToList();

            // Filter tutors based on matching subject category IDs
            query = query.Where(t =>
                t.TutorSubject.Any(ts => categoryIds.Contains(ts.SubjectId)));

            //     query = query.Where(t =>
            //t.TutorSubject.Any(ts => tags.Contains(ts.Tags)));
            // Execute the query and return the result
            var relatedTutors = await query.ToListAsync();
            return relatedTutors;
        }

        public async Task<Tutor> TutorProfileByUserName(string email)
        {
            var tutor = await _contaxt.Tutor.Include(x => x.WishLists).Include(x => x.Likes).Include(z => z.Media).Include(z => z.Contact).Include(z => z.TutorSubject).Include(z => z.ApplicationUser).Include(z => z.Education).FirstOrDefaultAsync(x => x.ApplicationUser.UserName == email);
            return tutor;
        }

        public async Task<List<Tutor>> GetTutorsByStatus(int status)
        {
            IQueryable<Tutor> query = _contaxt.Tutor.Include(x => x.Contact).Include(x => x.ApplicationUser).Include(x => x.TutorSubject).Include(x => x.Reviews).Where(x => x.IsBlock == false);
            if (status == 0)
            {
                query = query.Where(x => x.IsVerified == false);
            }
            else
            {
                query = query.Where(x => x.IsVerified == true);
            }

            return await query.ToListAsync();
        }

        public async Task<bool> AddTutorSubjectAsync(TutorSubject subject)
        {
            _contaxt.TutorSubject.Add(subject);
            return await _contaxt.SaveChangesAsync() > 0;
        }

        public async Task<RatingBreakdownViewModel> GetTutorRatingBreakDown(Guid tutorId)
        {
            var ratings = await _contaxt.Review.Where(r => r.TutorId == tutorId).ToListAsync();

            var oneStarCount = ratings.Count(r => r.Rating == 1);
            var twoStarCount = ratings.Count(r => r.Rating == 2);
            var threeStarCount = ratings.Count(r => r.Rating == 3);
            var fourStarCount = ratings.Count(r => r.Rating == 4);
            var fiveStarCount = ratings.Count(r => r.Rating == 5);
            var overallRating = ratings.Count > 0 ? ratings.Average(r => r.Rating) : 0;

            return new RatingBreakdownViewModel
            {
                OneStarCount = oneStarCount,
                TwoStarCount = twoStarCount,
                ThreeStarCount = threeStarCount,
                FourStarCount = fourStarCount,
                FiveStarCount = fiveStarCount,
                OverallRating = overallRating
            };
        }

        public async Task<bool> AddNewSubjectRequest(NewSubjectRequest subject)
        {
            _contaxt.NewSubjectRequest.Add(subject);
            await _contaxt.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EditTutorByAdmin(Tutor tutorObj)
        {
            var tutor = await _contaxt.Tutor.Include(x => x.Contact).FirstOrDefaultAsync(x => x.Id == tutorObj.Id);

            tutor.City = tutorObj.City;
            tutor.Country = tutorObj.Country;

            tutor.Contact = new Contact();
            tutor.Contact.Website = tutorObj.Contact.Website;
            tutor.Contact.PhoneNumber = tutorObj.Contact.PhoneNumber;

            _contaxt.Tutor.Update(tutor);
            return await _contaxt.SaveChangesAsync() > 0;
        }

        public async Task<Tutor> GetTutor(Guid tutorId)
        {
            var tutor = await _contaxt.Tutor.FirstOrDefaultAsync(z => z.Id == tutorId);
            return tutor;
        }
    }
}