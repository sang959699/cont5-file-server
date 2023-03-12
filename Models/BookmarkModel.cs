namespace Cont5.Models.Bookmark {
    public class AddBookmarkRequest {
        public string Path { get; set; }
    }
    public class AddBookmarkModel {
        public bool Result { get; set; }
    }
    public class DeleteBookmarkRequest {
        public string Path { get; set; }
    }
    public class DeleteBookmarkModel {
        public bool Result { get; set; }
    }
    public class GetBookmarkModel {
        public string Name { get; set; }
        public string Path { get; set; }
    }
}