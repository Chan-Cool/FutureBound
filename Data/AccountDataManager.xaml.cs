using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using FutureBound.Models;
using FutureBound.Data;

namespace FutureBound.Data
{
    /// <summary>
    /// Static class for managing account data persistence (TotalAmount, Bills, Transactions)
    /// 静态类，用于管理账户数据持久化（总金额、账单、交易记录）
    /// </summary>
    public static class AccountDataManager
    {
        // Preferences key prefix for total amount
        // 总金额的偏好设置键前缀
        private const string TotalAmountPrefix = "TotalAmount_";
        // Preferences key prefix for bills
        // 账单的偏好设置键前缀
        private const string BillsPrefix = "Bills_";
        // Preferences key prefix for transactions
        // 交易记录的偏好设置键前缀
        private const string TransactionsPrefix = "Transactions_";

        /// <summary>
        /// Save total amount to local preferences
        /// 将总金额保存到本地偏好设置
        /// </summary>
        /// <param name="amount">Total amount to save</param>
        public static void SaveTotalAmount(decimal amount)
        {
            if (string.IsNullOrEmpty(AccountContext.CurrentUsername)) return;
            Preferences.Set($"{TotalAmountPrefix}{AccountContext.CurrentUsername}", amount.ToString());
        }

        /// <summary>
        /// Load total amount from local preferences
        /// 从本地偏好设置加载总金额
        /// </summary>
        /// <returns>Total amount for current user</returns>
        public static decimal LoadTotalAmount()
        {
            if (string.IsNullOrEmpty(AccountContext.CurrentUsername)) return 0;
            string val = Preferences.Get($"{TotalAmountPrefix}{AccountContext.CurrentUsername}", "0");
            decimal.TryParse(val, out decimal amount);
            return amount;
        }

        /// <summary>
        /// Save bills collection to local preferences (JSON serialized)
        /// 将账单集合保存到本地偏好设置（JSON序列化）
        /// </summary>
        /// <param name="bills">ObservableCollection of Bill objects</param>
        public static void SaveBills(ObservableCollection<Bill> bills)
        {
            if (string.IsNullOrEmpty(AccountContext.CurrentUsername) || bills == null) return;
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            string json = JsonConvert.SerializeObject(bills, settings);
            Preferences.Set($"{BillsPrefix}{AccountContext.CurrentUsername}", json);
        }

        /// <summary>
        /// Load bills collection from local preferences (JSON deserialized)
        /// 从本地偏好设置加载账单集合（JSON反序列化）
        /// </summary>
        /// <returns>ObservableCollection of Bill objects</returns>
        public static ObservableCollection<Bill> LoadBills()
        {
            if (string.IsNullOrEmpty(AccountContext.CurrentUsername))
                return new ObservableCollection<Bill>();

            string json = Preferences.Get($"{BillsPrefix}{AccountContext.CurrentUsername}", "[]");
            var list = JsonConvert.DeserializeObject<List<Bill>>(json) ?? new List<Bill>();
            return new ObservableCollection<Bill>(list);
        }

        /// <summary>
        /// Save transactions collection to local preferences (JSON serialized)
        /// Fix: Resolve serialization crash issue
        /// 将交易记录集合保存到本地偏好设置（JSON序列化）
        /// 修复：解决序列化崩溃问题
        /// </summary>
        /// <param name="transactions">ObservableCollection of Transaction objects</param>
        public static void SaveTransactions(ObservableCollection<Transaction> transactions)
        {
            if (string.IsNullOrEmpty(AccountContext.CurrentUsername) || transactions == null) return;
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            string json = JsonConvert.SerializeObject(transactions, settings);
            Preferences.Set($"{TransactionsPrefix}{AccountContext.CurrentUsername}", json);
        }

        /// <summary>
        /// Load transactions collection from local preferences (JSON deserialized)
        /// 从本地偏好设置加载交易记录集合（JSON反序列化）
        /// </summary>
        /// <returns>ObservableCollection of Transaction objects</returns>
        public static ObservableCollection<Transaction> LoadTransactions()
        {
            if (string.IsNullOrEmpty(AccountContext.CurrentUsername))
                return new ObservableCollection<Transaction>();

            string json = Preferences.Get($"{TransactionsPrefix}{AccountContext.CurrentUsername}", "[]");
            var list = JsonConvert.DeserializeObject<List<Transaction>>(json) ?? new List<Transaction>();
            return new ObservableCollection<Transaction>(list);
        }

        /// <summary>
        /// Delete all persisted data for specified username
        /// 删除指定用户名的所有持久化数据
        /// </summary>
        /// <param name="username">Username to delete data for</param>
        public static void DeleteAccountData(string username)
        {
            Preferences.Remove($"{TotalAmountPrefix}{username}");
            Preferences.Remove($"{BillsPrefix}{username}");
            Preferences.Remove($"{TransactionsPrefix}{username}");
        }
    }
}