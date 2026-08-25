namespace BasicStructure.Models
{
    public class API
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EndPoint { get; set; }
        public bool IsBackend { get; set; }
        public List<PermissionMatrix> PermissionMatrix { get; set; }


    }
}
