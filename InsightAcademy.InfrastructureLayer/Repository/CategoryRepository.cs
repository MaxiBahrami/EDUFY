using InsightAcademy.ApplicationLayer.Repository;
using InsightAcademy.DomainLayer.Entities;
using InsightAcademy.InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.InfrastructureLayer.Implementation
{
    public class CategoryRepository : ICategory
    {
        public readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _context.Category.ToListAsync();
        }

        public async Task<List<string>> GetAllTags()
        {
            var tutorSubjects = await _context.TutorSubject.ToListAsync();

            var allTags = tutorSubjects
                            .Where(ts => ts.Tags != null) // Filter out null Tags
                            .SelectMany(ts => ts.Tags.Split(','))
                            .Select(tag => tag.Trim())
                            .ToList();

            return allTags;
        }

        public async Task<bool> AddCategory(Category category)
        {
            await _context.Category.AddAsync(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EditCategory(Category category)
        {
            var categoryFromDb = await _context.Category.FirstOrDefaultAsync(x => x.Id == category.Id);
            if (categoryFromDb != null)
            {
                categoryFromDb.Name = category.Name;
                _context.Category.Update(category);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> DeleteCategory(Guid categoryId)
        {
            var categoryFromDb = await _context.Category.FirstOrDefaultAsync(x => x.Id == categoryId);
            if (categoryFromDb != null)
            {
                _context.Remove(categoryFromDb);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<Category> GetCategoryById(Guid Id)
        {
            var categoryFromDb = await _context.Category.FirstOrDefaultAsync(x => x.Id == Id);
            if (categoryFromDb != null)
            {
                return categoryFromDb;
            }
            return null;
        }
    }
}