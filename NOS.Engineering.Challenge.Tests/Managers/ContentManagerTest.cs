using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
using Moq;

namespace NOS.Engineering.Challenge.Tests.Managers;

public class ContentManagerTest
{
    private readonly Mock<IDatabase<Content?, ContentDto>> _mockDatabase;
    private readonly IContentsManager _contentsManager;

    public ContentManagerTest()
    {
        _mockDatabase = new Mock<IDatabase<Content?, ContentDto>>();
        _contentsManager = new ContentsManager(_mockDatabase.Object);
    }

    private Content GenerateDefaultContent()
    {
        return new Content(Guid.NewGuid(), "Title1", "Genre1", "Description", "Image", 120, DateTime.UtcNow, DateTime.Now, Array.Empty<string>());
    }

    private ContentDto GenerateDefaultContentDto()
    {
        return new ContentDto("Title1", "Genre1", "Description", "Image", 120, DateTime.UtcNow, DateTime.Now, Array.Empty<string>());
    }


    [Fact]
    public async Task GetManyContents_ShouldReturnContents()
    {
        // Arrange
        var contents = new List<Content?>() { GenerateDefaultContent() };

        _mockDatabase.Setup(db => db.ReadAll()).ReturnsAsync(contents);

        // Act
        var result = await _contentsManager.GetManyContents();

        // Assert
        Assert.Equal(contents, result);
        _mockDatabase.Verify(x => x.ReadAll(), Times.Once);

    }

    [Fact]
    public async Task CreateContent_ShouldCallDatabaseCreate()
    {
        // Arrange
        ContentDto contentDto = GenerateDefaultContentDto();
        Content content = GenerateDefaultContent();

        _mockDatabase.Setup(db => db.Create(contentDto)).ReturnsAsync(content);

        // Act
        var result = await _contentsManager.CreateContent(contentDto);

        // Assert
        Assert.Equal(content, result);
        _mockDatabase.Verify(db => db.Create(contentDto), Times.Once);
    }

    [Fact]
    public async Task GetContent_ShouldReturnContent()
    {
        // Arrange
        var content = GenerateDefaultContent();
        _mockDatabase.Setup(db => db.Read(content.Id)).ReturnsAsync(content);

        // Act
        var result = await _contentsManager.GetContent(content.Id);

        // Assert
        Assert.Equal(content, result);
        _mockDatabase.Verify(x => x.Read(content.Id), Times.Once);
    }

    [Fact]
    public async Task UpdateContent_ShouldCallDatabaseUpdate()
    {
        // Arrange
        var updatedContent = GenerateDefaultContent();
        var contentDto = GenerateDefaultContentDto();
        _mockDatabase.Setup(db => db.Update(updatedContent.Id, contentDto)).ReturnsAsync(updatedContent);

        // Act
        var result = await _contentsManager.UpdateContent(updatedContent.Id, contentDto);

        // Assert
        Assert.Equal(updatedContent, result);
        _mockDatabase.Verify(db => db.Update(updatedContent.Id, contentDto), Times.Once);
    }

    [Fact]
    public async Task DeleteContent_ShouldCallDatabaseDelete()
    {
        // Arrange
        var contentId = Guid.NewGuid();
        _mockDatabase.Setup(db => db.Delete(contentId)).ReturnsAsync(contentId);

        // Act
        var result = await _contentsManager.DeleteContent(contentId);

        // Assert
        Assert.Equal(contentId, result);
        _mockDatabase.Verify(db => db.Delete(contentId), Times.Once);
    }

    [Fact]
    public async Task AddGenres_ShouldUpdateContentGenres()
    {
        // Arrange
        var contentId = Guid.NewGuid();
        Content initialContent = new(contentId, "Title1", "Genre1", "Description", "Image", 120, DateTime.UtcNow, DateTime.Now, new string[] { "Genre1" });
        Content updatedContent = new(contentId, "Title1", "Genre1", "Description", "Image", 120, DateTime.UtcNow, DateTime.Now, new string[] { "Genre1", "Genre2" });

        _mockDatabase.Setup(db => db.Read(contentId)).ReturnsAsync(initialContent);
        _mockDatabase.Setup(db => db.Update(contentId, It.IsAny<ContentDto>())).ReturnsAsync(updatedContent);

        // Act
        var result = await _contentsManager.AddGenres(contentId, new List<string> { "Genre2" });

        // Assert
        Assert.Equal(updatedContent, result);
        _mockDatabase.Verify(db => db.Update(contentId, It.Is<ContentDto>(dto => dto.GenreList.SequenceEqual(updatedContent.GenreList))), Times.Once);
    }

    [Fact]
    public async Task RemoveGenres_ShouldUpdateContentGenres()
    {
        // Arrange
        var contentId = Guid.NewGuid();
        Content initialContent = new(contentId, "Title1", "Genre1", "Description", "Image", 120, DateTime.UtcNow, DateTime.Now, new string[] { "Genre1", "Genre2" });
        Content updatedContent = new(contentId, "Title1", "Genre1", "Description", "Image", 120, DateTime.UtcNow, DateTime.Now, new string[] { "Genre1" });

        _mockDatabase.Setup(db => db.Read(contentId)).ReturnsAsync(initialContent);
        _mockDatabase.Setup(db => db.Update(contentId, It.IsAny<ContentDto>())).ReturnsAsync(updatedContent);

        // Act
        var result = await _contentsManager.RemoveGenres(contentId, new List<string> { "Genre2" });

        // Assert
        Assert.Equal(updatedContent, result);
        _mockDatabase.Verify(db => db.Update(contentId, It.Is<ContentDto>(dto => dto.GenreList.SequenceEqual(updatedContent.GenreList))), Times.Once);
    }

}
