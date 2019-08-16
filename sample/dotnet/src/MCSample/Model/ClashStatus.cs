namespace MCSample.Model
{
    public enum ClashStatus
    {
        /// <summary>
        /// The clash is new
        /// </summary>
        New,

        /// <summary>
        /// The clash is pre-existing
        /// </summary>
        Existing,

        /// <summary>
        /// The clash has been opened again
        /// </summary>
        Reopened,

        /// <summary>
        /// The clash has been resolved
        /// </summary>
        Resolved
    }
}
