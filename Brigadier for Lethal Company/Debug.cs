namespace Rumi.BrigadierForLethalCompany
{
    static class Debug
    {
        public static void Log(object data) => BFLCPlugin.logger?.LogInfo(data);
        public static void LogWarning(object data) => BFLCPlugin.logger?.LogWarning(data);
        public static void LogError(object data) => BFLCPlugin.logger?.LogError(data);
    }
}
