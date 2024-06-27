using System.ComponentModel.DataAnnotations;

namespace OutOfOffice.Models
{
    public class Project
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string ProjectType { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int ProjectManagerId { get; set; }

        public string Comment { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Active";

        public Employee ProjectManager { get; set; }

        public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();
    }
}
