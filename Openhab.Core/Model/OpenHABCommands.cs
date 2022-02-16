namespace OpenHAB.Core.Model
{
    /// <summary>
    /// A list of constants used for sending commands to OpenHAB.
    /// </summary>
    public sealed class OpenHabCommands
    {
        /// <summary>
        /// The command to toggle a switch ON.
        /// </summary>
        public const string OnCommand = "ON";

        /// <summary>
        /// The command to toggle a switch OFF.
        /// </summary>
        public const string OffCommand = "OFF";

        /// <summary>
        /// The command to make a rollershutter go up.
        /// </summary>
        public const string UpCommand = "UP";

        /// <summary>
        /// The command to make a rollershutter go down.
        /// </summary>
        public const string DownCommand = "DOWN";

        /// <summary>
        /// The command to make a rollershutter stop.
        /// </summary>
        public const string StopCommand = "STOP";
    }
}
