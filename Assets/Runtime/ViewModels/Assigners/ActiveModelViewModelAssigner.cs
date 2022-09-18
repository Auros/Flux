using VContainer;

namespace Flux.ViewModels.Assigners
{
    public class ActiveModelViewModelAssigner : ViewModelAssigner
    {
        [Inject]
        private readonly ActiveModelViewModel _activeModelViewModel = null!;

        private void Start() => Assign(_activeModelViewModel);
    }
}