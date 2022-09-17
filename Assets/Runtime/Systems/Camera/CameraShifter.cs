using Flux.Input;
using Flux.Input.StateComponents;
using UnityEngine;
using VContainer;

namespace Flux.Systems.Camera
{
    public class CameraShifter : MonoBehaviour
    {
        [Inject]
        private readonly FluxInput _fluxInput = null!;
        
        [Inject]
        private MouseContainController _mouseContainController = null!;
        
        [SerializeField]
        private float _speed = 1f;

        [SerializeField]
        private Transform _cameraTransform = null!;

        private void Update()
        {
            // If the user isn't holding down the movement trigger,
            // don't do anything.
            if (!Mathf.Approximately(_fluxInput.Camera.Move.ReadValue<float>(), 1f))
                return;
            
            // If the user isn't holding down the shifting trigger,
            // don't do anything.
            if (!Mathf.Approximately(_fluxInput.Camera.Shift.ReadValue<float>(), 1f))
                return;

            // If the movement delta hasn't changed, we don't need to move the camera.
            var delta = _fluxInput.Camera.Movement.ReadValue<Vector2>();
            if (delta == default)
                return;
            
            // Multiply by deltaTime to make frame independent, then set the speed of movement;
            delta *= Time.deltaTime * _speed;
            
            // Invert the delta to move the camera the opposite way of the user dragging.
            delta *= -1f;
            
            // Move the camera
            transform.Translate(_cameraTransform.TransformDirection(new Vector3(delta.x, delta.y, 0f)));
            
            // Lock the mouse within the bounds of the screen.
            _mouseContainController.LockThisFrame();
        }
    }
}