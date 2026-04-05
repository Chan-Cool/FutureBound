namespace FutureBound.Data
{
    /// <summary>
    /// Static class for managing current user context
    /// 静态类，用于管理当前用户上下文
    /// </summary>
    public static class AccountContext
    {
        /// <summary>
        /// Current logged-in username
        /// 当前登录的用户名
        /// </summary>
        public static string CurrentUsername { get; set; } = string.Empty;

        /// <summary>
        /// Clear current user context (logout)
        /// 清除当前用户上下文（登出）
        /// </summary>
        public static void Clear() => CurrentUsername = string.Empty;
    }
}