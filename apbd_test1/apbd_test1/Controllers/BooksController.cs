using apbd_test1.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace apbd_test1.Controllers;

[Route("api/")]
[ApiController]
public class BooksController: ControllerBase
{
    private IBooksRepository _booksRepository;

    public BooksController(IBooksRepository booksRepository)
    {
        _booksRepository = booksRepository;
    }
    
    [HttpGet("books/{id}")]
    public async Task<IActionResult> GetBookData(int id)
    {
        if (await _booksRepository.DoesBookExist(id))
        {
            var bookDTO = await _booksRepository.GetBookData(id);
            bookDTO.authors = await _booksRepository.GetAuthorsData(id);
            bookDTO.genres = await _booksRepository.GetGenresData(id);
            return Ok(bookDTO);
        }
        return NotFound();
    }
    
    [HttpDelete("genres/{id}")]
    public async Task<IActionResult> DeleteGenre(int id)
    {
        if (await _booksRepository.DoesGenreExist(id))
        {
            await _booksRepository.DeleteGenre(id);
            return NoContent();
        }

        return NotFound();
    }
}