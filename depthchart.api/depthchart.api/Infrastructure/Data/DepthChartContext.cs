using Bolt.Common.Extensions;
using depthchart.api.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace depthchart.api.Infrastructure.Data
{
    public class DepthChartContext : DbContext
    {
        public DepthChartContext(DbContextOptions<DepthChartContext> options) : base(options)
        {
            Database.EnsureCreated();

            try
            {
                Database.OpenConnection();
            }
            catch (Exception ex)
            {
                // This will break when mocked.
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var type = typeof(IEntityConfiguration);

            typeof(DepthChartContext).Assembly
                .GetTypes()
                .Where(t => type.IsAssignableFrom(t) && t.IsClass)
                .Select(Activator.CreateInstance)
                .Cast<IEntityConfiguration>()
                .All(configuration =>
                {
                    configuration.Configure(modelBuilder);
                    return true;
                });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ValidEntitiesBeingModified();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            ValidEntitiesBeingModified();
            return base.SaveChanges();
        }

        private void ValidEntitiesBeingModified()
        {
            var changedEntities = ChangeTracker
                .Entries()
                .Where(_ => _.State == EntityState.Added ||
                            _.State == EntityState.Modified);

            var errors = new List<ValidationResult>();
            foreach (var e in changedEntities)
            {
                var vc = new ValidationContext(e.Entity, null, null);
                Validator.TryValidateObject(e.Entity, vc, errors, validateAllProperties: true);
            }

            if (errors.HasItem()) throw new InvalidOperationException(errors.Map(vr => vr.ErrorMessage).FirstOrDefault());
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<DepthPosition> DepthPositions { get; set; }
    }
}