namespace Firming_Solution.Domain.Entities;

public class CostCategoryConfig : BaseEntity
{
    public string CategoryKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public CostCategoryConfig? Parent { get; set; }
    public ICollection<CostCategoryConfig> SubCategories { get; set; } = new List<CostCategoryConfig>();
    public int SortOrder { get; set; }
}
