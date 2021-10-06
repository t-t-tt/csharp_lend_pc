using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace csharp_lend_pc.Migrations
{
    public partial class CreateLendPc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    FirstNameKana = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    LastNameKana = table.Column<string>(nullable: true),
                    Department = table.Column<string>(nullable: true),
                    TelNumber = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Age = table.Column<int>(nullable: false),
                    Gender = table.Column<string>(nullable: true),
                    Position = table.Column<string>(nullable: true),
                    Privilege = table.Column<string>(nullable: true),
                    RetiredAt = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
