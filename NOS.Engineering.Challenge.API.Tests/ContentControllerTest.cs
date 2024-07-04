using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NOS.Engineering.Challenge.API.Controllers;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Cache;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.API.Tests;

public class ContentControllerTest
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IContentsManager> _mockContentsManager;
    private readonly ContentController _controller;

    public ContentControllerTest()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockContentsManager = new Mock<IContentsManager>();
        _controller = new ContentController(_mockCacheService.Object, _mockContentsManager.Object);
    }

    #region GetContents

    [Fact]
    public async Task GetManyContents_ReturnsNotFound_WhenNoContentsExist()
    {
        // Arrange
        _mockContentsManager.Setup(m => m.GetManyContents()).ReturnsAsync(new List<Content>());

        // Act
        var result = await _controller.GetManyContents();

        // Assert
        _ = Assert.IsType<NotFoundResult>(result);
        _mockContentsManager.Verify(x => x.GetManyContents(), Times.Once);
    }

    [Fact]
    public async Task GetManyContents_ReturnsOk_WithContents()
    {
        // Arrange
        List<Content> contents = new()
        { 
            new(Guid.NewGuid(), "", "", "", "", 0 , DateTime.UtcNow, DateTime.UtcNow.AddMinutes(10), [])
        };

        _mockContentsManager.Setup(m => m.GetManyContents()).ReturnsAsync(contents);


        // Act
        var result = await _controller.GetManyContents();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<Content>>(okResult.Value);
        Assert.Equal(contents, returnValue);
    }

    #endregion

    #region GetContentById

    [Fact]

    public async Task GetContent_ReturnsNotFound_WhenContentNotInCacheOrDatabase()
    {
        // Arrange
        var contentId = Guid.NewGuid();

        _mockCacheService
            .Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<Content?>>>(), It.IsAny<TimeSpan>()))
            .ReturnsAsync((Content)null);

        // Act
        var result = await _controller.GetContent(contentId);

        // Assert
        var actionResult = Assert.IsType<NotFoundResult>(result);
    }

    #endregion


    #region CreateContent 
    [Fact]
    public async Task CreateContent_ReturnsProblem_WhenCreationFails()
    {
        // Arrange
        var contentInput = new ContentInput();
        _mockContentsManager.Setup(m => m.CreateContent(It.IsAny<ContentDto>())).ReturnsAsync((Content)null);

        // Act
        var result = await _controller.CreateContent(contentInput);

        // Assert
        var actionResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, actionResult.StatusCode);
    }

    [Fact]
    public async Task CreateContent_ReturnsOk_WithCreatedContent()
    {
        // Arrange
        ContentInput contentInput = new();
        Content content = new(Guid.NewGuid(), "", "", "", "", 0, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(10), []);

        _mockContentsManager.Setup(m => m.CreateContent(It.IsAny<ContentDto>())).ReturnsAsync(content);
        _mockCacheService.Setup(c => c.SetAsync(It.IsAny<Guid>(), It.IsAny<Content>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreateContent(contentInput);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Content>(okResult.Value);
        Assert.Equal(content, returnValue);
    }
    #endregion


    #region DeleteContent
    [Fact]
    public async Task DeleteContent_ReturnsOk_WhenContentsExist()
    {
        // Arrange
        Content content = new(Guid.NewGuid(), "", "", "", "", 0, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(10), []);

        _mockContentsManager.Setup(m => m.DeleteContent(It.IsAny<Guid>())).ReturnsAsync(content.Id);
        _mockCacheService.Setup(c => c.RemoveAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteContent(content.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Guid>(okResult.Value);
        Assert.Equal(content.Id, returnValue);

        _mockCacheService.Verify(x => x.RemoveAsync(It.IsAny<Guid>()), Times.Once);
        _mockContentsManager.Verify(x => x.DeleteContent(It.IsAny<Guid>()), Times.Once);
    }
    #endregion
}
