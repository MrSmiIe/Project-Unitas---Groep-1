using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Project_Unitas.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    FullName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CompanyName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ContactMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Company = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: false),
                    Subject = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Handled = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMessages", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Summary = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MachineSpecs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    MachineId = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    Value = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineSpecs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MachineSpecs_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Machines",
                columns: new[] { "Id", "Category", "Description", "Name", "Price", "Summary" },
                values: new object[,]
                {
                    { 1, "Deegbereiding", "De SK-80 kneedt tot 80 kg deeg per keer met twee snelheden en een omkeerbare kuip. Geschikt voor brood-, pizza- en zoetdeeg.", "Spiraalkneder SK-80", 18950m, "Krachtige spiraalkneder voor grote hoeveelheden deeg." },
                    { 2, "Deegverwerking", "De DV-30 verdeelt een deegklomp in 30 gelijke stukken per cyclus. Het gewicht is traploos instelbaar.", "Deegverdeler DV-30", 12400m, "Verdeelt deeg in gelijke stukken van 50 tot 1000 gram." },
                    { 3, "Deegverwerking", "De OB-2000 maakt van verdeelde deegstukken mooie, gelijkmatige bollen. Ideaal in combinatie met de DV-30.", "Opboller OB-2000", 9750m, "Bolt tot 2000 deegstukken per uur rond op." },
                    { 4, "Deegverwerking", "De LM-600 rolt en vormt deeg tot stokbroden, puntjes en tijgerbroden. De walsafstand is per product in te stellen.", "Langmaker LM-600", 8300m, "Rolt deegbollen uit tot langwerpige broden." },
                    { 5, "Rijzen", "De RK-20 biedt plaats aan 20 bakplaten. Temperatuur en luchtvochtigheid worden automatisch op peil gehouden.", "Rijskast RK-20", 6900m, "Rijskast met regeling van temperatuur en luchtvochtigheid." },
                    { 6, "Bakken", "De DO-4 heeft vier onafhankelijk regelbare dekken met stenen vloer, voor een krokante korst. Inclusief stoominstallatie.", "Dekoven DO-4", 34500m, "Dekoven met vier stenen bakvloeren." },
                    { 7, "Bakken", "In de RO-18 draait een volledig rek met 18 platen rond, zodat alles even bruin wordt. Gas- en elektrische uitvoering.", "Rekoven RO-18", 27800m, "Roterende rekoven voor een gelijkmatig bakresultaat." },
                    { 8, "Afwerking", "De BS-11 snijdt tot 600 broden per uur. Veilig door een afgeschermd mes en noodstop.", "Broodsnijmachine BS-11", 5450m, "Snijdt broden in gelijke plakken van 11 mm." }
                });

            migrationBuilder.InsertData(
                table: "MachineSpecs",
                columns: new[] { "Id", "Label", "MachineId", "SortOrder", "Value" },
                values: new object[,]
                {
                    { 1, "Capaciteit", 1, 0, "80 kg deeg" },
                    { 2, "Snelheden", 1, 1, "2 + omkeerbaar" },
                    { 3, "Vermogen", 1, 2, "7,5 kW" },
                    { 4, "Afmetingen (b × d × h)", 1, 3, "75 × 125 × 140 cm" },
                    { 5, "Stukken per cyclus", 2, 0, "30" },
                    { 6, "Gewicht per stuk", 2, 1, "50 – 1000 g" },
                    { 7, "Vermogen", 2, 2, "1,5 kW" },
                    { 8, "Afmetingen (b × d × h)", 2, 3, "60 × 70 × 130 cm" },
                    { 9, "Capaciteit", 3, 0, "2000 stuks per uur" },
                    { 10, "Gewicht per stuk", 3, 1, "30 – 1200 g" },
                    { 11, "Vermogen", 3, 2, "0,75 kW" },
                    { 12, "Afmetingen (b × d × h)", 3, 3, "65 × 65 × 120 cm" },
                    { 13, "Walsbreedte", 4, 0, "600 mm" },
                    { 14, "Capaciteit", 4, 1, "1500 stuks per uur" },
                    { 15, "Vermogen", 4, 2, "0,55 kW" },
                    { 16, "Afmetingen (b × d × h)", 4, 3, "90 × 80 × 125 cm" },
                    { 17, "Capaciteit", 5, 0, "20 platen (60 × 40 cm)" },
                    { 18, "Temperatuur", 5, 1, "20 – 45 °C" },
                    { 19, "Vermogen", 5, 2, "3 kW" },
                    { 20, "Afmetingen (b × d × h)", 5, 3, "80 × 95 × 205 cm" },
                    { 21, "Dekken", 6, 0, "4" },
                    { 22, "Bakoppervlak", 6, 1, "9,6 m²" },
                    { 23, "Max. temperatuur", 6, 2, "300 °C" },
                    { 24, "Vermogen", 6, 3, "36 kW" },
                    { 25, "Capaciteit", 7, 0, "18 platen (60 × 80 cm)" },
                    { 26, "Max. temperatuur", 7, 1, "280 °C" },
                    { 27, "Vermogen", 7, 2, "45 kW" },
                    { 28, "Afmetingen (b × d × h)", 7, 3, "140 × 200 × 240 cm" },
                    { 29, "Snijdikte", 8, 0, "11 mm" },
                    { 30, "Capaciteit", 8, 1, "600 broden per uur" },
                    { 31, "Vermogen", 8, 2, "0,37 kW" },
                    { 32, "Afmetingen (b × d × h)", 8, 3, "70 × 60 × 85 cm" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_CreatedAtUtc",
                table: "ContactMessages",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Machines_Category",
                table: "Machines",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_MachineSpecs_MachineId",
                table: "MachineSpecs",
                column: "MachineId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ContactMessages");

            migrationBuilder.DropTable(
                name: "MachineSpecs");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Machines");
        }
    }
}
