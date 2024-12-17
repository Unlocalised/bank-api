using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BankTier
{
    A, B, C
}

public class BankModel
{
    [IdSchema][BankId]
    public Guid Id { get; set; }

    [DefaultValue("Guanchen")]
    [Description("Name of the bank.")]
    [MaxLength(Int32.MaxValue)]
    [RegularExpression(".*")]
    public string? Name { get; set; }

    [DefaultValue(true)]
    [ConfidentialData]
    [Description("Compliancy status of the bank.")]
    public bool IsCompliant { get; set; }

    [DefaultValue(BankTier.A)]
    [Description("Tier of the bank.")]
    public BankTier BankTier { get; set; }
}
