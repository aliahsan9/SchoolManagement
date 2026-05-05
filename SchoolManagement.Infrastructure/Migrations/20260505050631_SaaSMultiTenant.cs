using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SaaSMultiTenant : Migration
    {
        private static readonly string[] IndexColumnsUsersSchoolIdEmail = ["SchoolId", "Email"];
        private static readonly string[] IndexColumnsStudentsSchoolIdAdmissionNumber = ["SchoolId", "AdmissionNumber"];
        private static readonly string[] IndexColumnsParentsSchoolIdUserId = ["SchoolId", "UserId"];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Students_AdmissionNumber",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_SchoolId",
                table: "Students");

            migrationBuilder.AddColumn<Guid>(
                name: "SchoolId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Schools",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Subdomain",
                table: "Schools",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "SchoolId",
                table: "Parents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "AuditLogs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "SchoolId",
                table: "AuditLogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SchoolSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchoolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Plan = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TrialEndsAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentPeriodEndUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MonthlyPriceSnapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    YearlyPriceSnapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolSubscriptions_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchoolSubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExternalTransactionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PaidAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionPayments_SchoolSubscriptions_SchoolSubscriptionId",
                        column: x => x.SchoolSubscriptionId,
                        principalTable: "SchoolSubscriptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_SchoolId_Email",
                table: "Users",
                columns: IndexColumnsUsersSchoolIdEmail,
                unique: true,
                filter: "[SchoolId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Students_SchoolId_AdmissionNumber",
                table: "Students",
                columns: IndexColumnsStudentsSchoolIdAdmissionNumber,
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schools_Subdomain",
                table: "Schools",
                column: "Subdomain",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parents_SchoolId_UserId",
                table: "Parents",
                columns: IndexColumnsParentsSchoolIdUserId);

            migrationBuilder.CreateIndex(
                name: "IX_SchoolSubscriptions_SchoolId",
                table: "SchoolSubscriptions",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPayments_ExternalTransactionId",
                table: "SubscriptionPayments",
                column: "ExternalTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPayments_SchoolSubscriptionId",
                table: "SubscriptionPayments",
                column: "SchoolSubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Parents_Schools_SchoolId",
                table: "Parents",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Schools_SchoolId",
                table: "Users",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parents_Schools_SchoolId",
                table: "Parents");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Schools_SchoolId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "SubscriptionPayments");

            migrationBuilder.DropTable(
                name: "SchoolSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Users_SchoolId_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Students_SchoolId_AdmissionNumber",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Schools_Subdomain",
                table: "Schools");

            migrationBuilder.DropIndex(
                name: "IX_Parents_SchoolId_UserId",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "Subdomain",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "AuditLogs");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "AuditLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_AdmissionNumber",
                table: "Students",
                column: "AdmissionNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_SchoolId",
                table: "Students",
                column: "SchoolId");
        }
    }
}
