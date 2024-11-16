namespace pointMaster.Models
{
    public class Patrulje
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<PatruljeMedlem> PatruljeMedlems { get; set; } = null!;
        public List<Point> Points { get; set; } = null!;
    }

    public class PatruljeMedlem
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public Patrulje Patrulje { get; set; } = null!;
    }
}
