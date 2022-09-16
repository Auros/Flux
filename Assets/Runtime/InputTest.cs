using Flux.Input;
using UnityEngine;

namespace Flux
{
    public class InputTest : MonoBehaviour
    {
        private FluxInput _fluxInput;

        private void Awake()
        {
            _fluxInput = new FluxInput();
        }

        private void Start()
        {
            _fluxInput.Camera.Enable();
        }

        private void Update()
        {
            var value = _fluxInput.Camera.Zoom.ReadValue<float>();
            if (Mathf.Approximately(value, 0f))
                return;
            
            print(value);
        }

        private void OnDestroy()
        {
            _fluxInput.Camera.Disable();
        }
    }
}