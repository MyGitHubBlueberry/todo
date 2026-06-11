namespace Server.Services;

using Server.Data;
using Server.Core.Dtos.Category;
using Server.Core.Interfaces;

using System.IO;
using System.Linq;
using Server.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;

public class CategoryService(AppDbContext db) : ICategoryService
{
    public async Task<CategoryResponseDto> CreateCategoryAsync(int userId, string name)
    {
        if (await db.Categories.AnyAsync(c => c.UserId == userId && c.Name == name))
            throw new InvalidDataException("Failed to create category: category with this name already exists.");

        var category = new Category
        {
            Name = name,
            UserId = userId,
        };

        await db.Categories.AddAsync(category);
        await db.SaveChangesAsync();

        return new CategoryResponseDto(category.Id, category.Name);
    }

    public async Task<bool> DeleteCategoryAsync(int categoryId, int userId) =>
        (await db.Categories
            .Where(c => c.Id == categoryId && c.UserId == userId)
            .ExecuteDeleteAsync()) != 0;

    public async Task<IEnumerable<CategoryResponseDto>> GetCategoriesAsync(int userId) =>
        await db.Categories
            .Where(c => c.UserId == userId)
            .Select(c => new CategoryResponseDto(c.Id, c.Name))
            .ToArrayAsync();

    public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int categoryId, int userId) =>
        await db.Categories
            .Where(c => c.Id == categoryId && c.UserId == userId)
            .Select(c => new CategoryResponseDto(c.Id, c.Name))
            .FirstOrDefaultAsync();


    public async Task<CategoryResponseDto> UpdateCategoryAsync(int categoryId, int userId, string newName)
    {
        var category = await db.Categories
            .SingleOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);

        if (category is null)
            throw new InvalidOperationException("Failed to update category: doesn't exist.");

        if (category.Name == newName)
            return new CategoryResponseDto(category.Id, category.Name);

        if (await db.Categories.AnyAsync(c => c.UserId == userId
                    && c.Name == newName
                    && c.Id != categoryId))
            throw new InvalidDataException("Failed to update category: category with this name already exists.");

        category.Name = newName;
        await db.SaveChangesAsync();

        return new CategoryResponseDto(category.Id, category.Name);
    }
}
