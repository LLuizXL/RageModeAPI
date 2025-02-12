namespace RageModeAPI.Models
{
    public class Likes
    {
        public Guid LikesId { get; set; }
        public Guid PostId { get; set; }
        public Post? Post { get; set; }
        public bool IsLike { get; set; }


    }
}
