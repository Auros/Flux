using VContainer;

namespace Flux.ViewModels.Assigners
{
    public class LoadViewModelAssigner : ViewModelAssigner
    {
        [Inject]
        private readonly LoadViewModel _loadViewModel = null!;

        private void Start() => Assign(_loadViewModel);
    }
}