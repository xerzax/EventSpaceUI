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
        }

        protected override async Task OnInitializedAsync()

        {
            var endpoint = "Blog/GetAllBlogs";
            blogs = await _apiService.CallApiAsync<List<Blog>>(endpoint, HttpMethod.Get);
        }

        private async Task CreateBlog()
        {
            var endpoint = "Blog/PostBlogs";
            blogs = await _apiService.CallApiAsync<List<Blog>>(endpoint, HttpMethod.Post, newBlog);
        }
    }
}