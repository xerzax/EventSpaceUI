using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using static EventSpaceUI.Client.Pages.Todo.Todo;

namespace EventSpaceUI.Client.Pages.Post
{
    public partial class BlogIndex
    {
        private List<Blog> blogs;
        private Blog newBlog = new Blog();
        private string imageUrl = "";

        public class Blog
        {
            public string? Title { get; set; }
            public string Content { get; set; }

            public string? PhotoName { get; set; }
        }

        protected override async Task OnInitializedAsync()

        {
            var endpoint = "Blog/GetAllBlogs";
            blogs = await _apiService.CallApiAsync<List<Blog>>(endpoint, HttpMethod.Get);
        }

		private async Task CreateBlog()
		{
			var endpoint = "Blog/PostBlogs";
			//var formData = new MultipartFormDataContent();

			//formData.Add(new StringContent(newBlog.Title), nameof(newBlog.Title));
			//formData.Add(new StringContent(newBlog.Content), nameof(newBlog.Content));
			//formData.Add(new StringContent(newBlog.PhotoName), nameof(newBlog.PhotoName));

			//blogs = await _apiService.CallApiAsync<List<Blog>>(endpoint, HttpMethod.Post, formData);

            await _apiService.CallApiAsync<Blog>("Blog/PostBlogs", HttpMethod.Post, newBlog);
        }


        private async Task OnInputFileChange(InputFileChangeEventArgs e)
		{
            using var formData = new MultipartFormDataContent();

            var file = e.File;

            var fileContent = new StreamContent(file.OpenReadStream(long.MaxValue));

            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            formData.Add(content: fileContent, name: "File", fileName: file.Name);

            formData.Add(new StringContent("1"), "FilePath");

            var response = await Http.PostAsync("https://localhost:7060/api/File/upload", formData);

            if (response.IsSuccessStatusCode)
            {
                var uploadedResult = await response.Content.ReadAsStringAsync();

                newBlog.PhotoName = uploadedResult ?? "";
            }
		}
	}
}