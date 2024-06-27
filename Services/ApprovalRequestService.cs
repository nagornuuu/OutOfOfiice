using Microsoft.EntityFrameworkCore;
using OutOfOffice.Data;
using OutOfOffice.Models;

namespace OutOfOffice.Services
{
    public class ApprovalRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly LeaveRequestService _leaveRequestService;

        public ApprovalRequestService(ApplicationDbContext context, LeaveRequestService leaveRequestService)
        {
            _context = context;
            _leaveRequestService = leaveRequestService;
        }

        public async Task<ApprovalRequest> GetApprovalRequestByIdAsync(int id)
        {
            return await _context.ApprovalRequests
                .Include(ar => ar.Approver)
                .Include(ar => ar.LeaveRequest)
                .FirstOrDefaultAsync(ar => ar.ID == id);
        }

        public async Task<List<ApprovalRequest>> GetApprovalRequestsAsync()
        {
            return await _context.ApprovalRequests
                .Include(ar => ar.Approver)
                .Include(ar => ar.LeaveRequest)
                .ToListAsync();
        }

        public async Task ApproveRequestAsync(int id)
        {
            var approvalRequest = await GetApprovalRequestByIdAsync(id);
            if (approvalRequest != null)
            {
                approvalRequest.Status = "Approved";
                _context.Update(approvalRequest);

                // Update related leave request status and adjust absence balance
                approvalRequest.LeaveRequest.Status = "Approved";
                await _leaveRequestService.UpdateLeaveRequestAsync(approvalRequest.LeaveRequest);

                await _context.SaveChangesAsync();
            }
        }

        public async Task RejectRequestAsync(int id, string comment)
        {
            var approvalRequest = await GetApprovalRequestByIdAsync(id);
            if (approvalRequest != null)
            {
                approvalRequest.Status = "Rejected";
                approvalRequest.Comment = comment;
                _context.Update(approvalRequest);

                // Update related leave request status
                approvalRequest.LeaveRequest.Status = "Rejected";
                await _leaveRequestService.UpdateLeaveRequestAsync(approvalRequest.LeaveRequest);

                await _context.SaveChangesAsync();
            }
        }
    }
}
