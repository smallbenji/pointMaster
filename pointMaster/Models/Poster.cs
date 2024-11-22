namespace pointMaster.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime? DateCreated { get; set; }
    }
}
