using MicroBase.Entity.Accounts;
using MicroBase.Entity.Localtions;
using MicroBase.Entity.Notifications;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MicroBase.Entity
{
    public class MicroDbContext : IdentityDbContext<IdentityUser, PrivilegesRole, Guid>
    {
        public MicroDbContext(DbContextOptions<MicroDbContext> options)
            : base(options)
        {

        }

        #region IdentityUser

        public DbSet<IdentityUser> IdentityUsers { get; set; }

        public DbSet<IdentityUserMetaData> IdentityUserMetaDatas { get; set; }

        public DbSet<IdentityUserActivity> IdentityUserActivities { get; set; }

        public DbSet<IdentityUserTwoFA> IdentityUserTwoFAs { get; set; }

        #region Roles
        
        public DbSet<PrivilegesRole> PrivilegesRoles { get; set; }

        public DbSet<PrivilegesGroup> PrivilegesGroups { get; set; }

        public DbSet<PrivilegesUserRoleMap> PrivilegesUserRoleMaps { get; set; }

        public DbSet<PrivilegesRoleGroupMap> PrivilegesRoleGroupMaps { get; set; }

        #endregion

        public DbSet<SystemMenu> SystemMenus { get; set; }

        #endregion

        public DbSet<EmailTemplate> EmailTemplates { get; set; }

        public DbSet<LocalizationKey> LocalizationKeys { get; set; }

        public DbSet<SiteSetting> SiteSettings { get; set; }

        //public DbSet<IdentityUserAppInfo> IdentityUserAppInfos { get; set; }

        //public DbSet<NotificationInBox> NotificationInBoxs { get; set; }

        //public DbSet<NotificationSetting> NotificationSettings { get; set; }

        //public DbSet<NotificationUser> NotificationUsers { get; set; }

        #region Localtions

        //public DbSet<Province> Provinces { get; set; }

        //public DbSet<District> Districts { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Only For Postgresql

            //foreach (var property in modelBuilder.Model.GetEntityTypes()
            //     .SelectMany(t => t.GetProperties())
            //     .Where
            //     (p
            //       => p.ClrType == typeof(DateTime)
            //          || p.ClrType == typeof(DateTime?)
            //     ))
            //{
            //    property.SetColumnType("timestamp without time zone");
            //}

            #endregion

            modelBuilder.Entity<IdentityUser>(entity =>
            {
                entity.ToTable(name: "IdentityUsers");
            });

            modelBuilder.Entity<PrivilegesRole>(entity =>
            {
                entity.ToTable(name: "IdentityUser_AC_Roles");
            });

            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>>(entity =>
            {
                entity.ToTable(name: "IdentityRoleClaims");
            });

            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<Guid>>(entity =>
            {
                entity.ToTable(name: "IdentityUserLogins");
            });

            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>>(entity =>
            {
                entity.ToTable(name: "IdentityUserRoles");
            });

            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<Guid>>(entity =>
            {
                entity.ToTable(name: "IdentityUserTokens");
            });

            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>>(entity =>
            {
                entity.ToTable(name: "IdentityUserClaims");
            });

            //modelBuilder.Entity<ExternalAccount>()
            //    .HasIndex(c => c.ExternalAccountId)
            //    .IsUnique();

            //var converter = new ValueConverter<Guid, byte[]>(
            //    to => to.ToByteArray(),
            //    from => new Guid(from));

            //foreach (var property in modelBuilder.Model.GetEntityTypes()
            //    .SelectMany(t => t.GetProperties())
            //    .Where(p => p.ClrType == typeof(Guid) ||
            //                    p.ClrType == typeof(Guid?)))
            //{
            //    property.SetValueConverter(converter);
            //}
        }
    }
}