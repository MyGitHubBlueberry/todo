namespace Server.Core.Interfaces;

using Server.Core.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetCategoriesAsync(int userId);
    Task<Category?> GetCategoryByIdAsync(int categoryId, int userId);
    Task<Category> CreateCategoryAsync(Category category);
    Task<bool> DeleteCategoryAsync(int categoryId, int userId);
    Task<Category?> UpdateCategoryAsync(int categoryId, int userId, string newName);
}
