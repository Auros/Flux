using JetBrains.Annotations;

namespace Flux.Models.Avatars.Events
{
    [PublicAPI]
    public class AvatarLoadingStartedContext
    {
        /// <summary>
        /// The source of the avatar, usually a file path.
        /// </summary>
        public string Source { get; set; }

        public AvatarLoadingStartedContext(string source)
        {
            Source = source;
        }
    }
}