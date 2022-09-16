using UnityEngine;
using VContainer;

namespace Flux.Input.StateComponents
{
    public sealed class CameraInputController : MonoBehaviour
    {
        [Inject]
        private FluxInput _fluxInput = null!;

        private void Start()
        {
            _fluxInput.Enable();
        }

        private void OnEnable()
        {
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            // Injection might not occur when OnEnable is called.
            _fluxInput?.Enable();
        }

        private void OnDisable()
        {
            _fluxInput.Disable();
        }
    }
}