namespace Server.Api.Controllers;

using Server.Core.Interfaces;
using Server.Core.Extensions;
using Server.Core.Dtos.Category;

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ILogger<CategoriesController> logger, ICategoryService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CategoryResponseDto>> Create([FromBody] CategoryCreateDto dto)
    {
        if (!User.TryGetUserId(out int userId))
            return Unauthorized(new { message = "Invalid token payload." });

        try
        {
            var category = await service.CreateCategoryAsync(userId, dto.name);

            logger.LogInformation("User {UserId} successfully created category '{CategoryName}' (Id: {CategoryId}).",
                userId, category.name, category.id);

            return Ok(category);
        }
        catch (InvalidDataException ex)
        {
            logger.LogWarning("Failed to create category '{Name}': {Message}",
                    dto.name, ex.Message);
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponseDto>> GetById(int id)
    {
        if (!User.TryGetUserId(out int userId))
            return Unauthorized(new { message = "Invalid token payload." });

        var category = await service.GetCategoryByIdAsync(id, userId);
        if (category is null)
            return NotFound(new { message = "Category doesn't exist." });
        return Ok(category);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> Get()
    {
        if (!User.TryGetUserId(out int userId))
            return Unauthorized(new { message = "Invalid token payload." });

        return Ok(await service.GetCategoriesAsync(userId));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryResponseDto>> Update(int id, [FromBody] CategoryUpdateDto dto)
    {
        if (!User.TryGetUserId(out int userId))
            return Unauthorized(new { message = "Invalid token payload." });

        try
        {
            var category = await service.UpdateCategoryAsync(id, userId, dto.newName);
            logger.LogInformation("User {UserId} successfully updated category '{CategoryName}' (Id: {CategoryId}).",
                userId, category.name, category.id);
            return Ok(category);
        }
        catch (InvalidDataException ex)
        {
            logger.LogWarning("Failed to update category with id '{Id}' with new name '{Name}': {Message}",
                    id, dto.newName, ex.Message);
            return Conflict(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Failed to update category with id '{Id}' with new name '{Name}': {Message}",
                    id, dto.newName, ex.Message);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete()]
    public async Task<ActionResult> Delete([FromBody] CategoryDeleteDto dto)
    {
        if (!User.TryGetUserId(out int userId))
            return Unauthorized(new { message = "Invalid token payload." });
        if (dto.ids == null || dto.ids.Length == 0)
            return BadRequest("No category IDs provided.");

        var result = await service.DeleteCategoriesAsync(dto.ids, userId);
        if (!result)
        {
            logger.LogWarning("Failed to delete categories");
            return NotFound(new { message = "Categories don't exist" });
        }

        logger.LogInformation("User {UserId} successfully deleted categories {ids}.",
                userId, dto.ids);

        return NoContent();
    }
}
