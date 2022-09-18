using JetBrains.Annotations;
using UniGLTF;

namespace Flux.Models.Avatars.Events
{
    [PublicAPI]
    public class AvatarLoadingFinishedContext
    {
        /// <summary>
        /// The source of the avatar, usually a file path.
        /// </summary>
        public string Source { get; }
        
        /// <summary>
        /// The time it took to load this avatar, in seconds.
        /// </summary>
        public double LoadTime { get; }
        
        /// <summary>
        /// The loaded avatar.
        /// </summary>
        public RuntimeGltfInstance Avatar { get; }

        public AvatarLoadingFinishedContext(string source, double loadTime, RuntimeGltfInstance avatar)
        {
            Source = source;
            LoadTime = loadTime;
            Avatar = avatar;
        }
    }
}