namespace BasicStructure.Models
{
    public class Field
    {
        public int Id { get; set; }
        public string FieldName { get; set; }
        public bool Status { get; set; }
        public string GrowerName { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Coordinate> Coordinates { get; set; }

    }
}
