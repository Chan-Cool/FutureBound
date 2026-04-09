using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics;
using Newtonsoft.Json;

namespace FutureBound.Models
{
    /// <summary>
    /// Bill entity class that implements the INotifyPropertyChanged interface to support property change notifications
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public partial class Bill : INotifyPropertyChanged
    {
        /// <summary>
        /// Private backing field for the CurrentAmount property
        /// </summary>
        private decimal _currentAmount;

        /// <summary>
        /// Name of the bill (e.g., "Monthly Salary", "Grocery Expense")
        /// </summary>
        [JsonProperty]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Type/category of the bill (e.g., "Food", "Transportation", "Income")
        /// </summary>
        [JsonProperty]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Last modified time of the bill (string format, e.g., "yyyy-MM-dd HH:mm:ss")
        /// </summary>
        [JsonProperty]
        public string LastModifiedTime { get; set; } = string.Empty;

        /// <summary>
        /// Hex color code corresponding to the bill type (e.g., "#FF5733" for food)
        /// </summary>
        [JsonProperty]
        public string TypeColorHex { get; set; } = "#FFFFFF";

        /// <summary>
        /// Color object derived from TypeColorHex (ignored in JSON serialization)
        /// </summary>
        [JsonIgnore]
        public Color TypeColor => Color.FromArgb(TypeColorHex);

        /// <summary>
        /// Logo identifier associated with the bill type (e.g., "icon_food", "icon_salary")
        /// </summary>
        [JsonProperty]
        public string TypeLogo { get; set; } = string.Empty;

        /// <summary>
        /// Event date associated with the bill (string format, e.g., "yyyy-MM-dd")
        /// </summary>
        [JsonProperty]
        public string EventDate { get; set; } = string.Empty;

        /// <summary>
        /// Current total amount of the bill
        /// Triggers PropertyChanged event when updated to refresh UI bindings
        /// </summary>
        [JsonProperty]
        public decimal CurrentAmount
        {
            get => _currentAmount;
            set
            {
                _currentAmount = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Collection of transaction records associated with the bill
        /// ObservableCollection enables automatic UI updates when items are added/removed
        /// </summary>
        [JsonProperty]
        public ObservableCollection<BillRecord> Records { get; set; } = new ObservableCollection<BillRecord>();

        /// <summary>
        /// Overridden property change event to notify bound UI elements of updates
        /// </summary>
        public new event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Triggers the PropertyChanged event for a specified property
        /// </summary>
        /// <param name="name">Name of the property that changed (auto-populated by CallerMemberName)</param>
        protected void OnPropertyChanged([CallerMemberName] string name = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// Bill transaction record entity class
    /// Represents a single income/expense entry for a bill
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BillRecord
    {
        /// <summary>
        /// Monetary amount of the transaction
        /// </summary>
        [JsonProperty]
        public decimal Amount { get; set; } = 0;

        /// <summary>
        /// Additional notes/remarks for the transaction
        /// </summary>
        [JsonProperty]
        public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// Transaction type flag: 
        /// - True = Income (funds added to the bill)
        /// - False = Expense (funds deducted from the bill)
        /// </summary>
        [JsonProperty]
        public bool IsSave { get; set; } = false;

        /// <summary>
        /// Timestamp of the transaction (string format, e.g., "yyyy-MM-dd HH:mm:ss")
        /// </summary>
        [JsonProperty]
        public string Time { get; set; } = string.Empty;

        /// <summary>
        /// Associated bill object (ignored in JSON serialization to avoid circular references)
        /// </summary>
        [JsonIgnore]
        public Bill Bill { get; set; } = null!;

        /// <summary>
        /// Display color for the transaction record:
        /// - LightSkyBlue for income (IsSave = true)
        /// - LightSalmon for expense (IsSave = false)
        /// (Ignored in JSON serialization)
        /// </summary>
        [JsonIgnore]
        public Color RecordColor => IsSave ? Colors.LightSkyBlue : Colors.LightSalmon;

        /// <summary>
        /// Formatted display string for the transaction amount:
        /// - Prepends "+" for income (e.g., "+100.00")
        /// - Prepends "-" for expense (e.g., "-50.00")
        /// </summary>
        public string AmountDisplay => IsSave ? $"+{Amount}" : $"-{Amount}";
    }
}
