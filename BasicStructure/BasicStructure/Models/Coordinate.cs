namespace BasicStructure.Models
{
    public class Coordinate
    {
        public int Id { get; set; }
        public int SequenceNumber { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int? FieldId { get; set; }
        public Field Field { get; set; }

    }
}
