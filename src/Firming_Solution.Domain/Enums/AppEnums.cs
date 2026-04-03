namespace Firming_Solution.Domain.Enums;

public enum UserRole
{
    SuperAdmin,
    Manager,
    Worker,
    Accountant,
    Viewer
}

public enum FarmType
{
    Livestock,
    Agriculture,
    Mixed
}

public enum BatchStatus
{
    Planning,
    Active,
    Selling,
    Closed
}

public enum BatchSpecies
{
    Cattle,
    Poultry,
    Fish,
    Goat
}

public enum FeedCategory
{
    Starter,
    Grower,
    Finisher,
    Supplement
}

public enum FeedSession
{
    Morning,
    Noon,
    Evening
}

public enum CostCategory
{
    Feed,
    Medicine,
    Fertilizer,
    Labour,
    Utility,
    Transport,
    Breeding,
    Infrastructure,
    Other
}

public enum OwnershipType
{
    Own,
    Lease,
    Mortgaged
}

public enum CropStatus
{
    Planning,
    Growing,
    Harvested,
    Closed
}

public enum EidType
{
    EidUlAdha,
    EidUlFitr
}

public enum TaskType
{
    Feeding,
    Medicine,
    Weighing,
    Cleaning,
    Inspection,
    Irrigation,
    Fertilising,
    Other
}

public enum TaskStatus
{
    Pending,
    InProgress,
    Done,
    Skipped
}

public enum RecommendationType
{
    SellDate,
    BreedSelection,
    CropSelection,
    FeedOptimisation
}

public enum WeatherCondition
{
    Sunny,
    Cloudy,
    Rainy,
    Storm,
    Foggy
}

public enum InvestmentCategory
{
    LandPurchase,
    Infrastructure,
    Equipment,
    WorkingCapital,
    Other
}

public enum InvestmentSource
{
    OwnFunds,
    BankLoan,
    Investor,
    Other
}
