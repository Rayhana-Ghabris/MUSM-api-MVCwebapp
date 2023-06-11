using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MUSM_api_MVCwebapp.Models;

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
               .HasKey(r => new { r.RequestId, r.PublicUserId});

        }

        public DbSet<RequestModel> Requests { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<VoteModel> Votes { get; set; }


    }
    }
