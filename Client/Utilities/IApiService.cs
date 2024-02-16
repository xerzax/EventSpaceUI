using static EventSpaceUI.Client.Pages.Auth.Login;

namespace EventSpaceUI.Client.Utilities
{
    public interface IApiService
    {
        Task<T> CallApiAsync<T>(string url, HttpMethod method, object data = null);
        Task<string> Login(LoginModel model);
    }
}
