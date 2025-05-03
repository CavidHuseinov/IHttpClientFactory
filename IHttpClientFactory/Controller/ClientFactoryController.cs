using IHttpClientFactory.Settings;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;

namespace IHttpClientFactory.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientFactoryController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly Data _data;

        public ClientFactoryController(System.Net.Http.IHttpClientFactory httpClientFactory, IOptions<Data> data)
        {
            _httpClient = httpClientFactory.CreateClient();
            _data = data.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var allData = await _httpClient.GetAsync(_data.Resource);
                allData.EnsureSuccessStatusCode();
                var content = await allData.Content.ReadAsStringAsync();
                return Ok(content);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Api elimiz catmir. Sistem xetasi: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(string title, float price, string description, string image)
        {
            var request = new
            {
                title = title,
                price = price,
                description = description,
                image = image
            };
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_data.Resource, content);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"xarici api yeni melumat elave etmek mumkun olmadi. Server xetasi: {ex.Message}");
            }
        }

        [HttpDelete("delete-foreign-data/{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_data.Resource}/{id}");
                response.EnsureSuccessStatusCode();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Silmek mumkun olmadi. Server xetasi {ex.Message}");
            }
        }

        [HttpPut("update-foreign-data")]
        public async Task<IActionResult> UpdateAsync(string title, float price, string description, string image, int id)
        {
            var request = new
            {
                title = title,
                price = price,
                description = description,
                image = image
            };
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_data.Resource}/{id}", content);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Api yenilemek mumkun olmadi. Server xetasi: {ex.Message}");
            }
        }
    }
}