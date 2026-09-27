
using LibraryServer.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("con")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Create the database / apply migrations automatically when the server starts
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var Members = app.MapGroup("/api/Members");

// GET all (with BorrowedBooks)
Members.MapGet("/", async (AppDbContext db) =>
    await db.Members.Include(s => s.BorrowedBooks).ToListAsync());

// GET by id
Members.MapGet("/{id:int}", async (int id, AppDbContext db) =>
    await db.Members.Include(s => s.BorrowedBooks).FirstOrDefaultAsync(s => s.Id == id)
        is Member Member ? Results.Ok(Member) : Results.NotFound());

// POST - one SaveChanges saves the Member AND all its BorrowedBooks
Members.MapPost("/", async (Member Member, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(Member.Name))
        return Results.BadRequest("Name is required.");

    db.Members.Add(Member);
    await db.SaveChangesAsync();
    return Results.Created($"/api/Members/{Member.Id}", Member);
});

// PUT - update the Member, replace its BorrowedBooks
Members.MapPut("/{id:int}", async (int id, Member input, AppDbContext db) =>
{
    var Member = await db.Members.Include(s => s.BorrowedBooks)
                                   .FirstOrDefaultAsync(s => s.Id == id);
    if (Member is null)
        return Results.NotFound();
    if (string.IsNullOrWhiteSpace(input.Name))
        return Results.BadRequest("Name is required.");

    Member.Name = input.Name;
    Member.DateOfBirth = input.DateOfBirth;
    Member.IsActive = input.IsActive;
    Member.ImageUrl = input.ImageUrl;

    // Simple way to update the details: remove all old BorrowedBooks, add the new list
    Member.BorrowedBooks.Clear();
    foreach (var a in input.BorrowedBooks)
        Member.BorrowedBooks.Add(new BorrowedBook { Title = a.Title, Genre = a.Genre });

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// DELETE - the BorrowedBooks are deleted automatically (cascade delete)
Members.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
{
    var Member = await db.Members.FindAsync(id);
    if (Member is null)
        return Results.NotFound();

    db.Members.Remove(Member);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();