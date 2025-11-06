using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIAgentPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhase2To4Entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "agent_executions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    conversation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    response_time_ms = table.Column<double>(type: "double precision", precision: 18, scale: 2, nullable: true),
                    tokens_used = table.Column<int>(type: "integer", nullable: true),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    metadata = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agent_executions", x => x.id);
                    table.ForeignKey(
                        name: "FK_agent_executions_Conversations_conversation_id",
                        column: x => x.conversation_id,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_agent_executions_agents_agent_id",
                        column: x => x.agent_id,
                        principalTable: "agents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "agent_versions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    change_description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    change_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    configuration_snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_current = table.Column<bool>(type: "boolean", nullable: false),
                    rolled_back_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rolled_back_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agent_versions", x => x.id);
                    table.ForeignKey(
                        name: "FK_agent_versions_agents_agent_id",
                        column: x => x.agent_id,
                        principalTable: "agents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plugins",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    configuration = table.Column<string>(type: "jsonb", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    author = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plugins", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "agent_plugins",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    plugin_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    execution_order = table.Column<int>(type: "integer", nullable: false),
                    custom_configuration = table.Column<string>(type: "jsonb", nullable: true),
                    added_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    added_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agent_plugins", x => x.id);
                    table.ForeignKey(
                        name: "FK_agent_plugins_agents_agent_id",
                        column: x => x.agent_id,
                        principalTable: "agents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_agent_plugins_plugins_plugin_id",
                        column: x => x.plugin_id,
                        principalTable: "plugins",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_agent_executions_agent_id",
                table: "agent_executions",
                column: "agent_id");

            migrationBuilder.CreateIndex(
                name: "ix_agent_executions_conversation_id",
                table: "agent_executions",
                column: "conversation_id");

            migrationBuilder.CreateIndex(
                name: "ix_agent_executions_start_time",
                table: "agent_executions",
                column: "start_time");

            migrationBuilder.CreateIndex(
                name: "ix_agent_executions_status",
                table: "agent_executions",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_agent_plugins_agent_id",
                table: "agent_plugins",
                column: "agent_id");

            migrationBuilder.CreateIndex(
                name: "ix_agent_plugins_agent_plugin",
                table: "agent_plugins",
                columns: new[] { "agent_id", "plugin_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_agent_plugins_execution_order",
                table: "agent_plugins",
                column: "execution_order");

            migrationBuilder.CreateIndex(
                name: "ix_agent_plugins_plugin_id",
                table: "agent_plugins",
                column: "plugin_id");

            migrationBuilder.CreateIndex(
                name: "ix_agent_versions_agent_id",
                table: "agent_versions",
                column: "agent_id");

            migrationBuilder.CreateIndex(
                name: "ix_agent_versions_agent_version",
                table: "agent_versions",
                columns: new[] { "agent_id", "version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_agent_versions_created_at",
                table: "agent_versions",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_agent_versions_is_current",
                table: "agent_versions",
                column: "is_current");

            migrationBuilder.CreateIndex(
                name: "ix_plugins_is_enabled",
                table: "plugins",
                column: "is_enabled");

            migrationBuilder.CreateIndex(
                name: "ix_plugins_name",
                table: "plugins",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_plugins_type",
                table: "plugins",
                column: "type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agent_executions");

            migrationBuilder.DropTable(
                name: "agent_plugins");

            migrationBuilder.DropTable(
                name: "agent_versions");

            migrationBuilder.DropTable(
                name: "plugins");
        }
    }
}
