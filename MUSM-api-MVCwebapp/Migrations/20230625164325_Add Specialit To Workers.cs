using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUSM_api_MVCwebapp.Migrations
{
    public partial class AddSpecialitToWorkers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "7005685e-515b-441f-ab72-355724c1c40a" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7005685e-515b-441f-ab72-355724c1c40a");

            migrationBuilder.AddColumn<string>(
                name: "Speciality",
                table: "AspNetUsers",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "Speciality",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Deleted", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "7005685e-515b-441f-ab72-355724c1c40a", 0, "dc86df47-94bb-476a-894d-ca25e14c9813", new DateTime(2023, 6, 25, 10, 14, 31, 622, DateTimeKind.Local).AddTicks(4589), false, "manager@mu.edu.lb", true, "Manager", false, null, "MANAGER@MU.EDU.LB", "MANAGER@MU.EDU.LB", "AQAAAAEAACcQAAAAEC6rK5TRHGeXrJ1LtZFyKXDq+6XCQmxvYV0BDp2P10JESYdtA/EUBHqT3WXouCLDCA==", "1234567890", true, "df5d5050-1d62-41ac-b174-3232131655ef", false, "manager@mu.edu.lb" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "7005685e-515b-441f-ab72-355724c1c40a" });
        }
    }
}
