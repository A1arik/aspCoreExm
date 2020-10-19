using System;
using System.Collections.Generic;
using System.Text;
using exam.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace exam.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<TType> TTypes { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<ClientTraining> ClientsTrainings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ClientTraining>()
             .HasKey(k => new { k.ClientId, k.TrainingId });


            modelBuilder.Entity<ClientTraining>()
             .HasOne(ac => ac.Client)
             .WithMany(c => c.Trainings)
             .HasForeignKey(sc => sc.ClientId);


            modelBuilder.Entity<ClientTraining>()
                .HasOne(ac => ac.Training)
                .WithMany(c => c.Clients)
                .HasForeignKey(sc => sc.TrainingId);
        }

    }
}
