using OutOfOffice.Models;
using Microsoft.AspNetCore.Identity;

namespace OutOfOffice.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedData>>();

                var roles = new[] { "HRManager", "ProjectManager", "Employee" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                        if (roleResult.Succeeded)
                        {
                            logger.LogInformation($"Role {role} created successfully.");
                        }
                        else
                        {
                            logger.LogError($"Error creating role {role}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                        }
                    }
                }

                var users = new[]
                {
                    new { Email = "hrmanager@gmail.com", Role = "HRManager" },
                    new { Email = "projectmanager@gmail.com", Role = "ProjectManager" },
                    new { Email = "employee1@gmail.com", Role = "Employee" },
                    new { Email = "employee2@gmail.com", Role = "Employee" }
                };

                foreach (var user in users)
                {
                    if (await userManager.FindByEmailAsync(user.Email) == null)
                    {
                        var newUser = new IdentityUser { UserName = user.Email, Email = user.Email, EmailConfirmed = true };
                        var result = await userManager.CreateAsync(newUser, "Password123!");

                        if (result.Succeeded)
                        {
                            var roleResult = await userManager.AddToRoleAsync(newUser, user.Role);
                            if (roleResult.Succeeded)
                            {
                                logger.LogInformation($"User {user.Email} created successfully and added to role {user.Role}.");
                            }
                            else
                            {
                                logger.LogError($"Error adding user {user.Email} to role {user.Role}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                            }
                        }
                        else
                        {
                            logger.LogError($"Error creating user {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        }
                    }
                }

                if (!context.Employees.Any())
                {
                    var johnDoe = new Employee { FullName = "John Doe", Subdivision = "IT", Position = "Developer", Status = "Active", OutOfOfficeBalance = 10, Photo = null };
                    var janeSmith = new Employee { FullName = "Jane Smith", Subdivision = "HR", Position = "HR Manager", Status = "Active", OutOfOfficeBalance = 15, Photo = null };
                    var mikeJohnson = new Employee { FullName = "Mike Johnson", Subdivision = "Finance", Position = "Accountant", Status = "Active", OutOfOfficeBalance = 20, Photo = null };

                    context.Employees.AddRange(johnDoe, janeSmith, mikeJohnson);
                    await context.SaveChangesAsync();

                    johnDoe.PeoplePartner = janeSmith.ID;
                    janeSmith.PeoplePartner = johnDoe.ID;

                    context.Update(johnDoe);
                    context.Update(janeSmith);
                    await context.SaveChangesAsync();
                }

                if (!context.LeaveRequests.Any())
                {
                    var employee1 = context.Employees.First(e => e.FullName == "John Doe");
                    var employee2 = context.Employees.First(e => e.FullName == "Jane Smith");
                    var employee3 = context.Employees.First(e => e.FullName == "Mike Johnson");

                    context.LeaveRequests.AddRange(
                        new LeaveRequest { EmployeeId = employee1.ID, AbsenceReason = "Vacation", StartDate = DateTime.UtcNow.AddDays(1), EndDate = DateTime.UtcNow.AddDays(5), Status = "Pending" },
                        new LeaveRequest { EmployeeId = employee2.ID, AbsenceReason = "Sick Leave", StartDate = DateTime.UtcNow.AddDays(1), EndDate = DateTime.UtcNow.AddDays(3), Status = "Pending" },
                        new LeaveRequest { EmployeeId = employee3.ID, AbsenceReason = "Personal", StartDate = DateTime.UtcNow.AddDays(2), EndDate = DateTime.UtcNow.AddDays(4), Status = "Pending" },
                        new LeaveRequest { EmployeeId = employee1.ID, AbsenceReason = "Conference", StartDate = DateTime.UtcNow.AddDays(10), EndDate = DateTime.UtcNow.AddDays(12), Status = "Pending" },
                        new LeaveRequest { EmployeeId = employee2.ID, AbsenceReason = "Family Event", StartDate = DateTime.UtcNow.AddDays(15), EndDate = DateTime.UtcNow.AddDays(20), Status = "Pending" }
                    );

                    await context.SaveChangesAsync();
                }

                if (!context.ApprovalRequests.Any())
                {
                    var janeSmith = context.Employees.First(e => e.FullName == "Jane Smith");
                    var johnDoe = context.Employees.First(e => e.FullName == "John Doe");
                    var mikeJohnson = context.Employees.First(e => e.FullName == "Mike Johnson");
                    var leaveRequest1 = context.LeaveRequests.First(lr => lr.AbsenceReason == "Vacation");
                    var leaveRequest2 = context.LeaveRequests.First(lr => lr.AbsenceReason == "Sick Leave");
                    var leaveRequest3 = context.LeaveRequests.First(lr => lr.AbsenceReason == "Personal");
                    var leaveRequest4 = context.LeaveRequests.First(lr => lr.AbsenceReason == "Conference");
                    var leaveRequest5 = context.LeaveRequests.First(lr => lr.AbsenceReason == "Family Event");

                    context.ApprovalRequests.AddRange(
                        new ApprovalRequest { ApproverId = janeSmith.ID, LeaveRequestId = leaveRequest1.ID, Status = "Pending", Comment = "Needs approval" },
                        new ApprovalRequest { ApproverId = johnDoe.ID, LeaveRequestId = leaveRequest2.ID, Status = "Pending", Comment = "Needs approval" },
                        new ApprovalRequest { ApproverId = janeSmith.ID, LeaveRequestId = leaveRequest3.ID, Status = "Pending", Comment = "Needs approval" },
                        new ApprovalRequest { ApproverId = mikeJohnson.ID, LeaveRequestId = leaveRequest4.ID, Status = "Pending", Comment = "Conference leave" },
                        new ApprovalRequest { ApproverId = johnDoe.ID, LeaveRequestId = leaveRequest5.ID, Status = "Pending", Comment = "Family event leave" }
                    );

                    await context.SaveChangesAsync();
                }

                if (!context.Projects.Any())
                {
                    var johnDoe = context.Employees.First(e => e.FullName == "John Doe");
                    var janeSmith = context.Employees.First(e => e.FullName == "Jane Smith");

                    context.Projects.AddRange(
                        new Project { ProjectType = "Development", StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddDays(30), ProjectManagerId = johnDoe.ID, Comment = "Important project", Status = "Active" },
                        new Project { ProjectType = "Research", StartDate = DateTime.UtcNow.AddDays(-20), EndDate = DateTime.UtcNow.AddDays(10), ProjectManagerId = janeSmith.ID, Comment = "Research project", Status = "Inactive" }
                    );

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}