using Firming_Solution.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Firming_Solution.Web.Models;

public class LandParcelViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "খামার নির্বাচন করুন।")]
    public int FarmId { get; set; }

    [Required(ErrorMessage = "জমির পরিচিতি নাম দিন।")]
    [Display(Name = "জমির পরিচিতি নাম")]
    public string LandName { get; set; } = string.Empty;

    [Required(ErrorMessage = "পরিমাণ দিন।")]
    [Range(0.001, double.MaxValue, ErrorMessage = "পরিমাণ অবশ্যই শূন্যের বেশি হতে হবে।")]
    [Display(Name = "পরিমাণ")]
    public decimal InputValue { get; set; }

    [Required(ErrorMessage = "একক নির্বাচন করুন।")]
    [Display(Name = "একক")]
    public string InputUnit { get; set; } = "শতাংশ";

    [Display(Name = "মালিকানার ধরন")]
    public OwnershipType OwnershipType { get; set; } = OwnershipType.Own;

    [Display(Name = "মৌসুম প্রতি ইজারা মূল্য (৳)")]
    public decimal? LeaseCostPerSeason { get; set; }

    [Display(Name = "মাটির ধরন")]
    public string? SoilType { get; set; }

    [Display(Name = "সর্বশেষ মাটি পরীক্ষার তারিখ")]
    public DateTime? LastTestedDate { get; set; }

    // Conversion constants (1 unit = X শতাংশ)
    public static readonly Dictionary<string, decimal> ToShotangshoFactor = new()
    {
        { "শতাংশ", 1m },
        { "কাঠা",  1.65289256m },   // 720 / 435.6
        { "বিঘা",  33.0578512m },   // 14400 / 435.6
        { "একর",   100m }
    };

    public static readonly string[] Units = ["শতাংশ", "কাঠা", "বিঘা", "একর"];

    public decimal ToShotangsho() =>
        InputValue * (ToShotangshoFactor.TryGetValue(InputUnit, out var f) ? f : 1m);
}
