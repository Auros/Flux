using Elysium;
using UnityEngine;

namespace Flux
{
    [RequireComponent(typeof(ViewModelDefinition))]
    public abstract class ViewModelAssigner : MonoBehaviour
    {
        protected ViewModelDefinition _viewModelDefinition = null!;

        private void Awake() => _viewModelDefinition = GetComponent<ViewModelDefinition>();

        protected void Assign(object viewModel) => _viewModelDefinition.ViewModel = viewModel;
    }
}