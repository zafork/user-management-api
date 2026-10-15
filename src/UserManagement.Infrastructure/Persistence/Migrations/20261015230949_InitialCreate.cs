using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PERMISSIONS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CODE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PERMISSIONS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ROLES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROLES", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: false),
                    FULL_NAME = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    PASSWORD_HASH = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    IS_ACTIVE = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ROLE_PERMISSIONS",
                columns: table => new
                {
                    ROLE_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PERMISSION_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROLE_PERMISSIONS", x => new { x.ROLE_ID, x.PERMISSION_ID });
                    table.ForeignKey(
                        name: "FK_ROLE_PERMISSIONS_PERMISSIONS_PERMISSION_ID",
                        column: x => x.PERMISSION_ID,
                        principalTable: "PERMISSIONS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ROLE_PERMISSIONS_ROLES_ROLE_ID",
                        column: x => x.ROLE_ID,
                        principalTable: "ROLES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USER_ROLES",
                columns: table => new
                {
                    USER_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ROLE_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_ROLES", x => new { x.USER_ID, x.ROLE_ID });
                    table.ForeignKey(
                        name: "FK_USER_ROLES_ROLES_ROLE_ID",
                        column: x => x.ROLE_ID,
                        principalTable: "ROLES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_USER_ROLES_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PERMISSIONS",
                columns: new[] { "ID", "CODE" },
                values: new object[,]
                {
                    { 1, "users.read" },
                    { 2, "users.update" }
                });

            migrationBuilder.InsertData(
                table: "ROLES",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { 1, "Administrador" },
                    { 2, "Consulta" }
                });

            migrationBuilder.InsertData(
                table: "USERS",
                columns: new[] { "ID", "CREATED_AT", "EMAIL", "FULL_NAME", "IS_ACTIVE", "PASSWORD_HASH", "UPDATED_AT" },
                values: new object[,]
                {
                    { new Guid("6eeb52d3-15a8-4c2e-9850-0afdeab1bef2"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "inactivo@demo.com", "Usuario Inactivo", false, "$2a$12$AyQZJv17xMcLJk2TNpNODe1V6e2EX3CfEglm06v4AyLt9FqHolYTK", null },
                    { new Guid("ba435598-dded-4609-a1fa-47760ebd17a5"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@demo.com", "Administrador", true, "$2a$12$VklczTU7tSV9kXvo/xhTv.Xf62B9c05upBoW3Y2XVItFDyTRK78e6", null },
                    { new Guid("c65c123f-10e1-4abe-b96a-cad13d8c790c"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "consulta@demo.com", "Usuario Consulta", true, "$2a$12$8V0lKPmSAA/v2VonpQmgwea0/NuEsE2yXJusEWmYsP87mVgromJX.", null }
                });

            migrationBuilder.InsertData(
                table: "ROLE_PERMISSIONS",
                columns: new[] { "PERMISSION_ID", "ROLE_ID" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 1, 2 }
                });

            migrationBuilder.InsertData(
                table: "USER_ROLES",
                columns: new[] { "ROLE_ID", "USER_ID" },
                values: new object[,]
                {
                    { 2, new Guid("6eeb52d3-15a8-4c2e-9850-0afdeab1bef2") },
                    { 1, new Guid("ba435598-dded-4609-a1fa-47760ebd17a5") },
                    { 2, new Guid("c65c123f-10e1-4abe-b96a-cad13d8c790c") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PERMISSIONS_CODE",
                table: "PERMISSIONS",
                column: "CODE",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ROLE_PERMISSIONS_PERMISSION_ID",
                table: "ROLE_PERMISSIONS",
                column: "PERMISSION_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ROLES_NAME",
                table: "ROLES",
                column: "NAME",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USER_ROLES_ROLE_ID",
                table: "USER_ROLES",
                column: "ROLE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_EMAIL",
                table: "USERS",
                column: "EMAIL",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ROLE_PERMISSIONS");

            migrationBuilder.DropTable(
                name: "USER_ROLES");

            migrationBuilder.DropTable(
                name: "PERMISSIONS");

            migrationBuilder.DropTable(
                name: "ROLES");

            migrationBuilder.DropTable(
                name: "USERS");
        }
    }
}
