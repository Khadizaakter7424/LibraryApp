using LibraryClient.Models;
using LibraryClient.Service;

namespace LibraryClient;

public partial class MemberListPage : ContentPage
{
    public MemberListPage()
    {
        InitializeComponent();
    }

    // Runs the first time AND every time we come back from the form page,
    // so the list is always fresh.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    async Task LoadAsync()
    {
        try
        {
            MembersView.ItemsSource = await Api.GetMembersAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error",
                "Could not load Members. Is the Server running?\n" + ex.Message, "OK");
        }
    }

    async void OnAddClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new MemberFormPage(new Member()));
    }

    async void OnEditClicked(object? sender, EventArgs e)
    {
        // The button's BindingContext is the Member of that row
        if (sender is Button { BindingContext: Member Member })
            await Navigation.PushAsync(new MemberFormPage(Member));
    }

    async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { BindingContext: Member Member })
            return;

        bool yes = await DisplayAlertAsync("Delete", $"Delete {Member.Name}?", "Yes", "No");
        if (!yes)
            return;

        try
        {
            await Api.DeleteMemberAsync(Member.Id);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }
}