using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;
using VContainer;
using VRMShaders;

namespace Flux
{
    public sealed class LoadTest : MonoBehaviour
    {
        /*private async UniTaskVoid Start()
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

        }*/
        
        [Inject]
        private readonly IPublisher<string, int> _publisher = null!;
        
        [Inject]
        private readonly ISubscriber<string, int> _subscriber = null!;

        public void Start()
        {
            var d = DisposableBag.CreateBuilder();
            _subscriber.Subscribe("asd", x => Debug.Log("S1:" + x)).AddTo(d);
            _subscriber.Subscribe("ert", x => Debug.Log("S2:" + x)).AddTo(d);

            _publisher.Publish("asd", 10);
            _publisher.Publish("ert", 20);
            _publisher.Publish("asd" , 30);

            var disposable = d.Build();
            disposable.Dispose();
        }
    }
}