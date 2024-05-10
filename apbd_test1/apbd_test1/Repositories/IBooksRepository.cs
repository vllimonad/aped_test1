using apbd_test1.Models.DTOs;

namespace apbd_test1.Repositories;

public interface IBooksRepository
{
    Task<bool> DoesBookExist(int id);
    Task<bool> DoesGenreExist(int id);
    Task<BookDTO> GetBookData(int id);
    Task<List<AuthorDTO>> GetAuthorsData(int id);
    Task<List<string>> GetGenresData(int id);
    Task DeleteGenre(int id);
}