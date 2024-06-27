using System.ComponentModel.DataAnnotations;

namespace OutOfOffice.Models
{
    public class LeaveRequest
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public string AbsenceReason { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public string Comment { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "New";

        public Employee Employee { get; set; }
    }
}