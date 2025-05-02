using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyWeathersApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAliasToFavoriteCity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alias",
                table: "FavoriteCities",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alias",
                table: "FavoriteCities");
        }
    }
}
