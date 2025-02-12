﻿namespace pointMaster.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public bool BlockPoint { get; set; }
        public bool BlockTurnout { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
