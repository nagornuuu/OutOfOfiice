using System.ComponentModel.DataAnnotations;

namespace OutOfOffice.Models
{
    public class ApprovalRequest
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int ApproverId { get; set; }
        public Employee Approver { get; set; }

        [Required]
        public int LeaveRequestId { get; set; }
        public LeaveRequest LeaveRequest { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        public string Comment { get; set; } = string.Empty;
    }
}
