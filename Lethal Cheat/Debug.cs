namespace Rumi.LethalCheat
{
    static class Debug
    {
        public static void Log(object data) => LCheatPlugin.logger?.LogInfo(data);
        public static void LogWarning(object data) => LCheatPlugin.logger?.LogWarning(data);
        public static void LogError(object data) => LCheatPlugin.logger?.LogError(data);
    }
}
