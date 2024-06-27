using System.ComponentModel.DataAnnotations;

namespace OutOfOffice.Models
{
    public class Employee
    {
        [Key]
        public int ID { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Required]
        public string Subdivision { get; set; }

        [Required]
        public string Position { get; set; }

        [Required]
        public string Status { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int OutOfOfficeBalance { get; set; }

        public byte[]? Photo { get; set; }

        public int PeoplePartner { get; set; }

        public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();
    }
}
