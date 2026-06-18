namespace Server.Core.Interfaces;

using Server.Core.Dtos.Category;

using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponseDto>> GetCategoriesAsync(int userId);
    Task<CategoryResponseDto?> GetCategoryByIdAsync(int categoryId, int userId);
    Task<CategoryResponseDto> CreateCategoryAsync(int userId, string name);
    Task<bool> DeleteCategoriesAsync(int[] categoryIds, int userId);
    Task<CategoryResponseDto> UpdateCategoryAsync(int categoryId, int userId, string newName);
}
