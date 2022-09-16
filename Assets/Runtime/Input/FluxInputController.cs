using UnityEngine;

namespace Flux.Input
{
    public sealed class FluxInputController : MonoBehaviour
    {
        private FluxInput? _fluxInput;

        public FluxInput FluxInput => _fluxInput ??= new FluxInput();

        private void OnDestroy()
        {
            // Release the resources being held by our input action.
            _fluxInput?.Dispose();
        }
    }
}