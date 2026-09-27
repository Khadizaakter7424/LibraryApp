namespace LibraryClient.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime DateOfBirth { get; set; } = DateTime.Today;
        public bool IsActive { get; set; } = true;
        public string? ImageUrl { get; set; }          // picture stored as a Base64 string
        public List<BorrowedBook> BorrowedBooks { get; set; } = new();
    }

    public class BorrowedBook
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Genre { get; set; } = "";
        public int MemberId { get; set; }
    }
}