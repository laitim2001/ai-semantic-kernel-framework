using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIAgentPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPluginEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plugins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PluginId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Category = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Metadata = table.Column<string>(type: "jsonb", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AssemblyPath = table.Column<string>(type: "text", nullable: true),
                    AssemblyFullName = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plugins", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Plugins_Category",
                table: "Plugins",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Plugins_CreatedAt",
                table: "Plugins",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Plugins_PluginId",
                table: "Plugins",
                column: "PluginId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plugins_UserId",
                table: "Plugins",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Plugins");
        }
    }
}
