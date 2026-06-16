using Server.Data;
using Server.Services;

using TaskModel = Server.Core.Models.Task;
using Task = System.Threading.Tasks.Task;
using Server.Core.Dtos.Task;
using Microsoft.EntityFrameworkCore;

namespace Server.Tests.Services;

[Trait("Service", "TaskService")]
public class TaskServiceTests
{
    [Fact]
    public async Task GetTasksAsync_Can_Return_Empty()
    {
        var (connection, options, config) = await DbTestHelper.SetupTestDbAsync();

        var user = await DbTestHelper.AddUserAsync(options);

        using (var context = new AppDbContext(options))
        {
            var service = new TaskService(context);
            var (tasks, count) = await service.GetTasksAsync(user.Id, 0, 10);

            Assert.Empty(tasks);
            Assert.Equal(0, count);
        }
    }

    [Fact]
    public async Task GetTasksAsync_Returns_Task()
    {
        var (connection, options, config) = await DbTestHelper.SetupTestDbAsync();
        var task = new TaskModel
        {
            Title = "title",
            Body = "body",
        };

        var user = await DbTestHelper.AddUserAsync(options);
        using (var context = new AppDbContext(options))
        {
            task.UserId = user.Id;
            await context.Tasks.AddAsync(task);
            await context.SaveChangesAsync();
        }

        using (var context = new AppDbContext(options))
        {
            var service = new TaskService(context);
            var (tasks, count) = await service.GetTasksAsync(user.Id, 0, 10);
            Assert.Single(tasks);
            Assert.Equal(1, count);
        }

        using (var context = new AppDbContext(options))
        {
            var result = await context.Tasks.SingleAsync();

            Assert.Equal(task.Title, result.Title);
            Assert.Equal(task.Body, result.Body);
        }
    }

    [Fact]
    public async Task CreateTaskAsync_Creates_Task()
    {
        var (connection, options, config) = await DbTestHelper.SetupTestDbAsync();

        var user = await DbTestHelper.AddUserAsync(options);
        var dto = new TaskCreateDto("title", "body", []);

        using (var context = new AppDbContext(options))
        {
            var service = new TaskService(context);
            var response = await service.CreateTaskAsync(user.Id, dto);
            Assert.NotNull(response);
            Assert.Equal(dto.title, response.title);
            Assert.Equal(dto.body, response.body);
        }

        using (var context = new AppDbContext(options))
        {
            var task = await context.Tasks.FirstOrDefaultAsync();

            Assert.NotNull(task);
            Assert.Equal(dto.title, task.Title);
            Assert.Equal(dto.body, task.Body);

        }
    }
}
