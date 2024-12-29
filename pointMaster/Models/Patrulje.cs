namespace pointMaster.Models
{
    public class Patrulje
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PatruljeMedlem> PatruljeMedlems { get; set; }
        public List<Point> Points { get; set; }
        public DateTime? DateCreated { get; set; }
    }

    public class PatruljeMedlem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public Patrulje Patrulje { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
