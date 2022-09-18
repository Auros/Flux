using System.Windows.Input;
using Elysium;
using Flux.Systems.Avatars;
using JetBrains.Annotations;
using PropertyChanged.SourceGenerator;

namespace Flux.ViewModels
{
    [PublicAPI]
    public sealed partial class MenuPanelsViewModel
    {
        [Notify]
        private Panel _activePanel;

        public ICommand CloseAllPanelsCommand { get; }
        public ICommand OpenLoadPanelCommand { get; }
        public ICommand OpenInfoPanelCommand { get; }
        public ICommand OpenModelPanelCommand { get; }
        public ICommand OpenOptionsPanelCommand { get; }
        public ICommand OpenQuitPanelCommand { get; }
        
        
        public MenuPanelsViewModel(AvatarController avatarController)
        {
            CloseAllPanelsCommand = new RelayCommand(() => ActivePanel = Panel.None, null);
            OpenLoadPanelCommand = new RelayCommand(() => ActivePanel = Panel.Load, null);
            OpenInfoPanelCommand = new RelayCommand(() => ActivePanel = Panel.Info, null);
            OpenModelPanelCommand = new RelayCommand(() => ActivePanel = Panel.Model, () => avatarController.Avatar);
            OpenOptionsPanelCommand = new RelayCommand(() => ActivePanel = Panel.Options, null);
            OpenQuitPanelCommand = new RelayCommand(() => ActivePanel = Panel.Quit, null);
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