namespace apbd_test1.Models.DTOs;

public class BookDTO
{
    public int id { get; set; }
    public string title { get; set; }
    public List<AuthorDTO> authors { get; set; }
    public List<string> genres { get; set; }
}
