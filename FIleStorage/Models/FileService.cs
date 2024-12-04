using FIleStorage.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FIleStorage.Services
{
    public class FileService
    {
        private readonly HttpClient _httpClient;

        public FileService()
        {
            _httpClient = new HttpClient();
        }

        // Метод для получения файлов пользователя по userId
        public async Task<List<FileModel>> GetUserFilesAsync(int userId, string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // ИЗМЕНИТЬ АПИ НА ВЕРНЫЙ 
            HttpResponseMessage response = await client.GetAsync($"http://course-project-4/api/files?userId={userId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var files = JsonSerializer.Deserialize<List<FileModel>>(content);
                return files;
            }

            return new List<FileModel>(); // Если нет файлов или ошибка, возвращаем пустой список
        }
    }
}
