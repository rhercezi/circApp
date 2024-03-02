using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace User.Query.Domain.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    username = table.Column<string>(name: "user_name", type: "nvarchar(max)", nullable: false),
                    passhash = table.Column<string>(name: "pass_hash", type: "nvarchar(max)", nullable: false),
                    firstname = table.Column<string>(name: "first_name", type: "nvarchar(max)", nullable: false),
                    familyname = table.Column<string>(name: "family_name", type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    emailverified = table.Column<bool>(name: "email_verified", type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
