using Comun.Models;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace WsApiexamen.Models
{
    public partial class BdiExamenContext : DbContext
    {
        public BdiExamenContext()
        {
        }

        public BdiExamenContext(DbContextOptions<BdiExamenContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Examen> Examenes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Modern_Spanish_CI_AS");

            modelBuilder.Entity<Examen>(entity =>
            {
                entity.HasKey(e => e.IdExamen);

                entity.ToTable("tblExamen");

                entity.Property(e => e.IdExamen).HasColumnName("idExamen");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
