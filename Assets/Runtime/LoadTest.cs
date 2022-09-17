using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VRMShaders;

namespace Flux
{
    public sealed class LoadTest : MonoBehaviour
    {
        private async UniTaskVoid Start()
        {
            CancellationTokenSource cts = new(10000);
            var avatarTask = VRM.VrmUtility.LoadAsync(@"P:\Avatars\@Stores\Fusion Street.vrm", new RuntimeOnlyAwaitCaller()).AsUniTask();

            try
            {
                var avatar = await UniTask.RunOnThreadPool(() => avatarTask, cancellationToken: cts.Token);
                avatar.ShowMeshes();
            }
            catch (Exception e)
            {
                print(e);
            }

        }
    }
}