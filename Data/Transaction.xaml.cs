using Newtonsoft.Json;

namespace FutureBound.Data
{
    /// <summary>
    /// Transaction model class (only annotated fields are serialized)
    /// 交易记录模型类（仅标注的字段会被序列化）
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]  // 只序列化标注的字段
    public partial class Transaction
    {
        /// <summary>
        /// Transaction icon identifier
        /// 交易图标标识
        /// </summary>
        [JsonProperty] public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// Transaction remark/description
        /// 交易备注/描述
        /// </summary>
        [JsonProperty] public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// Transaction time (format: yyyy-MM-dd HH:mm:ss)
        /// 交易时间（格式：年-月-日 时:分:秒）
        /// </summary>
        [JsonProperty] public string Time { get; set; } = string.Empty;

        /// <summary>
        /// Transaction amount (string format for serialization)
        /// 交易金额（字符串格式用于序列化）
        /// </summary>
        [JsonProperty] public string Amount { get; set; } = string.Empty;

        /// <summary>
        /// Indicate if transaction is income (true) or expense (false)
        /// 标识交易是否为收入（true）或支出（false）
        /// </summary>
        [JsonProperty] public bool IsIncome { get; set; }
    }
}