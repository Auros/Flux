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

            CloseAllPanelsCommand = CreateTransitionCommand(Panel.None);
            OpenLoadPanelCommand = CreateTransitionCommand(Panel.Load);
            OpenInfoPanelCommand = CreateTransitionCommand(Panel.Info);
            OpenModelPanelCommand = CreateTransitionCommand(Panel.Model, () => avatarController.Avatar);
            OpenOptionsPanelCommand = CreateTransitionCommand(Panel.Options);
            OpenQuitPanelCommand = CreateTransitionCommand(Panel.Quit);
        }

        private ICommand CreateTransitionCommand(Panel panel, Func<bool>? canExecute = null)
            => new RelayCommand(() => ActivePanel = ActivePanel == panel ? Panel.None : panel, canExecute);

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