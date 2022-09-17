using System;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using Flux.Models.Avatars.Events;
using MessagePipe;
using UniGLTF;
using UnityEngine;
using VContainer;
using VRM;
using VRMShaders;

namespace Flux.Systems.Avatars
{
    public sealed class AvatarController : MonoBehaviour
    {
        [Inject]
        private readonly IPublisher<AvatarLoadingFinishedContext> _avatarLoadingFinishedContextPublisher = null!;

        [SerializeField, Min(0.01f)]
        private float _loadTimeout = 10f;
        
        private RuntimeOnlyAwaitCaller? _awaiter;
        private CancellationTokenSource? _cancelLoadToken;

        /// <summary>
        /// The avatar 
        /// </summary>
        public RuntimeGltfInstance? Avatar { get; private set; }

        /// <summary>
        /// Starts loading for an avatar at a path.
        /// </summary>
        /// <param name="path">The path to load the avatar VRM data from.</param>
        public void Load(string path)
        {
            // Cancel anything that may be loading at the moment.
            Clear();
            Cancel();
            
            // Create the awaiter if it doesn't exist
            _awaiter ??= new RuntimeOnlyAwaitCaller();
            
            // Build the avatar load task and run it.
            var loadTask = VrmUtility.LoadAsync(path, _awaiter).AsUniTask();
            UniTask.RunOnThreadPool(() => LoadAsync(path, loadTask));
        }

        private async UniTaskVoid LoadAsync(string path, UniTask<RuntimeGltfInstance> loadTask)
        {
            try
            {
                // Create the stopwatch
                var sw = Stopwatch.StartNew();
                
                // Create the timeout cancellation token source
                //   Our _loadTimeout variable is in seconds to be consistent with Unity,
                //   so we convert it to milliseconds my multiplying it and casting it to an int.
                using CancellationTokenSource cts = new((int)(_loadTimeout * 1000f));
                _cancelLoadToken = cts;

                // We run on the thread pool again to easily pass in our cancellation token.
                Avatar = await UniTask.RunOnThreadPool(() => loadTask, cancellationToken: cts.Token);

                // Make the avatar visible.
                Avatar.ShowMeshes();

                // Finish timing the avatar loading sequence.
                sw.Stop();
                
                var ctx = new AvatarLoadingFinishedContext(path, sw.Elapsed.TotalSeconds, Avatar);
                _avatarLoadingFinishedContextPublisher.Publish(ctx);
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
            Avatar = null;
        }
    }
}