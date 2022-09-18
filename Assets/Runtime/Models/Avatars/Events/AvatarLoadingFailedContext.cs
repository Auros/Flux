namespace Flux.Models.Avatars.Events
{
    public class AvatarLoadingFailedContext
    {
        /// <summary>
        /// The source, often the file path, of the avatar that failed to load.
        /// </summary>
        public string Source { get; }
        
        /// <summary>
        /// The reason for failure.
        /// </summary>
        public ReasonType Reason { get; }
        
        /// <summary>
        /// Text representing why the reason failed. If not explictly defined, this can often
        /// be the type name of the exception thrown.
        /// </summary>
        public string ReasonText { get; }
        
        public AvatarLoadingFailedContext(string source, ReasonType reason, string reasonText)
        {
            Source = source;
            Reason = reason;
            ReasonText = reasonText;
        }
        
        public enum ReasonType
        {
            Canceled,
            TimedOut,
            FileNotFound,
            InvalidFile,
            Other
        }
    }
}