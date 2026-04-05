using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics;
using Newtonsoft.Json;

namespace FutureBound.Models
{
    /// <summary>
    /// Represents the main bill entity that contains core bill information and related records
    /// Implements INotifyPropertyChanged to support MVVM property change notification
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]  // ✅ Only serialize annotated fields
    public partial class Bill : INotifyPropertyChanged
    {
        // Backing field for CurrentAmount property
        private decimal _currentAmount;

        /// <summary>
        /// Name of the bill
        /// </summary>
        [JsonProperty] public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Type/category of the bill (e.g., Food, Transportation)
        /// </summary>
        [JsonProperty] public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Last modified timestamp of the bill
        /// </summary>
        [JsonProperty] public string LastModifiedTime { get; set; } = string.Empty;

        /// <summary>
        /// Hex color code corresponding to the bill type (e.g., #FFFFFF)
        /// </summary>
        [JsonProperty] public string TypeColorHex { get; set; } = "#FFFFFF";

        /// <summary>
        /// Converted Color object from TypeColorHex for MAUI UI rendering
        /// Ignored in JSON serialization as it's UI-specific
        /// </summary>
        [JsonIgnore]
        public Color TypeColor => Color.FromArgb(TypeColorHex);

        /// <summary>
        /// Logo identifier corresponding to the bill type
        /// </summary>
        [JsonProperty] public string TypeLogo { get; set; } = string.Empty;

        /// <summary>
        /// Current total amount of the bill
        /// Triggers property change notification when value updates
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
        /// Uses ObservableCollection to support UI auto-refresh on collection changes
        /// </summary>
        [JsonProperty] public ObservableCollection<BillRecord> Records { get; set; } = new ObservableCollection<BillRecord>();

        /// <summary>
        /// Event triggered when a property value changes
        /// </summary>
        public new event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event to notify UI of property updates
        /// </summary>
        /// <param name="name">Name of the changed property (auto-filled by CallerMemberName)</param>
        protected void OnPropertyChanged([CallerMemberName] string name = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// Represents a single transaction record under a bill
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]  // ✅ Only serialize annotated fields for BillRecord
    public class BillRecord
    {
        /// <summary>
        /// Monetary amount of the single transaction
        /// </summary>
        [JsonProperty] public decimal Amount { get; set; } = 0;

        /// <summary>
        /// Remarks/description for the transaction (e.g., "Lunch")
        /// </summary>
        [JsonProperty] public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if the amount is a deposit (true) or expenditure (false)
        /// </summary>
        [JsonProperty] public bool IsSave { get; set; } = false;

        /// <summary>
        /// Timestamp when the transaction occurred
        /// </summary>
        [JsonProperty] public string Time { get; set; } = string.Empty;

        /// <summary>
        /// Reference to the parent Bill object
        /// Ignored in JSON serialization to avoid circular references
        /// </summary>
        [JsonIgnore]
        public Bill Bill { get; set; } = null!;

        /// <summary>
        /// Formatted display text for the amount (e.g., "+100" for deposit, "-50" for expenditure)
        /// Calculated property for UI presentation
        /// </summary>
        public string AmountDisplay => IsSave ? $"+{Amount}" : $"-{Amount}";
    }
}