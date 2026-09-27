using LibraryClient.Models;
using LibraryClient.Service;
using System.Collections.ObjectModel;

namespace LibraryClient;

public partial class MemberFormPage : ContentPage
{
    public Member Member { get; }
    public ObservableCollection<BorrowedBook> BorrowedBooks { get; }

    public MemberFormPage(Member Member)
    {
        this.Member = Member;   // fixed: "this." makes sure the property is actually set
        BorrowedBooks = new ObservableCollection<BorrowedBook>(Member.BorrowedBooks);
        Title = Member.Id == 0 ? "New Member" : "Edit Member";

        InitializeComponent();
        BindingContext = this;   // XAML binds to Member.Name, BorrowedBooks ...

        Photo.Source = Base64ToImageConverter.ToImageSource(Member.ImageUrl);
    }

    async void OnChooseImageClicked(object? sender, EventArgs e)
    {
        try
        {
            var file = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Choose a picture",
                FileTypes = FilePickerFileType.Images
            });
            if (file is null)
                return;

            using var stream = await file.OpenReadAsync();
            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);

            Member.ImageUrl = Convert.ToBase64String(memory.ToArray());
            Photo.Source = Base64ToImageConverter.ToImageSource(Member.ImageUrl);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    void OnAddBorrowedBookClicked(object? sender, EventArgs e)
    {
        BorrowedBooks.Add(new BorrowedBook());       // a new empty row to fill in
    }

    void OnRemoveBorrowedBookClicked(object? sender, EventArgs e)
    {
        if (sender is Button { BindingContext: BorrowedBook BorrowedBook })
            BorrowedBooks.Remove(BorrowedBook);
    }

    async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Member.Name))
        {
            await DisplayAlertAsync("Required", "Please enter a name.", "OK");
            return;
        }

        // keep only the BorrowedBook rows that were actually filled in
        Member.BorrowedBooks = BorrowedBooks
            .Where(a => !string.IsNullOrWhiteSpace(a.Title)
                     || !string.IsNullOrWhiteSpace(a.Genre))
            .ToList();

        try
        {
            await Api.SaveMemberAsync(Member);
            await Navigation.PopAsync();    // back to the list, which reloads itself
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }
}