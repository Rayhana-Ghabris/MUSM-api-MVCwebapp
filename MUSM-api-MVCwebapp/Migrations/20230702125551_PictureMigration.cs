using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUSM_api_MVCwebapp.Migrations
{
    public partial class PictureMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "6176fbe8-1ae1-43ae-b924-ff2470a27164" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6176fbe8-1ae1-43ae-b924-ff2470a27164");

            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Requests");

            migrationBuilder.AddColumn<int>(
                name: "PhotoId",
                table: "Requests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Pictures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pictures", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Deleted", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Speciality", "TwoFactorEnabled", "UserName" },
                values: new object[] { "fa60bdf1-cfac-458e-a84e-544fdd223fca", 0, "d13c8b2c-f330-454c-8053-f677bc097161", new DateTime(2023, 7, 2, 15, 55, 50, 712, DateTimeKind.Local).AddTicks(720), false, "manager@mu.edu.lb", true, "Manager", false, null, "MANAGER@MU.EDU.LB", "MANAGER@MU.EDU.LB", "AQAAAAEAACcQAAAAEC6rK5TRHGeXrJ1LtZFyKXDq+6XCQmxvYV0BDp2P10JESYdtA/EUBHqT3WXouCLDCA==", "1234567890", true, "7f018358-5eaf-4a15-ba0c-a6a42c0844b2", null, false, "manager@mu.edu.lb" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "fa60bdf1-cfac-458e-a84e-544fdd223fca" });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_PhotoId",
                table: "Requests",
                column: "PhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Pictures_PhotoId",
                table: "Requests",
                column: "PhotoId",
                principalTable: "Pictures",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Pictures_PhotoId",
                table: "Requests");

            migrationBuilder.DropTable(
                name: "Pictures");

            migrationBuilder.DropIndex(
                name: "IX_Requests_PhotoId",
                table: "Requests");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "fa60bdf1-cfac-458e-a84e-544fdd223fca" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fa60bdf1-cfac-458e-a84e-544fdd223fca");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Requests");

            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Deleted", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Speciality", "TwoFactorEnabled", "UserName" },
                values: new object[] { "6176fbe8-1ae1-43ae-b924-ff2470a27164", 0, "dda74664-2a65-45aa-8c3b-34a1239fa84f", new DateTime(2023, 6, 25, 19, 43, 25, 715, DateTimeKind.Local).AddTicks(1303), false, "manager@mu.edu.lb", true, "Manager", false, null, "MANAGER@MU.EDU.LB", "MANAGER@MU.EDU.LB", "AQAAAAEAACcQAAAAEC6rK5TRHGeXrJ1LtZFyKXDq+6XCQmxvYV0BDp2P10JESYdtA/EUBHqT3WXouCLDCA==", "1234567890", true, "5adf3fc8-95cb-4741-a630-e03da8cb237b", null, false, "manager@mu.edu.lb" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "6176fbe8-1ae1-43ae-b924-ff2470a27164" });
        }
    }
}
