using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUSM_api_MVCwebapp.Migrations
{
    public partial class newmigration : Migration
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

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Deleted", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Speciality", "TwoFactorEnabled", "UserName" },
                values: new object[] { "8672299f-ad19-40f8-9c25-5d5e840dc1f2", 0, "a2790b84-d707-4199-99e1-8e9ae408d36f", new DateTime(2023, 7, 3, 21, 40, 54, 102, DateTimeKind.Local).AddTicks(5033), false, "manager@mu.edu.lb", true, "Manager", false, null, "MANAGER@MU.EDU.LB", "MANAGER@MU.EDU.LB", "AQAAAAEAACcQAAAAEC6rK5TRHGeXrJ1LtZFyKXDq+6XCQmxvYV0BDp2P10JESYdtA/EUBHqT3WXouCLDCA==", "1234567890", true, "b75e554b-5514-46bd-9438-08136739acbb", null, false, "manager@mu.edu.lb" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "8672299f-ad19-40f8-9c25-5d5e840dc1f2" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "8672299f-ad19-40f8-9c25-5d5e840dc1f2" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8672299f-ad19-40f8-9c25-5d5e840dc1f2");

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
