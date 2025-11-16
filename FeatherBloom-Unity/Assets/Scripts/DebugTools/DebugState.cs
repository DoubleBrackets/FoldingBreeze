namespace DebugTools
{
    /// <summary>
    ///     Stores debug state (in between for editor window and debug service)
    /// </summary>
    public class DebugState
    {
        public static bool QuickArduinoConnect;
        public static bool DoNotLoadMapOnStart { get; set; }
    }
}