using GeradorDeCertificados.Dominio.Modulos.Certificados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorDeCertificados.Infraestrutura.Modulos.Certificados;

public class GeradorConfiguration : IEntityTypeConfiguration<Gerador>
{
    public void Configure(EntityTypeBuilder<Gerador> builder)
    {
        builder.ToTable("TBGeradores");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(g => g.CaminhoZip)
            .HasMaxLength(500);

        builder.HasMany(g => g.Certificados)
            .WithOne()
            .HasForeignKey(c => c.GeradorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Dominio.Modulos.Cursos.Curso>()
            .WithMany()
            .HasForeignKey(g => g.CursoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
