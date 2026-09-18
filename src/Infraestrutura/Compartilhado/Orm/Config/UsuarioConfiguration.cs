using GeradorDeCertificados.Dominio.Compartilhado.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorDeCertificados.Infraestrutura.Compartilhado.Orm.Config;

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("TBUsuario");

        builder.HasKey(u => u.UsuarioId)
            .HasName("PK_TBUsuario");

        builder.Property(u => u.UsuarioId)
            .ValueGeneratedNever();

        builder.Property(u => u.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne<IdentityUser<Guid>>()
            .WithOne()
            .HasForeignKey<Usuario>(u => u.UsuarioId)
            .HasConstraintName("FK_TBInstituicao_AspNetUsers")
            .OnDelete(DeleteBehavior.Restrict);
    }
}