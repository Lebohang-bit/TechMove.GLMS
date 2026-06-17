using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TechMove.GLMS.API.Models;


namespace TechMove.GLMS.Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void SetAuthToken()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // ============ AUTH ============
        public async Task<string?> LoginAsync(string username, string password)
        {
            var loginData = new { username, password };
            var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                return result?.GetValueOrDefault("token");
            }
            return null;
        }

        // ============ CLIENTS ============
        public async Task<List<Client>> GetClientsAsync()
        {
            SetAuthToken();
            var response = await _httpClient.GetAsync("api/clients");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Client>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Client>();
        }

        public async Task<Client?> GetClientAsync(int id)
        {
            SetAuthToken();
            var response = await _httpClient.GetAsync($"api/clients/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Client>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<Client?> CreateClientAsync(Client client)
        {
            SetAuthToken();
            var content = new StringContent(JsonSerializer.Serialize(client), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/clients", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Client>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<Client?> UpdateClientAsync(int id, Client client)
        {
            SetAuthToken();
            var content = new StringContent(JsonSerializer.Serialize(client), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/clients/{id}", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Client>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            SetAuthToken();
            var response = await _httpClient.DeleteAsync($"api/clients/{id}");
            return response.IsSuccessStatusCode;
        }

        // ============ AGREEMENTS / CONTRACTS ============
        public async Task<List<Agreement>> GetContractsAsync(string? status = null, DateTime? start = null, DateTime? end = null)
        {
            SetAuthToken();
            var url = $"api/Contracts?status={status}&startDate={start:yyyy-MM-dd}&endDate={end:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Agreement>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Agreement>();
        }

        public async Task<Agreement?> GetContractAsync(int id)
        {
            SetAuthToken();
            var response = await _httpClient.GetAsync($"api/Contracts/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Agreement>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<Agreement?> CreateContractAsync(Agreement agreement)
        {
            SetAuthToken();
            var content = new StringContent(JsonSerializer.Serialize(agreement), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Contracts", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Agreement>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<bool> UpdateContractStatusAsync(int id, string status)
        {
            SetAuthToken();
            var content = new StringContent($"\"{status}\"", Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync($"api/Contracts/{id}/status", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            SetAuthToken();
            var response = await _httpClient.DeleteAsync($"api/Contracts/{id}");
            return response.IsSuccessStatusCode;
        }

        // ============ SERVICE REQUESTS ============
        public async Task<List<ServiceRequest>> GetServiceRequestsAsync(string? status = null, DateTime? start = null, DateTime? end = null)
        {
            SetAuthToken();
            var url = $"api/ServiceRequests?status={status}&startDate={start:yyyy-MM-dd}&endDate={end:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ServiceRequest>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<ServiceRequest>();
        }

        public async Task<ServiceRequest?> GetServiceRequestAsync(int id)
        {
            SetAuthToken();
            var response = await _httpClient.GetAsync($"api/ServiceRequests/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ServiceRequest>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<ServiceRequest?> CreateServiceRequestAsync(ServiceRequest request)
        {
            SetAuthToken();
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/ServiceRequests", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ServiceRequest>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<bool> UpdateServiceRequestStatusAsync(int id, string status)
        {
            SetAuthToken();
            var content = new StringContent($"\"{status}\"", Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync($"api/ServiceRequests/{id}/status", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteServiceRequestAsync(int id)
        {
            SetAuthToken();
            var response = await _httpClient.DeleteAsync($"api/ServiceRequests/{id}");
            return response.IsSuccessStatusCode;
        }

        // ============ EXCHANGE RATE ============
        public async Task<decimal> GetExchangeRateAsync()
        {
            var response = await _httpClient.GetAsync("api/ServiceRequests/exchangerate");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<Dictionary<string, decimal>>(json);
            return data?.GetValueOrDefault("rate", 19.50m) ?? 19.50m;
        }
    }
}