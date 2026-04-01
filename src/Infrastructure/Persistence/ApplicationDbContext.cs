using Firming_Solution.Domain.Common;
using Firming_Solution.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Farm> Farms => Set<Farm>();
    public DbSet<UserFarm> UserFarms => Set<UserFarm>();
    public DbSet<Batch> Batches => Set<Batch>();
    public DbSet<FeedType> FeedTypes => Set<FeedType>();
    public DbSet<DailyFeedLog> DailyFeedLogs => Set<DailyFeedLog>();
    public DbSet<Cost> Costs => Set<Cost>();
    public DbSet<WeightLog> WeightLogs => Set<WeightLog>();
    public DbSet<MortalityLog> MortalityLogs => Set<MortalityLog>();
    public DbSet<Buyer> Buyers => Set<Buyer>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<LandParcel> LandParcels => Set<LandParcel>();
    public DbSet<CropSeason> CropSeasons => Set<CropSeason>();
    public DbSet<FertiliserPlan> FertiliserPlans => Set<FertiliserPlan>();
    public DbSet<WeatherLog> WeatherLogs => Set<WeatherLog>();
    public DbSet<EidBazarPlan> EidBazarPlans => Set<EidBazarPlan>();
    public DbSet<DailyTask> DailyTasks => Set<DailyTask>();
    public DbSet<Investment> Investments => Set<Investment>();
    public DbSet<AiRecommendation> AiRecommendations => Set<AiRecommendation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply configurations
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global soft-delete filter for all BaseEntity types
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(ApplicationDbContext)
                    .GetMethod(nameof(SetSoftDeleteFilter),
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);
                method.Invoke(null, [builder]);
            }
        }
    }

    private static void SetSoftDeleteFilter<T>(ModelBuilder builder) where T : BaseEntity
    {
        builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override int SaveChanges()
    {
        SetAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetAuditFields()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedDate = DateTime.UtcNow;
                    break;
            }
        }
    }
}
