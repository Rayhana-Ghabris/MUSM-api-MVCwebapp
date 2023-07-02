using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MUSM_api_MVCwebapp.Models;
using System;

namespace MUSM_api_MVCwebapp.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Initialize Roles

            builder.Entity<IdentityRole>().HasData(
                new { Id = "1", Name = "Manager", NormalizedName = "MANAGER" },
                new { Id = "2", Name = "Worker", NormalizedName = "WORKER" },
                new { Id = "3", Name = "PublicUser", NormalizedName = "PUBLICUSER" }
                );

            // Initialize the Manager with credentials: Email: manager@mu.edu.lb, Password: Manager@123

            var managerId = Guid.NewGuid().ToString();

            builder.Entity<AppUser>().HasData(
                new AppUser
                {
                    Id = managerId,
                    FullName = "Manager",
                    UserName = "manager@mu.edu.lb",
                    NormalizedUserName = "MANAGER@MU.EDU.LB",
                    Email = "manager@mu.edu.lb",
                    NormalizedEmail = "MANAGER@MU.EDU.LB",
                    PhoneNumber = "1234567890",
                    PhoneNumberConfirmed = true,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Deleted = false,
                    CreatedAt = DateTime.Now,
                    TwoFactorEnabled = false,
                    PasswordHash = "AQAAAAEAACcQAAAAEC6rK5TRHGeXrJ1LtZFyKXDq+6XCQmxvYV0BDp2P10JESYdtA/EUBHqT3WXouCLDCA=="
                }
            );

            // Assign the User to Manager Role

            builder.Entity<IdentityUserRole<string>>().HasData(
                    new { UserId = managerId, RoleId = "1" }
                );

            //One-To-Many relation between

            builder.Entity<RequestModel>()
                .HasOne(h => h.PublicUser)
                .WithMany(b => b.Requests)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TaskModel>()
                .HasOne(h => h.Worker)
                .WithMany(b => b.Tasks)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TaskModel>()
                .HasOne(e => e.Request)
                .WithOne(e => e.Task)
                .OnDelete(DeleteBehavior.Restrict);

           



            //Many-To-Many table "VoteModel"
            builder.Entity<VoteModel>()
               .HasKey(r => new { r.RequestId, r.PublicUserId });

        }

        public DbSet<RequestModel> Requests { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<VoteModel> Votes { get; set; }

        public DbSet<Picture> Pictures { get; set; }


    }
}
