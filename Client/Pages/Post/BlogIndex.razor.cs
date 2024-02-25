using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;

namespace EventSpaceUI.Client.Pages.Post
{
    public partial class BlogIndex
    {
        private List<Blog> blogs;
        private Blog newBlog = new Blog();


        public class Blog
        {
            public string? Title { get; set; }
            public string Content { get; set; }

            public string? PhotoName { get; set; }
            public IBrowserFile File { get; set; }

        }

        protected override async Task OnInitializedAsync()

        {
            var endpoint = "Blog/GetAllBlogs";
            blogs = await _apiService.CallApiAsync<List<Blog>>(endpoint, HttpMethod.Get);
        }

        //private async Task CreateBlog()
        //{
        //    var endpoint = "Blog/PostBlogs";
        //    var formData = new MultipartFormDataContent();

        //    formData.Add(new StringContent(newBlog.Title), nameof(newBlog.Title));
        //    formData.Add(new StringContent(newBlog.Content), nameof(newBlog.Content));
        //    formData.Add(new StringContent(newBlog.PhotoName), nameof(newBlog.PhotoName));

        //    if (newBlog.File != null)
        //    {
        //        const long maxAllowedSize = 10 * 1024 * 1024;
        //        var fileStream = new MemoryStream();
        //        //await newBlog.File.OpenReadStream().CopyToAsync(fileStream);
        //        await newBlog.File.OpenReadStream(maxAllowedSize).CopyToAsync(fileStream);
        //        fileStream.Seek(0, SeekOrigin.Begin);

        //        var streamContent = new StreamContent(fileStream);
        //        streamContent.Headers.ContentType = new MediaTypeHeaderValue(newBlog.File.ContentType);

        //        formData.Add(streamContent, "file", newBlog.File.Name);
        //    }


        //    blogs = await _apiService.CallApiAsync<List<Blog>>(endpoint, HttpMethod.Post, formData);
        //}
        private async Task CreateBlog()
        {
            var endpoint = "Blog/PostBlogs";

            try
            {
                var formData = new MultipartFormDataContent();

                // Add the string content fields
                formData.Add(new StringContent(newBlog.Title), "Title");
                formData.Add(new StringContent(newBlog.Content), "Content");
                formData.Add(new StringContent(newBlog.PhotoName), "PhotoName");

                if (newBlog.File != null)
                {
                    const long maxAllowedSize = 10 * 1024 * 1024;

                    var fileStream = new MemoryStream();
                    await newBlog.File.OpenReadStream(maxAllowedSize).CopyToAsync(fileStream);
                    fileStream.Seek(0, SeekOrigin.Begin);

                    using (fileStream)
                    {
                        var streamContent = new StreamContent(fileStream);
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue(newBlog.File.ContentType);

                        formData.Add(streamContent, "file", newBlog.File.Name);
                    }
                }


                // Call the API service to send the request
                blogs = await _apiService.CallApiAsyncForm<List<Blog>>(endpoint, HttpMethod.Post, formData);
            }
            catch (Exception ex)
            {
                // Handle any exceptions here, such as logging or displaying an error message
                Console.WriteLine($"An error occurred while creating the blog: {ex.Message}");
                // You might want to rethrow the exception here if you want to propagate it further
                throw;
            }
        }

        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            var file = e.File;
            newBlog.File = file;
        }


    }
}