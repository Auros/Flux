using Cysharp.Threading.Tasks;
using Flux.Systems.Avatars;
using UnityEngine;
using VContainer;

namespace Flux
{
    public sealed class LoadTest : MonoBehaviour
    {
        [Inject]
        private readonly AvatarController _avatarController = null!;
        
        public async UniTaskVoid Start()
        {
            _avatarController.Load(@"P:\Avatars\@Stores\Fusion Street.vrm");

            await UniTask.Delay(5000);
            
            _avatarController.Clear();

            /*
            var d = DisposableBag.CreateBuilder();
            _subscriber.Subscribe("asd", x => Debug.Log("S1:" + x)).AddTo(d);
            _subscriber.Subscribe("ert", x => Debug.Log("S2:" + x)).AddTo(d);

            _publisher.Publish("asd", 10);
            _publisher.Publish("ert", 20);
            _publisher.Publish("asd" , 30);

            var disposable = d.Build();
            disposable.Dispose();*/
        }
    }
}