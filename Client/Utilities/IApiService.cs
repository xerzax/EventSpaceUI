using static EventSpaceUI.Client.Pages.Auth.Login;
using static EventSpaceUI.Client.Pages.Auth.Register;

namespace EventSpaceUI.Client.Utilities
{
    public interface IApiService
    {
        Task<T> CallApiAsync<T>(string url, HttpMethod method, object data = null);
        Task<string> Login(LoginModel model);
        Task<string> Register(RegisterModel registerModel);
    }
}
