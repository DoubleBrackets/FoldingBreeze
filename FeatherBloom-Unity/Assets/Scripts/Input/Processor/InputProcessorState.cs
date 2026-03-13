namespace Input.Processor
{
    public enum InputProcessorState
    {
        /// <summary>
        ///     The used input is tracking the device input
        /// </summary>
        Tracking,

        /// <summary>
        ///     The used input is not tracking the device input since it has gone outside the bounds
        /// </summary>
        Untracked
    }
}