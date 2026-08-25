using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BasicStructure.DataLayer
{
    public class DataContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedRoles(builder);
            SeedAPIS(builder);
            SeedPermissions(builder);
        }

        private static void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole<int>>().HasData
            (
                new IdentityRole<int>() { Id = 1, Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
                new IdentityRole<int>() { Id = 2, Name = "Manager", ConcurrencyStamp = "2", NormalizedName = "Manager" },
                new IdentityRole<int>() { Id = 3, Name = "Worker", ConcurrencyStamp = "3", NormalizedName = "Worker" }
            );
        }

        private static void SeedAPIS(ModelBuilder builder)
        {
            builder.Entity<API>().HasData
            (
                new API() { Id = 1, Name = "CreateUser", EndPoint = "post/api/User", IsBackend = true},
                new API() { Id = 2, Name = "UpdateUser", EndPoint = "put/api/User", IsBackend = true },
                new API() { Id = 3, Name = "GetAllUsers", EndPoint = "get/api/User", IsBackend = true },
                new API() { Id = 4, Name = "GetUserById", EndPoint = "get/api/User/id", IsBackend = true },
                new API() { Id = 5, Name = "DeleteUserById", EndPoint = "delete/api/User/id", IsBackend = true }
            );
        }

        private static void SeedPermissions(ModelBuilder builder)
        {
            builder.Entity<PermissionMatrix>().HasData
            (
                new PermissionMatrix() { Id = 1, APIId = 1, IdentityRoleId = 1 },
                new PermissionMatrix() { Id = 2, APIId = 2, IdentityRoleId = 1 },
                new PermissionMatrix() { Id = 3, APIId = 3, IdentityRoleId = 1 },
                new PermissionMatrix() { Id = 4, APIId = 4, IdentityRoleId = 1 },
                new PermissionMatrix() { Id = 5, APIId = 5, IdentityRoleId = 1 }
            );
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Field> Fields => Set<Field>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Coordinate> Coordinates => Set<Coordinate>();
        public DbSet<API> APIS => Set<API>();
        public DbSet<PermissionMatrix> Permissions => Set<PermissionMatrix>();

    }
}
