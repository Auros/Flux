using Flux.Systems.Avatars;
using UnityEngine;
using VContainer;

namespace Flux.Runtime.Utilities
{
    public sealed class LoadAvatarOnStart : MonoBehaviour
    {
        [Inject]
        private readonly AvatarController _avatarController = null!;

        [SerializeField]
        private string _path = string.Empty;

        private void Start()
        {
            _avatarController.Load(_path);
        }
    }
}