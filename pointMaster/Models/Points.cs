namespace pointMaster.Models
{
    public class Point
    {
        public int Id { get; set; }
        public int Points { get; set; }
        public int Turnout { get; set; }
        public Patrulje Patrulje { get; set; }
        public Post Poster { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
