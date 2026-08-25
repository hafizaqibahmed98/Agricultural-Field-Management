namespace BasicStructure.Models
{
    public class ResetPassword
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string email { get; set; }
        public string token { get; set; }
    }
}
