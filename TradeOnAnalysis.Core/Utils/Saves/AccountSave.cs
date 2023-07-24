namespace TradeOnAnalysis.Core.Utils.Saves;

public class AccountSave
{
    public string Name { get; set; } = "";
    public string MarketApi { get; set; } = "";
}

public class AccountsSave
{
    public List<AccountSave> Accounts { get; set; } = new();
}
