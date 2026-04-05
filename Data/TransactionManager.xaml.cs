using System.Collections.ObjectModel;
using System.Linq;

namespace FutureBound.Data
{
    /// <summary>
    /// Singleton class for managing transaction data operations
    /// 单例类，用于管理交易数据操作
    /// </summary>
    public partial class TransactionManager
    {
        // Singleton instance
        // 单例实例
        private static TransactionManager _instance = null!;

        /// <summary>
        /// Get singleton instance of TransactionManager
        /// 获取TransactionManager的单例实例
        /// </summary>
        public static TransactionManager Instance => _instance ??= new TransactionManager();

        /// <summary>
        /// Observable collection of all transactions
        /// 所有交易的可观察集合
        /// </summary>
        public ObservableCollection<Transaction> Transactions { get; set; } = new ObservableCollection<Transaction>();

        /// <summary>
        /// Load transaction records from local storage after user login
        /// 用户登录后从本地存储加载交易记录
        /// </summary>
        public void Initialize()
        {
            var saved = AccountDataManager.LoadTransactions();
            Transactions.Clear();
            foreach (var t in saved) Transactions.Add(t);
        }

        /// <summary>
        /// Get distinct months with transaction records, ordered by newest first
        /// 获取包含交易记录的唯一月份列表，按最新优先排序
        /// </summary>
        /// <returns>List of month strings (format: yyyy-MM)</returns>
        public List<string> GetAvailableMonths()
        {
            return Transactions
                .Select(t => t.Time[..7])
                .Distinct()
                .OrderByDescending(m => m)
                .ToList();
        }

        /// <summary>
        /// Get transactions filtered by specific month, ordered by time (newest first)
        /// 获取指定月份的交易记录，按时间降序排列
        /// </summary>
        /// <param name="month">Month string (format: yyyy-MM)</param>
        /// <returns>List of transactions in the specified month</returns>
        public List<Transaction> GetTransactionsByMonth(string month)
        {
            return Transactions
                .Where(t => t.Time.StartsWith(month))
                .OrderByDescending(t => t.Time)
                .ToList();
        }

        /// <summary>
        /// Add new transaction to collection and persist to local storage immediately
        /// 添加新交易到集合并立即持久化到本地存储
        /// </summary>
        /// <param name="transaction">Transaction object to add</param>
        public void AddTransaction(Transaction transaction)
        {
            Transactions.Insert(0, transaction);
            // Each newly added transaction is persisted immediately.
            AccountDataManager.SaveTransactions(Transactions);
        }
    }
}