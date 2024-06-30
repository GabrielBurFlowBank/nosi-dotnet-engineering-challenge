using Microsoft.IdentityModel.Tokens;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Managers;

public class ContentsManager : IContentsManager
{
    private readonly IDatabase<Content?, ContentDto> _database;

    public ContentsManager(IDatabase<Content?, ContentDto> database)
    {
        _database = database;
    }

    [Obsolete]
    public Task<IEnumerable<Content?>> GetManyContents()
    {
        return _database.ReadAll();
    }

    public async Task<IEnumerable<Content?>> GetFilteredContents(string? title, string? genre)
    {
        var allContents = await _database.ReadAll().ConfigureAwait(false);

        if (allContents.IsNullOrEmpty())
            return Enumerable.Empty<Content?>();

        if (!string.IsNullOrWhiteSpace(title))
            allContents = allContents.Where(x => x.Title.Contains(title, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(genre))
            allContents = allContents.Where(x => x.GenreList.Any(y => y.Equals(genre, StringComparison.OrdinalIgnoreCase)));

        return allContents;
    }

    public Task<Content?> CreateContent(ContentDto content)
    {
        return _database.Create(content);
    }

    public Task<Content?> GetContent(Guid id)
    {
        return _database.Read(id);
    }

    public Task<Content?> UpdateContent(Guid id, ContentDto content)
    {
        return _database.Update(id, content);
    }

    public Task<Guid> DeleteContent(Guid id)
    {
        return _database.Delete(id);
    }

    public Task<Content?> AddGenres(Guid id, IEnumerable<string> genres)
    {
        var existingContent = _database.Read(id);

        if (existingContent.Result is null)
            return existingContent;

        var existingGenres = existingContent.Result.GenreList ?? Array.Empty<string>();

        List<string> genresToUpdate = [.. existingGenres, .. genres];

        var dto = new ContentDto(genresToUpdate.Distinct(StringComparer.CurrentCultureIgnoreCase));

        return _database.Update(id, dto);
    }

    public Task<Content?> RemoveGenres(Guid id, IEnumerable<string> genres)
    {
        var existingContent = _database.Read(id);

        if (existingContent.Result is null || existingContent.Result.GenreList is null)
            return existingContent;

        List<string> genresToRemove = [.. existingContent.Result.GenreList ?? Array.Empty<string>()];

        genresToRemove.RemoveAll(x => genres.Contains(x, StringComparer.CurrentCultureIgnoreCase));

        return _database.Update(id, new ContentDto(genresToRemove));
    }

}