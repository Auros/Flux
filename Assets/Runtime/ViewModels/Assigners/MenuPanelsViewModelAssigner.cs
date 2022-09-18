using VContainer;

namespace Flux.ViewModels.Assigners
{
    public class MenuPanelsViewModelAssigner : ViewModelAssigner
    {
        [Inject]
        private readonly MenuPanelsViewModel _menuPanelsViewModel = null!;

        private void Start() => Assign(_menuPanelsViewModel);
    }
}