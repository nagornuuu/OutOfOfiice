using Microsoft.EntityFrameworkCore;
using OutOfOffice.Data;
using OutOfOffice.Models;

namespace OutOfOffice.Services
{
    public class EmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<string>> GetSubdivisionsAsync()
        {
            // need to be replaced this with the actual logic to get subdivisions
            return await Task.FromResult(new List<string> { "IT", "HR", "Finance", "Marketing" });
        }

        public async Task<List<string>> GetPositionsAsync()
        {
            // need to be replace this with the actual logic to get positions
            return await Task.FromResult(new List<string> { "Developer", "HR Manager", "Accountant", "Marketing Specialist" });
        }

        public async Task<List<Employee>> GetHRManagersAsync()
        {
            return await _context.Employees.Where(e => e.Position == "HR Manager").ToListAsync();
        }

        public async Task AssignEmployeeToProject(EmployeeProject assignment)
        {
            var employeeProject = new EmployeeProject
            {
                EmployeeId = assignment.EmployeeId,
                ProjectId = assignment.ProjectId
            };

            _context.EmployeeProjects.Add(employeeProject);
            await _context.SaveChangesAsync();
        }
    }
}