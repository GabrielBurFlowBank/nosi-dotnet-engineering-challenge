using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Managers;

public interface IContentsManager
{
    [Obsolete]
    Task<IEnumerable<Content?>> GetManyContents();
    Task<IEnumerable<Content?>> GetFilteredContents(string? title, string? genre);
    Task<Content?> CreateContent(ContentDto content);
    Task<Content?> GetContent(Guid id);
    Task<Content?> UpdateContent(Guid id, ContentDto content);
    Task<Guid> DeleteContent(Guid id);
    Task<Content?> AddGenres(Guid id, IEnumerable<string> genres);
    Task<Content?> RemoveGenres(Guid id, IEnumerable<string> genres);
}