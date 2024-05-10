using System.Data.Common;
using apbd_test1.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace apbd_test1.Repositories;

public class BooksRepository: IBooksRepository
{
    private readonly IConfiguration _configuration;

    public BooksRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<bool> DoesBookExist(int id)
    {
        var query = "select 1 from books where PK = @id";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result is not null;
    }

    public async Task<bool> DoesGenreExist(int id)
    {
        var query = "select 1 from genres where PK = @id";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return result is not null;
    }

    public async Task<BookDTO> GetBookData(int id)
    {
        var query = "select books.PK as bookID, books.title as bookTitle " +
                    "from books " +
                    "where books.PK = @id;";
        
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        SqlCommand command = new SqlCommand();
        
        command.CommandText = query;
        command.Connection = connection;
        command.Parameters.AddWithValue("@id", id);

        await connection.OpenAsync();
        var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();

        if (!reader.HasRows) throw new Exception();

        var bookDTO = new BookDTO()
        {
            id = reader.GetInt32(reader.GetOrdinal("bookID")),
            title = reader.GetString(reader.GetOrdinal("bookTitle")),
        };
        return bookDTO;
    }
    
    public async Task<List<AuthorDTO>> GetAuthorsData(int id)
    {
        var query = "select authors.first_name as authors_first_name, authors.last_name as authors_last_name " +
                    "from authors " +
                    "join books_authors on FK_author = authors.PK " +
                    "where books_authors.FK_book = @id;";
        
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        SqlCommand command = new SqlCommand();
        
        command.CommandText = query;
        command.Connection = connection;
        command.Parameters.AddWithValue("@id", id);

        await connection.OpenAsync();
        var reader = await command.ExecuteReaderAsync();
        var authorDTOs = new List<AuthorDTO>();
        while (await reader.ReadAsync())
        {
            var authorDTO = new AuthorDTO()
            {
                firstName = reader.GetString(reader.GetOrdinal("authors_first_name")),
                lastName = reader.GetString(reader.GetOrdinal("authors_last_name")),
            };
            authorDTOs.Add(authorDTO);
        }
        return authorDTOs;
    }
    
    public async Task<List<string>> GetGenresData(int id)
    {
        var query = "select genres.name as name " +
                    "from genres " +
                    "join books_genres on FK_genre = genres.PK " +
                    "where books_genres.FK_book = @id;";
        
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        SqlCommand command = new SqlCommand();
        
        command.CommandText = query;
        command.Connection = connection;
        command.Parameters.AddWithValue("@id", id);

        await connection.OpenAsync();
        var reader = await command.ExecuteReaderAsync();
        var genresDTO = new List<string>();
        while (await reader.ReadAsync())
        {
            genresDTO.Add(reader.GetString(reader.GetOrdinal("name")));
        }
        return genresDTO;
    }

    public async Task DeleteGenre(int id)
    {
        var query1 = "delete from books_genres where FK_genre = @id;";
        var query2 = "delete from genres where genres.PK = @id;";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        
        await connection.OpenAsync();
        
        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = (SqlTransaction)transaction;
        try
        {
            command.Parameters.Clear();
            command.CommandText = query1;
            command.Parameters.AddWithValue("@id", id);
            await command.ExecuteNonQueryAsync();
            
            command.Parameters.Clear();
            command.CommandText = query2;
            command.Parameters.AddWithValue("@id", id);
            await command.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}