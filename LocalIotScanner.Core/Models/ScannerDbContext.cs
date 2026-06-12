using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocalIotScanner.Core.Models
{
    public class ScannerDbContext : DbContext
    {
        public ScannerDbContext(DbContextOptions<ScannerDbContext> options) : base(options) { }

        public DbSet<AuditSession> AuditSessions { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<ScannedPort> ScannedPorts { get; set; }
        public DbSet<Vulnerability> Vulnerabilities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Fluent API configuration for relationships
            modelBuilder.Entity<AuditSession>()
                .HasMany(a => a.Devices)
                .WithOne(d => d.AuditSession)
                .HasForeignKey(d => d.AuditSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
