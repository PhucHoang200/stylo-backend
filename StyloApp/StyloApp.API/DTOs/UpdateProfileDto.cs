namespace StyloApp.API.DTOs
{
    public class UpdateProfileDto
    {
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
    }
}
