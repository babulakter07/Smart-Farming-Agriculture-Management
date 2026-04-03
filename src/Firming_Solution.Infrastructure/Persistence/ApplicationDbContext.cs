using Firming_Solution.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Firming_Solution.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Farm> Farms => Set<Farm>();
    public DbSet<UserFarm> UserFarms => Set<UserFarm>();
    public DbSet<Batch> Batches => Set<Batch>();
    public DbSet<FeedType> FeedTypes => Set<FeedType>();
    public DbSet<DailyFeedLog> DailyFeedLogs => Set<DailyFeedLog>();
    public DbSet<WeightLog> WeightLogs => Set<WeightLog>();
    public DbSet<MortalityLog> MortalityLogs => Set<MortalityLog>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<Cost> Costs => Set<Cost>();
    public DbSet<Investment> Investments => Set<Investment>();
    public DbSet<LandParcel> LandParcels => Set<LandParcel>();
    public DbSet<CropSeason> CropSeasons => Set<CropSeason>();
    public DbSet<FertiliserPlan> FertiliserPlans => Set<FertiliserPlan>();
    public DbSet<WeatherLog> WeatherLogs => Set<WeatherLog>();
    public DbSet<EidBazarPlan> EidBazarPlans => Set<EidBazarPlan>();
    public DbSet<DailyTask> DailyTasks => Set<DailyTask>();
    public DbSet<AIRecommendation> AIRecommendations => Set<AIRecommendation>();
    public DbSet<FeedCostHistory> FeedCostHistories => Set<FeedCostHistory>();
    public DbSet<AnimalCostBreakdown> AnimalCostBreakdowns => Set<AnimalCostBreakdown>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<CostCategoryConfig> CostCategoryConfigs => Set<CostCategoryConfig>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Global soft-delete filter
        builder.Entity<Farm>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Batch>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<FeedType>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<DailyFeedLog>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<WeightLog>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<MortalityLog>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Sale>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Cost>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Investment>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<LandParcel>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<CropSeason>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<FertiliserPlan>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<WeatherLog>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<EidBazarPlan>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<DailyTask>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<AIRecommendation>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<FeedCostHistory>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<AnimalCostBreakdown>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<CostCategoryConfig>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<UserFarm>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<AppUser>().HasQueryFilter(e => !e.IsDeleted);

        // Farm relationships
        builder.Entity<Farm>()
            .HasOne(f => f.Owner)
            .WithMany(u => u.OwnedFarms)
            .HasForeignKey(f => f.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // UserFarm (many-to-many)
        builder.Entity<UserFarm>()
            .HasOne(uf => uf.User)
            .WithMany(u => u.UserFarms)
            .HasForeignKey(uf => uf.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserFarm>()
            .HasOne(uf => uf.Farm)
            .WithMany(f => f.UserFarms)
            .HasForeignKey(uf => uf.FarmId)
            .OnDelete(DeleteBehavior.Cascade);

        // Batch → Farm
        builder.Entity<Batch>()
            .HasOne(b => b.Farm)
            .WithMany(f => f.Batches)
            .HasForeignKey(b => b.FarmId)
            .OnDelete(DeleteBehavior.Restrict);

        // DailyFeedLog
        builder.Entity<DailyFeedLog>()
            .HasOne(d => d.Batch)
            .WithMany(b => b.FeedLogs)
            .HasForeignKey(d => d.BatchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DailyFeedLog>()
            .HasOne(d => d.FeedType)
            .WithMany(f => f.DailyFeedLogs)
            .HasForeignKey(d => d.FeedTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DailyFeedLog>()
            .HasOne(d => d.LoggedBy)
            .WithMany(u => u.FeedLogs)
            .HasForeignKey(d => d.LoggedById)
            .OnDelete(DeleteBehavior.SetNull);

        // TotalCost is computed in memory, not in DB
        builder.Entity<DailyFeedLog>()
            .Ignore(d => d.TotalCost);

        // Cost
        builder.Entity<Cost>()
            .HasOne(c => c.Farm)
            .WithMany(f => f.Costs)
            .HasForeignKey(c => c.FarmId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Cost>()
            .HasOne(c => c.Batch)
            .WithMany(b => b.Costs)
            .HasForeignKey(c => c.BatchId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Cost>()
            .HasOne(c => c.CropSeason)
            .WithMany(cs => cs.Costs)
            .HasForeignKey(c => c.CropSeasonId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Cost>()
            .HasOne(c => c.EnteredBy)
            .WithMany(u => u.EnteredCosts)
            .HasForeignKey(c => c.EnteredById)
            .OnDelete(DeleteBehavior.SetNull);

        // Investment
        builder.Entity<Investment>()
            .HasOne(i => i.RecordedBy)
            .WithMany(u => u.RecordedInvestments)
            .HasForeignKey(i => i.RecordedById)
            .OnDelete(DeleteBehavior.SetNull);

        // DailyTask
        builder.Entity<DailyTask>()
            .HasOne(t => t.AssignedTo)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);

        // EidBazarPlan
        builder.Entity<EidBazarPlan>()
            .HasOne(e => e.LinkedBatch)
            .WithMany(b => b.EidBazarPlans)
            .HasForeignKey(e => e.LinkedBatchId)
            .OnDelete(DeleteBehavior.SetNull);

        // Decimal precision config
        builder.Entity<Farm>().Property(f => f.Latitude).HasPrecision(10, 7);
        builder.Entity<Farm>().Property(f => f.Longitude).HasPrecision(10, 7);
        builder.Entity<Farm>().Property(f => f.TotalArea).HasPrecision(10, 3);
        builder.Entity<Batch>().Property(b => b.InitialWeight_kg).HasPrecision(10, 2);
        builder.Entity<Batch>().Property(b => b.PurchaseCost).HasPrecision(15, 2);
        builder.Entity<FeedType>().Property(f => f.CurrentPrice).HasPrecision(12, 2);
        builder.Entity<DailyFeedLog>().Property(d => d.Quantity_kg).HasPrecision(10, 3);
        builder.Entity<DailyFeedLog>().Property(d => d.PricePerKg).HasPrecision(10, 2);
        builder.Entity<WeightLog>().Property(w => w.AvgWeight_kg).HasPrecision(8, 3);
        builder.Entity<WeightLog>().Property(w => w.TotalEstWeight).HasPrecision(12, 3);
        builder.Entity<WeightLog>().Property(w => w.FCR_Cumulative).HasPrecision(6, 3);
        builder.Entity<MortalityLog>().Property(m => m.EstimatedLoss).HasPrecision(15, 2);
        builder.Entity<Sale>().Property(s => s.TotalWeight_kg).HasPrecision(10, 3);
        builder.Entity<Sale>().Property(s => s.PricePerKg).HasPrecision(10, 2);
        builder.Entity<Sale>().Property(s => s.TotalRevenue).HasPrecision(15, 2);
        builder.Entity<Cost>().Property(c => c.Amount).HasPrecision(15, 2);
        builder.Entity<Investment>().Property(i => i.Amount).HasPrecision(18, 2);
        builder.Entity<LandParcel>().Property(l => l.Area_Decimal).HasPrecision(10, 3);
        builder.Entity<LandParcel>().Property(l => l.LeaseCostPerSeason).HasPrecision(15, 2);
        builder.Entity<CropSeason>().Property(c => c.ExpectedYield_kg).HasPrecision(12, 3);
        builder.Entity<CropSeason>().Property(c => c.ActualYield_kg).HasPrecision(12, 3);
        builder.Entity<CropSeason>().Property(c => c.SeedCost).HasPrecision(15, 2);
        // FertiliserPlan relationships — restrict to avoid multiple cascade paths to LandParcel
        builder.Entity<FertiliserPlan>()
            .HasOne(f => f.Land)
            .WithMany(l => l.FertiliserPlans)
            .HasForeignKey(f => f.LandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FertiliserPlan>()
            .HasOne(f => f.Season)
            .WithMany(s => s.FertiliserPlans)
            .HasForeignKey(f => f.SeasonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FertiliserPlan>().Property(f => f.DoseKgPerDecimal).HasPrecision(8, 3);
        builder.Entity<FertiliserPlan>().Property(f => f.TotalQuantity_kg).HasPrecision(12, 3);
        builder.Entity<FertiliserPlan>().Property(f => f.PricePerKg).HasPrecision(10, 2);
        builder.Entity<FertiliserPlan>().Property(f => f.TotalCost).HasPrecision(15, 2);
        builder.Entity<WeatherLog>().Property(w => w.TempMax_C).HasPrecision(5, 2);
        builder.Entity<WeatherLog>().Property(w => w.TempMin_C).HasPrecision(5, 2);
        builder.Entity<WeatherLog>().Property(w => w.Humidity_Pct).HasPrecision(5, 2);
        builder.Entity<WeatherLog>().Property(w => w.Rainfall_mm).HasPrecision(7, 2);
        builder.Entity<WeatherLog>().Property(w => w.FeedDemandIndex).HasPrecision(4, 2);
        builder.Entity<EidBazarPlan>().Property(e => e.TargetWeightPerAnimal).HasPrecision(8, 2);
        builder.Entity<EidBazarPlan>().Property(e => e.ExpectedPricePerKg).HasPrecision(10, 2);
        builder.Entity<EidBazarPlan>().Property(e => e.ExpectedRevenue).HasPrecision(15, 2);
        builder.Entity<AIRecommendation>().Property(a => a.ConfidenceScore).HasPrecision(5, 2);
        builder.Entity<FeedCostHistory>().Property(f => f.PricePerKg).HasPrecision(10, 2);

        // Enum conversions
        builder.Entity<Farm>().Property(f => f.FarmType).HasConversion<string>();
        builder.Entity<Batch>().Property(b => b.Species).HasConversion<string>();
        builder.Entity<Batch>().Property(b => b.Status).HasConversion<string>();
        builder.Entity<FeedType>().Property(f => f.Category).HasConversion<string>();
        builder.Entity<DailyFeedLog>().Property(d => d.Session).HasConversion<string>();
        builder.Entity<LandParcel>().Property(l => l.OwnershipType).HasConversion<string>();
        builder.Entity<CropSeason>().Property(c => c.Status).HasConversion<string>();
        builder.Entity<EidBazarPlan>().Property(e => e.EidType).HasConversion<string>();
        builder.Entity<DailyTask>().Property(t => t.TaskType).HasConversion<string>();
        builder.Entity<DailyTask>().Property(t => t.Status).HasConversion<string>();
        builder.Entity<AIRecommendation>().Property(a => a.RecoType).HasConversion<string>();
        builder.Entity<WeatherLog>().Property(w => w.WeatherCondition).HasConversion<string>();
        builder.Entity<Investment>().Property(i => i.Category).HasConversion<string>();
        builder.Entity<Investment>().Property(i => i.Source).HasConversion<string>();
        builder.Entity<AppUser>().Property(u => u.Role).HasConversion<string>();

        // CostCategoryConfig self-reference (no cascade to avoid circular paths)
        builder.Entity<CostCategoryConfig>()
            .HasOne(c => c.Parent)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;
            if (entry.State == EntityState.Added)
                entity.CreatedAt = DateTime.UtcNow;
            else
                entity.ModifiedAt = DateTime.UtcNow;
        }
    }
}
