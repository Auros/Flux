using VContainer;

namespace Flux.ViewModels.Assigners
{
    public class LoadingViewModelAssigner : ViewModelAssigner
    {
        [Inject]
        private readonly LoadingViewModel _loadingViewModel = null!;

        private void Start() => Assign(_loadingViewModel);
    }
}