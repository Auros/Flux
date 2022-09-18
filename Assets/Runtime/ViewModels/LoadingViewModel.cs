using System;
using System.IO;
using System.Windows.Input;
using Cysharp.Threading.Tasks;
using Elysium;
using Flux.Models.Avatars.Events;
using Flux.Systems.Avatars;
using MessagePipe;
using PropertyChanged.SourceGenerator;
using UnityEngine;
using VContainer.Unity;

namespace Flux.ViewModels
{
    public sealed partial class LoadingViewModel : IStartable, IDisposable
    {
        [Notify]
        private bool _loading;

        [Notify]
        private ICommand? _cancelCommand;

        [Notify]
        private Color _color = Color.white;
        
        [Notify]
        private string _subText = "Subtext";

        [Notify]
        private string _mainText = "Loading";

        private readonly Color _neutral = Color.white;
        private readonly Color _success = new(0.54f, 1f, 0.54f);
        private readonly Color _failed = new(1f, 0.7f, 0.7f);
        
        private IDisposable? _disposer;
        private readonly ISubscriber<AvatarLoadingFailedContext> _avatarFailed;
        private readonly ISubscriber<AvatarLoadingFinishedContext> _avatarLoaded;
        private readonly ISubscriber<AvatarLoadingStartedContext> _avatarLoading;

        public LoadingViewModel(
            AvatarController avatarController,
            ISubscriber<AvatarLoadingFailedContext> avatarFailed,
            ISubscriber<AvatarLoadingFinishedContext> avatarLoaded,
            ISubscriber<AvatarLoadingStartedContext> avatarLoading
        )
        {
            _avatarFailed = avatarFailed;
            _avatarLoaded = avatarLoaded;
            _avatarLoading = avatarLoading;
            CancelCommand = new RelayCommand(avatarController.Cancel, () => avatarController.IsLoading);
        }
        
        public void Start()
        {
            var builder = DisposableBag.CreateBuilder();
            _avatarFailed.Subscribe(AvatarFailed).AddTo(builder);
            _avatarLoaded.Subscribe(AvatarLoaded).AddTo(builder);
            _avatarLoading.Subscribe(AvatarLoading).AddTo(builder);
            _disposer = builder.Build();
        }

        public void Dispose() => _disposer?.Dispose();

        private void AvatarLoading(AvatarLoadingStartedContext ctx)
        {
            Loading = true;
            Color = _neutral;
            MainText = "Loading";
            SubText = Path.GetFileName(ctx.Source);
        }

        private void AvatarLoaded(AvatarLoadingFinishedContext ctx)
        {
            UniTask.Void(async () =>
            {
                Color = _success;
                MainText = "Loaded";
                await UniTask.Delay(1000);
                Loading = false;
            });
        }

        private void AvatarFailed(AvatarLoadingFailedContext ctx)
        {
            UniTask.Void(async () =>
            {
                Color = _failed;
                MainText = "Failed";
                SubText = ctx.Reason switch
                {
                    AvatarLoadingFailedContext.ReasonType.Canceled => nameof(AvatarLoadingFailedContext.ReasonType.Canceled),
                    AvatarLoadingFailedContext.ReasonType.TimedOut => "Timed Out (Took Too Long)",
                    _ => ctx.ReasonText
                };
                await UniTask.Delay(1000);
                Loading = false;
            });
        }
    }
}