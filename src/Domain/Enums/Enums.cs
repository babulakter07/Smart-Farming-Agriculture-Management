namespace Firming_Solution.Domain.Enums;

public enum UserRole
{
    SuperAdmin,
    FarmManager,
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

public enum CostCategory
{
    Feed,
    Medicine,
    Labour,
    Utility,
    Transport,
    Other
}

public enum TaskStatus
{
    Pending,
    InProgress,
    Done,
    Skipped
}

public enum TaskType
{
    Feeding,
    Medicine,
    Weighing,
    Cleaning,
    Inspection
}

public enum CropSeasonStatus
{
    Planning,
    Growing,
    Harvested,
    Closed
}

public enum OwnershipType
{
    Own,
    Lease,
    Mortgaged
}

public enum EidType
{
    EidUlAdha,
    EidUlFitr
}

public enum FeedSession
{
    Morning,
    Noon,
    Evening
}

public enum WeatherCondition
{
    Sunny,
    Cloudy,
    Rainy,
    Storm
}

public enum InvestmentSource
{
    OwnFunds,
    BankLoan,
    Investor
}

public enum AiRecoType
{
    SellDate,
    BreedSelection,
    CropSelection,
    FeedOptimisation
}
