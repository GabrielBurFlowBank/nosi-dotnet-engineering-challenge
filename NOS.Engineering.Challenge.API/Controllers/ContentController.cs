using Microsoft.AspNetCore.Mvc;
using NOS.Engineering.Challenge.API.Filter;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Cache;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ContentController : Controller
{
    private readonly ICacheService _cache;
    private readonly IContentsManager _manager;
    public ContentController(ICacheService cache, IContentsManager manager)
    {
        _cache = cache;
        _manager = manager;
    }

    [HttpGet]
    [Obsolete]
    public async Task<IActionResult> GetManyContents()
    {
        var contents = await _manager.GetManyContents().ConfigureAwait(false);

        if (!contents.Any())
            return NotFound();
        
        return Ok(contents);
    }

    [HttpGet("filtered")]
    public async Task<IActionResult> GetContents(
        [FromQuery] ContentQueryParams parameters
        )
    {
        var queryValue = Request.QueryString.Value ?? string.Empty;

        var cachedContent = await _cache.GetOrSetAsync<IEnumerable<Content?>>(queryValue, () => _manager.GetFilteredContents(parameters.Title, parameters.Genre));

        if (!cachedContent.Any())
            return NotFound();

        return Ok(cachedContent);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContent(Guid id)
    {
        var cachedContent = await _cache.GetOrSetAsync<Content?>(id.ToString(), () => _manager.GetContent(id));

        if (cachedContent == null)
            return NotFound();
        
        return Ok(cachedContent);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateContent(
        [FromBody] ContentInput content
        )
    {
        var createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(false);

        if (createdContent is null)
            return Problem();

        await _cache.SetAsync(createdContent.Id, createdContent).ConfigureAwait(false);
        
        return Ok(createdContent);
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateContent(
        Guid id,
        [FromBody] ContentInput content
        )
    {
        var updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);

        if(updatedContent is null)
            return Problem();

        await _cache.SetAsync(id, updatedContent).ConfigureAwait(false);

        return Ok(updatedContent);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContent(
        Guid id
    )
    {
        var deletedId = await _manager.DeleteContent(id).ConfigureAwait(false);
        
        if(deletedId == Guid.Empty)
            return Problem();

        await _cache.RemoveAsync(id).ConfigureAwait(false); 

        return Ok(deletedId);
    }
    
    [HttpPost("{id}/genre")]
    public async Task<IActionResult> AddGenres(
        Guid id,
        [FromBody] IEnumerable<string> genre
    )
    {
        var updateContent = await _manager.AddGenres(id, genre).ConfigureAwait(false);

        if(updateContent is null)
            return Problem();

        await _cache.SetAsync(id, updateContent).ConfigureAwait(false);

        return Ok(updateContent);
    }
    
    [HttpDelete("{id}/genre")]
    public async Task<IActionResult> RemoveGenres(
        Guid id,
        [FromBody] IEnumerable<string> genre
    )
    {
        var updateContent = await _manager.RemoveGenres(id, genre).ConfigureAwait(false);

        if (updateContent is null)
            return Problem();

        await _cache.SetAsync(id, updateContent).ConfigureAwait(false);

        return Ok(updateContent);
    }
}