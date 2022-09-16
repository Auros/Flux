using Flux.Input;
using UnityEngine;
using VContainer;

namespace Flux.Systems.Camera
{
    public sealed class CameraZoomer : MonoBehaviour
    {
        [Inject]
        private FluxInput _fluxInput = null!;
        
        [SerializeField]
        private float _speed = 1f;

        [SerializeField, Min(0.01f)]
        private float _minimumDistance = 0.1f;
        
        [SerializeField]
        private CameraTarget _target = null!;
        
        private void Update()
        {
            // Read the value of the camera zoom delta.
            // Basically, the scroll wheel on mouse.
            var value = _fluxInput.Camera.Zoom.ReadValue<float>();
            
            // If the value is zero, don't do anything.
            // We only wnat to move the camera when the scroll delta changes.
            if (Mathf.Approximately(value, 0f))
                return;

            // Multiply the value of the scroll, the speed of our zoom and the time delta to make it frame independent.
            value = value * _speed * Time.deltaTime;

            var targetPosition = _target.transform.position;
            
            // Calculate the new position.
            var newPosition = Vector3.MoveTowards(transform.position, targetPosition, value);

            // Void check, ensure that we haven't passed the minimum distance
            var distance = Vector3.Distance(targetPosition, newPosition);
            if (_minimumDistance >= distance)
                return;
            
            // Set the new position to ourself (camera, presumably)
            transform.position = newPosition;
        }
    }
}