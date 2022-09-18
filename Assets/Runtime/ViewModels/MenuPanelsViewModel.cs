using System;
using System.Windows.Input;
using Elysium;
using Flux.Models.Avatars.Events;
using Flux.Systems.Avatars;
using JetBrains.Annotations;
using MessagePipe;
using PropertyChanged.SourceGenerator;
using VContainer.Unity;

namespace Flux.ViewModels
{
    [PublicAPI]
    public sealed partial class MenuPanelsViewModel : IStartable, IDisposable
    {
        [Notify]
        private Panel _activePanel;

        public ICommand CloseAllPanelsCommand { get; }
        public ICommand OpenLoadPanelCommand { get; }
        public ICommand OpenInfoPanelCommand { get; }
        public ICommand OpenModelPanelCommand { get; }
        public ICommand OpenOptionsPanelCommand { get; }
        public ICommand OpenQuitPanelCommand { get; }

        private IDisposable? _disposable;
        private ISubscriber<AvatarClearedContext> _avatarCleared;

        public MenuPanelsViewModel(ISubscriber<AvatarClearedContext> avatarCleared, AvatarController avatarController)
        {
            _avatarCleared = avatarCleared;
            
            CloseAllPanelsCommand = new RelayCommand(() => ActivePanel = Panel.None, null);
            OpenLoadPanelCommand = new RelayCommand(() => ActivePanel = Panel.Load, null);
            OpenInfoPanelCommand = new RelayCommand(() => ActivePanel = Panel.Info, null);
            OpenModelPanelCommand = new RelayCommand(() => ActivePanel = Panel.Model, () => avatarController.Avatar);
            OpenOptionsPanelCommand = new RelayCommand(() => ActivePanel = Panel.Options, null);
            OpenQuitPanelCommand = new RelayCommand(() => ActivePanel = Panel.Quit, null);
        }
        
        public void Start() => _disposable = _avatarCleared.Subscribe(AvatarCleared);
        
        public void Dispose() => _disposable?.Dispose();
        
        private void AvatarCleared(AvatarClearedContext ctx)
        {
            if (ActivePanel is not Panel.Model)
                return;
            
            ActivePanel = Panel.None;
        }
        
        public enum Panel
        {
            None,
            Load,
            Info,
            Model,
            Options,
            Quit
        }
    }
}