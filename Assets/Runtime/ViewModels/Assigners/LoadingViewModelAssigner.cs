using VContainer;

namespace Flux.ViewModels.Assigners
{
    public sealed class LoadingViewModelAssigner : ViewModelAssigner
    {
        [Inject]
        private readonly LoadingViewModel _loadingViewModel = null!;

        private void Start() => Assign(_loadingViewModel);
    }
}