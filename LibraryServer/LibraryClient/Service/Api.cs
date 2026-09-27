using LibraryClient.Models;
using System.Net.Http.Json;

namespace LibraryClient.Service
{
    public static class Api
    {
        // The Android emulator reaches your PC through 10.0.2.2;
        // Windows / iOS / Mac use localhost.
        static readonly HttpClient http = new()
        {
            BaseAddress = new Uri(DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5191"
                : "http://localhost:5191")
        };

        public static async Task<List<Member>> GetMembersAsync()
            => await http.GetFromJsonAsync<List<Member>>("api/Members")
               ?? new List<Member>();

        // Id == 0 means "new Member" -> POST, otherwise -> PUT
        public static async Task SaveMemberAsync(Member Member)
        {
            var response = Member.Id == 0
                ? await http.PostAsJsonAsync("api/Members", Member)
                : await http.PutAsJsonAsync($"api/Members/{Member.Id}", Member);

            response.EnsureSuccessStatusCode();
        }

        public static async Task DeleteMemberAsync(int id)
        {
            var response = await http.DeleteAsync($"api/Members/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}