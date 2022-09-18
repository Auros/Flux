using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly IPublisher<AvatarLoadingFailedContext> _loadingFailedPublisher = null!;

        [Inject]
        private readonly IPublisher<AvatarLoadingStartedContext> _loadingStartedPublisher = null!;
        
        [Inject]
        private readonly IPublisher<AvatarLoadingFinishedContext> _loadingFinishedPublisher = null!;

        [SerializeField, Min(0.01f)]
        private float _loadTimeout = 10f;
        
        private RuntimeOnlyAwaitCaller? _awaiter;
        private CancellationTokenSource? _cancelLoadToken;

        /// <summary>
        /// The avatar 
        /// </summary>
        public RuntimeGltfInstance? Avatar { get; private set; }

        public bool IsLoading => _cancelLoadToken is not null && !_cancelLoadToken.IsCancellationRequested;

        /// <summary>
        /// Starts loading for an avatar at a path.
        /// </summary>
        /// <param name="path">The path to load the avatar VRM data from.</param>
        public void Load(string path)
        {
            UniTask.Void(() => LoadAsync(path));
        }

        private async UniTaskVoid LoadAsync(string path)
        {
            // Switch to main thread so unity objects can be properly created and events can be dispatched on
            // the main thread
            await UniTask.SwitchToMainThread();
            
            // Cancel anything that may be loading at the moment.
            Clear();
            Cancel();
            
            // Create the awaiter if it doesn't exist
            _awaiter = new RuntimeOnlyAwaitCaller();
            
            // Build the avatar load task and run it.
            var loadTask = VrmUtility.LoadAsync(path, _awaiter).AsUniTask();
            
            
            try
            {
                // Send the "started loading" event
                _loadingStartedPublisher.Publish(new AvatarLoadingStartedContext(path));
                
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
                _loadingFinishedPublisher.Publish(ctx);
            }
            catch (Exception e)
            {
                var reasonType = e switch
                {
                    TaskCanceledException => AvatarLoadingFailedContext.ReasonType.TimedOut,
                    OperationCanceledException => AvatarLoadingFailedContext.ReasonType.Canceled,
                    _ => AvatarLoadingFailedContext.ReasonType.Other
                };
                _loadingFailedPublisher.Publish(new AvatarLoadingFailedContext(path, reasonType, e.GetType().FullName));
            }
            finally
            {
                // Remove the handle of the token and awaiter
                _cancelLoadToken = null;
                _awaiter = null;
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