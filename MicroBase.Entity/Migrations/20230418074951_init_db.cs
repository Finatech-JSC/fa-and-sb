using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroBase.Entity.Migrations
{
    public partial class init_db : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Key = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Subject = table.Column<string>(type: "NVARCHAR2(512)", maxLength: 512, nullable: false),
                    Body = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CultureCode = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    IsDelete = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUser_AC_RoleGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    IsDefault = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    AllowFullAccess = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Enabled = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IsDelete = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUser_AC_RoleGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UserName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: false),
                    AccountType = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    LastLoginIpAddress = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true),
                    LastLoginTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Via = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    IsDelete = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    IsDefaultPassword = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    EmailConfirmDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    PhoneConfirmDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    IsSystemLocked = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    LockedDescription = table.Column<string>(type: "NVARCHAR2(512)", maxLength: 512, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Localization_Keys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Prefix = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    CultureCode = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Key = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IsDelete = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localization_Keys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FullName = table.Column<string>(type: "NVARCHAR2(512)", maxLength: 512, nullable: false),
                    ShortName = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    Enabled = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CountryCode = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: true),
                    Order = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IsDelete = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Key = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    ModelField = table.Column<string>(type: "NVARCHAR2(512)", maxLength: 512, nullable: true),
                    ModelFieldIsArray = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    GroupKey = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    StringValue = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    BoolValue = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    NumberValue = table.Column<decimal>(type: "DECIMAL(18, 2)", nullable: true),
                    Order = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    IsSecret = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IsDelete = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "System_Menus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Code = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    FontIcon = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    ImageUrlIcon = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    Route = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    Target = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    CssClass = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    ParentId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Enabled = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IsDelete = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_System_Menus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_System_Menus_System_Menus_ParentId",
                        column: x => x.ParentId,
                        principalTable: "System_Menus",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IdentityUser_AC_Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: false),
                    Code = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    GroupName = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    GroupCode = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    HttpMethod = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    Route = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    BaseRoute = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR2(512)", maxLength: 512, nullable: true),
                    IsDelete = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    IdentityUserRoleGroupId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    NormalizedName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUser_AC_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityUser_AC_Roles_IdentityUser_AC_RoleGroups_IdentityUserRoleGroupId",
                        column: x => x.IdentityUserRoleGroupId,
                        principalTable: "IdentityUser_AC_RoleGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IdentityUser_Activities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IpAddress = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true),
                    Location = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    UserAgent = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    Action = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    Via = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    IdentityUserId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    UserName = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    IsDelete = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUser_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityUser_Activities_IdentityUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IdentityUser_TwoFA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TwoFactorService = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Setting = table.Column<string>(type: "NCLOB", maxLength: 5000, nullable: true),
                    IdentityUserId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IsDelete = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUser_TwoFA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityUser_TwoFA_IdentityUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    UserId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ClaimType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ClaimValue = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityUserClaims_IdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UserId = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_IdentityUserLogins_IdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    LoginProvider = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Value = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_IdentityUserTokens_IdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProvinceId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FullName = table.Column<string>(type: "NVARCHAR2(512)", maxLength: 512, nullable: false),
                    ShortName = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    Enabled = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IsDelete = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Districts_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUser_MetaData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentityUserId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Address = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    CountryCode = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: true),
                    ProvinceId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    DistrictId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Avatar = table.Column<string>(type: "NCLOB", maxLength: 5000, nullable: true),
                    Gender = table.Column<short>(type: "NUMBER(3)", nullable: true),
                    ReferralCount = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ReferralWeekCount = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ReferralMonthCount = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DefaultLanguage = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: true),
                    AllowAppNotification = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    AllowEmailNotification = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PostCode = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    WalletAddress = table.Column<string>(type: "NVARCHAR2(512)", maxLength: 512, nullable: true),
                    NormalizedWalletAddress = table.Column<string>(type: "NVARCHAR2(512)", maxLength: 512, nullable: true),
                    IsDelete = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUser_MetaData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityUser_MetaData_IdentityUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdentityUser_MetaData_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IdentityRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    RoleId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ClaimType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ClaimValue = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityRoleClaims_IdentityUser_AC_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "IdentityUser_AC_Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUser_AC_Groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentityUserId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RoleGroupId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    RoleId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    IsDelete = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUser_AC_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityUser_AC_Groups_IdentityUser_AC_RoleGroups_RoleGroupId",
                        column: x => x.RoleGroupId,
                        principalTable: "IdentityUser_AC_RoleGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdentityUser_AC_Groups_IdentityUser_AC_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "IdentityUser_AC_Roles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdentityUser_AC_Groups_IdentityUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUser_AC_RoleGroup_Maps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RoleGroupId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RoleId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IsDelete = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUser_AC_RoleGroup_Maps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityUser_AC_RoleGroup_Maps_IdentityUser_AC_RoleGroups_RoleGroupId",
                        column: x => x.RoleGroupId,
                        principalTable: "IdentityUser_AC_RoleGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdentityUser_AC_RoleGroup_Maps_IdentityUser_AC_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "IdentityUser_AC_Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RoleId = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_IdentityUserRoles_IdentityUser_AC_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "IdentityUser_AC_Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdentityUserRoles_IdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Districts_ProvinceId",
                table: "Districts",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityRoleClaims_RoleId",
                table: "IdentityRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUser_AC_Groups_IdentityUserId",
                table: "IdentityUser_AC_Groups",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUser_AC_Groups_RoleGroupId",
                table: "IdentityUser_AC_Groups",
                column: "RoleGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUser_AC_Groups_RoleId",
                table: "IdentityUser_AC_Groups",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUser_AC_RoleGroup_Maps_RoleGroupId",
                table: "IdentityUser_AC_RoleGroup_Maps",
                column: "RoleGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUser_AC_RoleGroup_Maps_RoleId",
                table: "IdentityUser_AC_RoleGroup_Maps",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUser_AC_Roles_IdentityUserRoleGroupId",
                table: "IdentityUser_AC_Roles",
                column: "IdentityUserRoleGroupId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "IdentityUser_AC_Roles",
                column: "NormalizedName",
                unique: true,
                filter: "\"NormalizedName\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUser_Activities_IdentityUserId",
                table: "IdentityUser_Activities",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUser_MetaData_IdentityUserId",
                table: "IdentityUser_MetaData",
                column: "IdentityUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUser_MetaData_ProvinceId",
                table: "IdentityUser_MetaData",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUser_TwoFA_IdentityUserId",
                table: "IdentityUser_TwoFA",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserClaims_UserId",
                table: "IdentityUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserLogins_UserId",
                table: "IdentityUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserRoles_RoleId",
                table: "IdentityUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "IdentityUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "IdentityUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_System_Menus_ParentId",
                table: "System_Menus",
                column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "EmailTemplates");

            migrationBuilder.DropTable(
                name: "IdentityRoleClaims");

            migrationBuilder.DropTable(
                name: "IdentityUser_AC_Groups");

            migrationBuilder.DropTable(
                name: "IdentityUser_AC_RoleGroup_Maps");

            migrationBuilder.DropTable(
                name: "IdentityUser_Activities");

            migrationBuilder.DropTable(
                name: "IdentityUser_MetaData");

            migrationBuilder.DropTable(
                name: "IdentityUser_TwoFA");

            migrationBuilder.DropTable(
                name: "IdentityUserClaims");

            migrationBuilder.DropTable(
                name: "IdentityUserLogins");

            migrationBuilder.DropTable(
                name: "IdentityUserRoles");

            migrationBuilder.DropTable(
                name: "IdentityUserTokens");

            migrationBuilder.DropTable(
                name: "Localization_Keys");

            migrationBuilder.DropTable(
                name: "SiteSettings");

            migrationBuilder.DropTable(
                name: "System_Menus");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropTable(
                name: "IdentityUser_AC_Roles");

            migrationBuilder.DropTable(
                name: "IdentityUsers");

            migrationBuilder.DropTable(
                name: "IdentityUser_AC_RoleGroups");
        }
    }
}
