using System.Windows.Input;
using Elysium;
using Flux.Systems.Avatars;
using JetBrains.Annotations;
using PropertyChanged.SourceGenerator;

namespace Flux.ViewModels
{
    public sealed partial class LoadViewModel
    {
        [Notify]
        private string _vrmFilePath = string.Empty;
        
        [UsedImplicitly]
        public ICommand LoadCommand { get; }
        
        public LoadViewModel(AvatarController avatarController)
        {
            LoadCommand = new RelayCommand(() => avatarController.Load(VrmFilePath), () => !avatarController.IsLoading);
        }
    }
}