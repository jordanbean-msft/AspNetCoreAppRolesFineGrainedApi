using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCoreWithAppRolesAndFineGrained.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Branch",
                columns: table => new
                {
                    BranchID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    RegionalManagerAADGroupId = table.Column<string>(type: "TEXT", nullable: true),
                    SalespersonAADGroupId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branch", x => x.BranchID);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchID = table.Column<int>(type: "INTEGER", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    UserPrincipalName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.EmployeeID);
                    table.ForeignKey(
                        name: "FK_Employee_Branch_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branch",
                        principalColumn: "BranchID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Salary",
                columns: table => new
                {
                    SalaryID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeID = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salary", x => x.SalaryID);
                    table.ForeignKey(
                        name: "FK_Salary_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sale",
                columns: table => new
                {
                    SaleID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeID = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sale", x => x.SaleID);
                    table.ForeignKey(
                        name: "FK_Sale_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employee_BranchID",
                table: "Employee",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_Salary_EmployeeID",
                table: "Salary",
                column: "EmployeeID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sale_EmployeeID",
                table: "Sale",
                column: "EmployeeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Salary");

            migrationBuilder.DropTable(
                name: "Sale");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Branch");
        }
    }
}
