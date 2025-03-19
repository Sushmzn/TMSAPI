using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Data;
using TaskManagementAPI.Entities;
using TaskManagementAPI.Interfaces;
using Xunit;
using TaskManagementAPI.Exceptions;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

public class TasksControllerTests : IDisposable
{
    private readonly TasksController _controller;
    private readonly TaskManagementDbContext _dbContext;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<TasksController>> _mockLogger;

    public TasksControllerTests()
    {
        // Setup In-Memory Database
        var options = new DbContextOptionsBuilder<TaskManagementDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDB")
            .Options;
        _dbContext = new TaskManagementDbContext(options);

        _mockLogger = new Mock<ILogger<TasksController>>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _controller = new TasksController(_dbContext, _mockLogger.Object, _mockUnitOfWork.Object);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Fact]
    public async Task GetTasks_ReturnsListOfTasks()
    {
        // Arrange
        _dbContext.Tasks.AddRange(new List<TaskData>
        {
            new TaskData { Id = 1, Title = "Task 1", Description = "Desc 1", IsCompleted = false },
            new TaskData { Id = 2, Title = "Task 2", Description = "Desc 2", IsCompleted = true }
        });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _controller.GetTasks();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<TaskData>>>(result);
        var tasks = Assert.IsType<List<TaskData>>(actionResult.Value);
        Assert.Equal(2, tasks.Count);
    }

    [Fact]
    public async Task GetTask_ExistingId_ReturnsTask()
    {
        // Arrange
        var task = new TaskData { Id = 1, Title = "Test Task", Description = "Test Description", IsCompleted = false };
        _dbContext.Tasks.Add(task);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _controller.GetTask(1);

        // Assert
        var actionResult = Assert.IsType<ActionResult<TaskData>>(result);
        var returnedTask = Assert.IsType<TaskData>(actionResult.Value);
        Assert.Equal("Test Task", returnedTask.Title);
    }

    [Fact]
    public async Task GetTask_NonExistingId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetTask(999);

        // Assert
        var actionResult = Assert.IsType<ActionResult<TaskData>>(result);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Theory]
    [InlineData(999)]
    [InlineData(1000)]
    [InlineData(-1)]
    public async Task GetTask_NonExistingIds_ReturnsNotFound(int taskId)
    {
        // Act
        var result = await _controller.GetTask(taskId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<TaskData>>(result);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async Task CreateTask_ValidTask_ReturnsCreatedAtAction()
    {
        // Arrange
        var task = new TaskData { Title = "New Task", Description = "New Task Description", IsCompleted = false };

        // Act
        var result = await _controller.CreateTask(task);

        // Assert
        var actionResult = Assert.IsType<ActionResult<TaskData>>(result);
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        Assert.Equal(nameof(_controller.GetTask), createdResult.ActionName);
    }

    [Fact]
    public async Task UpdateTask_ExistingId_ReturnsNoContent()
    {
        // Arrange
        var task = new TaskData { Id = 1, Title = "Task to Update", Description = "Old Description", IsCompleted = false };
        _dbContext.Tasks.Add(task);
        await _dbContext.SaveChangesAsync();

        var updatedTask = new TaskData { Title = "Updated Task", Description = "Updated Description", IsCompleted = true };

        // Act
        var result = await _controller.UpdateTask(1, updatedTask);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateTask_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var updatedTask = new TaskData { Title = "Updated Task", Description = "Updated Description", IsCompleted = true };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () => await _controller.UpdateTask(999, updatedTask));
        Assert.Equal("No Data Found", exception.Message);
    }

    [Fact]
    public async Task DeleteTask_ExistingId_ReturnsNoContent()
    {
        // Arrange
        var task = new TaskData { Id = 1, Title = "Task to Delete", Description = "Delete This", IsCompleted = false };
        _dbContext.Tasks.Add(task);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _controller.DeleteTask(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteTask_NonExistingId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.DeleteTask(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
