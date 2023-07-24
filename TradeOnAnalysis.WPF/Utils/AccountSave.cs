using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TradeOnAnalysis.WPF.Utils;

public class AccountSave
{
    public string Name { get; set; } = "";
    public string MarketApi { get; set; } = "";
}

public class AccountsSave
{
    public List<AccountSave> Accounts { get; set; } = new();
}
