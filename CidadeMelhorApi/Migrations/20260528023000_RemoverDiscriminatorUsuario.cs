using CidadeMelhorApi.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CidadeMelhorApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260528023000_RemoverDiscriminatorUsuario")]
    public partial class RemoverDiscriminatorUsuario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE Usuarios DROP COLUMN Discriminator;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE Usuarios ADD COLUMN Discriminator TEXT NOT NULL DEFAULT 'Usuario';");
        }
    }
}
