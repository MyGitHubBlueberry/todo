namespace Server.Services;

using Server.Core.Interfaces;
using Server.Core.Models;
using Server.Data;

using System.Collections.Generic;
using System.Threading.Tasks;

public class CategoryService(AppDbContext db) : ICategoryService
{
    Task<Category> ICategoryService.CreateCategoryAsync(Category category)
    {
        throw new System.NotImplementedException();
    }

    Task<bool> ICategoryService.DeleteCategoryAsync(int categoryId, int userId)
    {
        throw new System.NotImplementedException();
    }

    Task<IEnumerable<Category>> ICategoryService.GetCategoriesAsync(int userId)
    {
        throw new System.NotImplementedException();
    }

    Task<Category?> ICategoryService.GetCategoryByIdAsync(int categoryId, int userId)
    {
        throw new System.NotImplementedException();
    }

    Task<Category?> ICategoryService.UpdateCategoryAsync(int categoryId, int userId, string newName)
    {
        throw new System.NotImplementedException();
    }
}
