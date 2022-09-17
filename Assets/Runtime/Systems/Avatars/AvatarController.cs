using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniGLTF;
using UnityEngine;
using VRM;
using VRMShaders;

namespace Flux.Systems.Avatars
{
    public sealed class AvatarController : MonoBehaviour
    {
        [SerializeField, Min(0.01f)]
        private float _loadTimeout = 10f;

        private RuntimeOnlyAwaitCaller? _awaiter;
        private CancellationTokenSource? _cancelLoadToken;

        public RuntimeGltfInstance? Avatar { get; private set; }

        /// <summary>
        /// Starts loading for an avatar at a path.
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path)
        {
            Clear();
            Cancel();
            _awaiter ??= new RuntimeOnlyAwaitCaller();
            var loadTask = VrmUtility.LoadAsync(path, _awaiter).AsUniTask();
            UniTask.RunOnThreadPool(() => LoadAsync(loadTask));
        }

        private async UniTaskVoid LoadAsync(UniTask<RuntimeGltfInstance> loadTask)
        {
            try
            {
                // Create the timeout cancellation token source
                //   Our _loadTimeout variable is in seconds to be consistent with Unity,
                //   so we convert it to milliseconds my multiplying it and casting it to an int.
                using CancellationTokenSource cts = new((int)(_loadTimeout * 1000f));
                _cancelLoadToken = cts;

                // We run on the thread pool again to easily pass in our cancellation token.
                Avatar = await UniTask.RunOnThreadPool(() => loadTask, cancellationToken: cts.Token);

                // Make the avatar visible.
                Avatar.ShowMeshes();
            }
            catch (Exception e)
            {
                print(e);
            }
            finally
            {
                // Remove the handle from this token
                _cancelLoadToken = null;
            }
        }
        
        /// <summary>
        /// Cancels the currently loading avatar
        /// </summary>
        public void Cancel()
        {
            _cancelLoadToken?.Cancel();
        }

        /// <summary>
        /// Clears the loaded avatar
        /// </summary>
        public void Clear()
        {
            // If the avatar isn't loaded, do nothing.
            if (Avatar == null)
                return;
            
            // Calling dispose is a shortcut for destroying the avatar : D
            Avatar.Dispose();
        }
    }
}