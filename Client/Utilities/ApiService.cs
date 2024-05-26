using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static EventSpaceUI.Client.Pages.Auth.Login;
using System.Text.Json;
using Microsoft.JSInterop;
using Blazored.SessionStorage;
using System;
using static EventSpaceUI.Client.Pages.Auth.Register;
using System.Text;
using System.Net.Http;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using EventSpaceUI.Client.Shared.Model;

namespace EventSpaceUI.Client.Utilities
{
    public class ApiService : IApiService
    {

        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7060/api";
        private readonly IJSRuntime _jsRuntime;
        private readonly ISessionStorageService _sessionStorageService;
        private readonly ILocalStorageService _localStorageService;
		private readonly AuthenticationStateProvider _authStateProvider;


		public ApiService(HttpClient httpClient, IJSRuntime jsRuntime, ISessionStorageService sessionStorageService, ILocalStorageService localStorageService, AuthenticationStateProvider authStateProvider)
		{
			_httpClient = httpClient;
			_jsRuntime = jsRuntime;
			_sessionStorageService = sessionStorageService;
			this._localStorageService = localStorageService;
			_authStateProvider = authStateProvider;
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
					//var jsonContent = JsonConvert.SerializeObject(data);

					//response = await _httpClient.PostAsync(fullUrl,new StringContent(jsonContent, Encoding.UTF8, "application/json"));
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
				if (fullUrl.Contains("Follow")) return default;
				return await response.Content.ReadFromJsonAsync<T>();
            }
            else
            {
                return default;
            }
        }

        public async Task<T> CallApiAsyncForm<T>(string url, HttpMethod method, object data = null)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("JWT_TOKEN");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            HttpResponseMessage response;
            string fullUrl = $"{_baseUrl}/{url}";
            HttpContent requestContent = (MultipartFormDataContent)data;
            response = await _httpClient.PostAsync(fullUrl, requestContent);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            else
            {
                return default;
            }
        }

		
		public async Task<(string message, string role)> Login(LoginModel loginModel)
		{
			var url = "Account/Login";
			string fullUrl = $"{_baseUrl}/{url}";

			HttpResponseMessage response = await _httpClient.PostAsJsonAsync(fullUrl, loginModel);


			if (response.IsSuccessStatusCode)
			{
				var responseContent = await response.Content.ReadAsStringAsync();
				var loginResponse = System.Text.Json.JsonSerializer.Deserialize<LoginResponseDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

				if (!string.IsNullOrEmpty(loginResponse.Token))
				{
					await _sessionStorageService.SetItemAsync("JWT_TOKEN", loginResponse.Token);
					//await _authStateProvider.GetAuthenticationStateAsync();
					await _localStorageService.SetItemAsync("token", loginResponse.Token);

					_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.Token);
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.ReadJwtToken(loginResponse.Token);
                    var roleClaim = token.Claims.FirstOrDefault(c => c.Type == "role")?.Value;


                    await _localStorageService.SetItemAsync("role", roleClaim);


                    //return "Login successful";
                    return ("Login successful", roleClaim);
                }
			}

			//return "Invalid email or password.";
            return ("Invalid Login Attempt", null);
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


        public async Task<DataModel[]> GetChartDataAsync(string url, HttpMethod method, object data = null)
        {
            var token = await _sessionStorageService.GetItemAsync<string>("JWT_TOKEN");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            string fullUrl = $"{_baseUrl}/{url}";
            HttpResponseMessage response;

            switch (method.Method)
            {
                case "GET":
                    response = await _httpClient.GetAsync(fullUrl);
                    break;
                case "POST":
                    var content = JsonContent.Create(data);
                    response = await _httpClient.PostAsync(fullUrl, content);
                    break;
                default:
                    throw new ArgumentException("Invalid HTTP method.");
            }

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DataModel[]>();
            }
            else
            {
                return null;
            }
        }

    }
}

