using AdminPortalV8.Data.ExtendedIdentity;
using AdminPortalV8.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdminPortalV8.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        #region Database Setup
       

        public override int SaveChanges()
        {
            AddAuditInfo();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            AddAuditInfo();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddAuditInfo();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void AddAuditInfo()
        {
            var entries = ChangeTracker.Entries().Where(x => (x.Entity is _BaseAuditInfo) && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach (var entry in entries)
            {
                ////Old logic using utc now
                //if (entry.State == EntityState.Added)
                //{
                //    (entry.Entity as _BaseAuditInfo).Created = System.DateTime.UtcNow;
                //}
                //(entry.Entity as _BaseAuditInfo).Modified = System.DateTime.UtcNow;

                //new modified due to met trip time not accurate
                if (entry.State == EntityState.Added)
                {
                    (entry.Entity as _BaseAuditInfo).Created = System.DateTime.Now;
                }
                (entry.Entity as _BaseAuditInfo).Modified = System.DateTime.Now;


            }

        }
        #endregion

        #region Tables
        public DbSet<ActivityType> ActivityTypes { get; set; }
        public DbSet<InterestType> InterestTypes { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<IdentityRoleProfiles> IdentityRoleProfiles { get; set; }
        public DbSet<IdentityUserProfiles> IdentityUserProfiles { get; set; }
       
        public DbSet<UserPasswordsHistory> UserPasswordsHistory { get; set; }
        public DbSet<IdentityAccessLevelExclusive> IdentityAccessLevelExclusives { get; set; }
        public DbSet<IdentityModule> IdentityModules { get; set; }
        public DbSet<IdentityModuleOrganize> IdentityModuleOrganizes { get; set; }
        public DbSet<IdentityAccessLevel> IdentityAccessLevels { get; set; }
        public DbSet<IdentityRoleAccess> IdentityRoleAccesses { get; set; }
        public DbSet<IdentityAccessLevelDetail> IdentityAccessLevelDetails { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<TableCoordinate> TableCoordinates { get; set; }
        public DbSet<Robot> Robots { get; set; }
        public DbSet<Anchor> Anchors { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<AdditionalRequest> AdditionalRequests { get; set; }
        public DbSet<AdditionalRequestDetail> AdditionalRequestDetails { get; set; }
        public DbSet<TagInfo> TagInfos { get; set; }
        public DbSet<UserRestaurant> userRestaurants { get; set; }

        #endregion


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("User");
            });
            builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable("IdentityRole");
            });

            builder.Entity<UserPasswordsHistory>()
                .HasKey(e => e.UserID);


            builder.Entity<IdentityAccessLevelExclusive>()
                .HasKey(e => e.Pvid);

            builder.Entity<IdentityAccessLevel>()
                .HasKey(e => e.Pvid);

            builder.Entity<IdentityRoleAccess>()
                .HasKey(e => e.Pvid);

            builder.Entity<IdentityAccessLevelDetail>()
                .HasKey(e=>e.Pvid);
           

        }

       
    }
}