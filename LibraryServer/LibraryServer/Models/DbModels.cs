using Microsoft.EntityFrameworkCore;

namespace LibraryServer.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime DateOfBirth { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }      // picture stored as Base64 text
        public List<BorrowedBook> BorrowedBooks { get; set; } = new List<BorrowedBook>();
    }

    public class BorrowedBook
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Genre { get; set; } = "";
        public int MemberId { get; set; }         // foreign key -> Member.Id
    }

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Member> Members { get; set; }
        public DbSet<BorrowedBook> BorrowedBooks { get; set; }
    }
}