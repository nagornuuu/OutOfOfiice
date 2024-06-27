using Microsoft.EntityFrameworkCore;
using OutOfOffice.Data;
using OutOfOffice.Models;

namespace OutOfOffice.Services
{
    public class ProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Project> GetProjectAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.ProjectManager)
                .FirstOrDefaultAsync(p => p.ID == id);
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            return await _context.Projects
                .Include(p => p.ProjectManager)
                .ToListAsync();
        }

        public async Task AddProjectAsync(Project project)
        {
            ConvertDateTimesToUtc(project);
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProjectAsync(Project project)
        {
            ConvertDateTimesToUtc(project);
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task DeactivateProjectAsync(int id)
        {
            var project = await GetProjectAsync(id);
            if (project != null)
            {
                project.Status = "Inactive";
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Employee>> GetProjectManagersAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        private void ConvertDateTimesToUtc(Project project)
        {
            project.StartDate = DateTime.SpecifyKind(project.StartDate, DateTimeKind.Utc);
            project.EndDate = DateTime.SpecifyKind(project.EndDate, DateTimeKind.Utc);
        }
    }
}
