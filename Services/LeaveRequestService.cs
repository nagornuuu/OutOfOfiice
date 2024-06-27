using Microsoft.EntityFrameworkCore;
using OutOfOffice.Data;
using OutOfOffice.Models;

namespace OutOfOffice.Services
{
    public class LeaveRequestService
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LeaveRequest> GetLeaveRequestAsync(int id)
        {
            return await _context.LeaveRequests
                .Include(lr => lr.Employee)
                .FirstOrDefaultAsync(lr => lr.ID == id);
        }

        public async Task<List<LeaveRequest>> GetLeaveRequestsAsync()
        {
            return await _context.LeaveRequests
                .Include(lr => lr.Employee)
                .ToListAsync();
        }

        public async Task AddLeaveRequestAsync(LeaveRequest leaveRequest)
        {
            if (!await EmployeeExists(leaveRequest.EmployeeId))
            {
                throw new InvalidOperationException("Invalid EmployeeId");
            }

            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLeaveRequestAsync(LeaveRequest leaveRequest)
        {
            if (!await EmployeeExists(leaveRequest.EmployeeId))
            {
                throw new InvalidOperationException("Invalid EmployeeId");
            }

            _context.LeaveRequests.Update(leaveRequest);
            await _context.SaveChangesAsync();
        }

        public async Task SubmitLeaveRequestAsync(int id)
        {
            var leaveRequest = await GetLeaveRequestAsync(id);
            if (leaveRequest != null)
            {
                leaveRequest.Status = "Submitted";

                var hrManagers = await _context.Employees
                    .Where(e => e.Position == "HR Manager")
                    .ToListAsync();

                var projectManagers = await _context.EmployeeProjects
                    .Where(ep => ep.EmployeeId == leaveRequest.EmployeeId)
                    .Select(ep => ep.Project.ProjectManager)
                    .Distinct()
                    .ToListAsync();

                var approvalRequests = hrManagers
                    .Concat(projectManagers)
                    .Distinct()
                    .Select(manager => new ApprovalRequest
                    {
                        ApproverId = manager.ID,
                        LeaveRequestId = leaveRequest.ID,
                        Status = "Pending"
                    });

                _context.ApprovalRequests.AddRange(approvalRequests);

                await _context.SaveChangesAsync();
            }
        }

        public async Task CancelLeaveRequestAsync(int id)
        {
            var leaveRequest = await GetLeaveRequestAsync(id);
            if (leaveRequest != null)
            {
                leaveRequest.Status = "Canceled";

                var approvalRequests = await _context.ApprovalRequests
                    .Where(ar => ar.LeaveRequestId == id)
                    .ToListAsync();

                approvalRequests.ForEach(ar => ar.Status = "Canceled");

                await _context.SaveChangesAsync();
            }
        }

        private async Task<bool> EmployeeExists(int employeeId)
        {
            return await _context.Employees.AnyAsync(e => e.ID == employeeId);
        }
    }
}
