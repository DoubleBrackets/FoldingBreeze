using Framework.LevelLoading;

namespace DevTools
{
    /// <summary>
    ///     Stores dev tool state bridging editor window and gameplay
    /// </summary>
    public static class DevToolState
    {
        public static bool QuickArduinoConnect;
        public static bool GoIntoGameplayImmediately;

        /// <summary>
        ///     If true, then the gameplay stages will not be generated (used to test hand placed stages)
        /// </summary>
        public static bool DoNotLoadMapOnStart { get; set; }

        public static bool AutoRestartOnDeath { get; set; }

        public static GameLevelSO OverrideStartupLevel { get; set; }
    }
}