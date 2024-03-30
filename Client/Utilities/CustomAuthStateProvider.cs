using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace EventSpaceUI.Client.Utilities
{
	public class CustomAuthStateProvider: AuthenticationStateProvider
	{
		private readonly ILocalStorageService _localStorageService;
		public CustomAuthStateProvider(ILocalStorageService localStorageService)
		{
			_localStorageService = localStorageService;
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			string token = await _localStorageService.GetItemAsStringAsync("token");
			var identity = new ClaimsIdentity();

			if (!string.IsNullOrEmpty(token))
			{
				identity = new ClaimsIdentity(ParseClaimsFromJWT(token), "jwt");
			}

			var user = new ClaimsPrincipal(identity);
			var state = new AuthenticationState(user);


			NotifyAuthenticationStateChanged(Task.FromResult(state));
			return state;
		}

		public void NotifyLogout()
		{
			var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
			var anonymousState = new AuthenticationState(anonymousUser);
			NotifyAuthenticationStateChanged(Task.FromResult(anonymousState));
		}


		public static IEnumerable<Claim> ParseClaimsFromJWT(string jwt)
		{
			var payload = jwt.Split('.')[1];

			var jsonBytes = ParseBase64WOPadding(payload);

			var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
			return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));

		}

		private static byte[] ParseBase64WOPadding(string payload)
		{
			switch (payload.Length % 4)
			{
				case 2: payload += "=="; break;
				case 3: payload += "="; break;

			}
			return Convert.FromBase64String(payload);


		}

	}
}
	

