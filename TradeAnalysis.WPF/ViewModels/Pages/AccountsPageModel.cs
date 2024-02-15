﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using TradeAnalysis.Core.Utils.Saves;

namespace TradeAnalysis.WPF.ViewModels;

public class AccountsPageModel : ViewModelBase
{
    private const string SaveFilePath = "Accounts.json";

    private ObservableCollection<AccountDataModel> _accounts = new();

    private RelayCommand? _addAccountCommand;

    public AccountsPageModel()
    {
        LoadAccounts();
    }

    public ObservableCollection<AccountDataModel> Accounts
    {
        get => _accounts;
        private set => ChangeProperty(ref _accounts, value);
    }

    public IEnumerable<AccountDataModel> LoadedAccounts
    {
        get => Accounts.Where(account => account.Account is not null);
    }

    public RelayCommand AddAccountCommand
    {
        get => _addAccountCommand ??= new(obj => AddAccount());
    }

    public void AddAccount(AccountDataModel? data = null)
    {
        data ??= new();
        data.RemoveCommand.AddExecute(obj => { Accounts.Remove(data); SaveAccounts(); });
        data.LoadCommand.AddExecute(obj => SaveAccounts());
        Accounts.Add(data);
    }

    public void LoadAccounts()
    {
        AccountsSave? save = JsonSave.Load<AccountsSave>(SaveFilePath);
        if (save == null)
            return;
        Accounts.Clear();
        foreach (AccountSave savedAccount in save.Accounts)
        {
            AccountDataModel account = new();
            account.LoadSave(savedAccount);
            AddAccount(account);
            account.LoadCommand.Execute(this);
        }
    }

    public void SaveAccounts()
    {
        AccountsSave save = new()
        {
            Accounts = Accounts.Select(account => account.GetSave()).ToList()
        };
        JsonSave.Save(save, SaveFilePath);
    }
}
