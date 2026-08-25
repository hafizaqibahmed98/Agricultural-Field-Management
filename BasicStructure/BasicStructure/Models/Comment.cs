namespace BasicStructure.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? FieldId { get; set; }
        public Field Field { get; set; }
        public int? UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}
