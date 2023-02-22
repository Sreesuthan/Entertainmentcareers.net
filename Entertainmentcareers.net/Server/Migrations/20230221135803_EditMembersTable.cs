using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entertainmentcareers.net.Server.Migrations
{
    /// <inheritdoc />
    public partial class EditMembersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VerifiedAt",
                table: "Members",
                newName: "ConfirmedAt");

            migrationBuilder.RenameColumn(
                name: "VerificationToken",
                table: "Members",
                newName: "ConfirmationToken");

            migrationBuilder.AddColumn<string>(
                name: "CoverFileContentType",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CoverLetterPath",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CoverOriginalFileName",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CoverStoredFileName",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "Members",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ResumeFileContentType",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResumeOriginalFileName",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResumePath",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResumeStoredFileName",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverFileContentType",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "CoverLetterPath",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "CoverOriginalFileName",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "CoverStoredFileName",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ResumeFileContentType",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ResumeOriginalFileName",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ResumePath",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ResumeStoredFileName",
                table: "Members");

            migrationBuilder.RenameColumn(
                name: "ConfirmedAt",
                table: "Members",
                newName: "VerifiedAt");

            migrationBuilder.RenameColumn(
                name: "ConfirmationToken",
                table: "Members",
                newName: "VerificationToken");
        }
    }
}
