using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DriveFromOutsideServer.DB
{
    public partial class AssignmentContext : DbContext
    {
        public AssignmentContext() { }

        public AssignmentContext(DbContextOptions<AssignmentContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<EmperorAssignment> Emperors { get; set; }
        public DbSet<KingAssignment> Kings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmperorAssignment>(entity =>
            {
                entity.ToTable("emperors");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasConversion<string>()
                    .HasColumnType("TEXT")
                    .HasMaxLength(10);
                entity.Property(e => e.IssueTime).HasColumnName("issue_time");
                entity.Property(e => e.Config)
                    .HasColumnName("config");
                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasConversion<string>()
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<KingAssignment>(entity =>
            {
                entity.ToTable("kings");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.EmperorId, "IX_kings_EmperorId");
                entity.HasOne(e => e.Emperor)
                    .WithMany(e => e.Kings)
                    .HasForeignKey(e => e.EmperorId);

                //entity.HasOne(e => e.King)
                //    .WithMany()
                //    .HasForeignKey(e => e.KingId);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Version).HasColumnName("version");
                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasConversion<string>()
                    .HasMaxLength(10);
                entity.Property(e => e.IssueTime).HasColumnName("issue_time");
                entity.Property(e => e.Config)
                    .HasColumnName("config");
                entity.Property(e => e.OpenTime).HasColumnName("open_time");
                entity.Property(e => e.CloseTime).HasColumnName("close_time");
                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasConversion<string>()
                    .HasMaxLength(10);
                entity.Property(e => e.EmperorId).HasColumnName("emperor_id");
                //entity.Property(e => e.KingId).HasColumnName("king_id");
            });
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}