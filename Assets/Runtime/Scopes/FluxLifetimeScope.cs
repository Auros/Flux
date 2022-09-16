using Flux.Input;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Flux.Scopes
{
    public sealed class FluxLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private FluxInputController _fluxInputController = null!;
        
        protected override void Configure(IContainerBuilder builder)
        {
            // Input registration
            builder.RegisterInstance(_fluxInputController.FluxInput);
        }
    }
}