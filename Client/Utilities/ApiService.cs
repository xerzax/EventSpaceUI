
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static EventSpaceUI.Client.Pages.Auth.Login;
using System.Text.Json;
using Microsoft.JSInterop;
using Blazored.SessionStorage;
using System;
using static EventSpaceUI.Client.Pages.Auth.Register;

namespace EventSpaceUI.Client.Utilities
{
    public class ApiService : IApiService
    {

        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7060/api";
        private readonly IJSRuntime _jsRuntime;
        private readonly ISessionStorageService _sessionStorageService;


        public ApiService(HttpClient httpClient, IJSRuntime jsRuntime, ISessionStorageService sessionStorageService)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            _sessionStorageService = sessionStorageService;
        }

        public async Task<T> CallApiAsync<T>(string url, HttpMethod method, object data = null)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("JWT_TOKEN");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            HttpResponseMessage response;
            string fullUrl = $"{_baseUrl}/{url}";

            switch (method.Method)
            {
                case "GET":
                    response = await _httpClient.GetAsync(fullUrl);
                    break;
                case "POST":
                    response = await _httpClient.PostAsJsonAsync(fullUrl, data);
                    break;
                case "PUT":
                    response = await _httpClient.PutAsJsonAsync(fullUrl, data);
                    break;
                case "DELETE":
                    response = await _httpClient.DeleteAsync(fullUrl);
                    break;
                default:
                    throw new ArgumentException("Invalid HTTP method");
            }

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            else
            {
                return default;
            }
        }

        public async Task<string> Login(LoginModel loginModel)
        {
            var url = "Account/Login";
            string fullUrl = $"{_baseUrl}/{url}";

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(fullUrl, loginModel);
            //var loginJson = JsonSerializer.Serialize(loginModel);
            //var loginContent = new StringContent(loginJson, System.Text.Encoding.UTF8, "application/json");

            //var response = await _httpClient.PostAsync("Account/login", loginContent);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (!string.IsNullOrEmpty(loginResponse.Token))
                {
                    // Store token in local storage
                    //await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", loginResponse.Token);
                    await _sessionStorageService.SetItemAsync("JWT_TOKEN", loginResponse.Token);
                    // Attach token to headers of HttpClient
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.Token);

                    return "Login successful";
                }
            }

            return "Invalid email or password.";
        }
         
        public async Task<string> Register(RegisterModel registerModel)
        {
            var url = "Account/register";
            string fullUrl = $"{_baseUrl}/{url}";
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(fullUrl, registerModel);
            if (response.IsSuccessStatusCode)
            {
                return "Registration successful";
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return "Registration Failed:" + errorResponse;
            }

        }
    }
}

