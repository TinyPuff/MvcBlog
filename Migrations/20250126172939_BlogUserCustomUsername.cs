using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcBlog.Migrations
{
    /// <inheritdoc />
    public partial class BlogUserCustomUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomUsername",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 16,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomUsername",
                table: "AspNetUsers");
        }
    }
}
