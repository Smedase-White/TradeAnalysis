namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public enum CalculationType
{
    Sum,
    Avg,
    Last
}

[AttributeUsage(AttributeTargets.Property)]
public class CombinableAttribute : Attribute
{
    public CalculationType _calculationType;

    public CombinableAttribute(CalculationType calculationType)
    {
        _calculationType = calculationType;
    }

    public CalculationType CalculationType
    {
        get => _calculationType;
    }
}
