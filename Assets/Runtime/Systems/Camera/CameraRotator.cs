using Flux.Input;
using Flux.Input.StateComponents;
using UnityEngine;
using VContainer;

namespace Flux.Systems.Camera
{
    public sealed class CameraRotator : MonoBehaviour
    {
        [Inject]
        private FluxInput _fluxInput = null!;

        [Inject]
        private MouseContainController _mouseContainController = null!;

        [SerializeField]
        private Transform? _calculationContainer;
        
        [SerializeField]
        private CameraTarget _target = null!;

        [SerializeField]
        private float _speed = 1f;

        private Transform? _targetCalculatorParent;
        private Transform? _targetCalculatorChild;
        
        private void Update()
        {
            // If the user isn't holding down the rotation trigger,
            // don't do anything.
            if (!Mathf.Approximately(_fluxInput.Camera.Move.ReadValue<float>(), 1f))
                return;

            // If we are using the shift key, disable the rotator.
            if (Mathf.Approximately(_fluxInput.Camera.Shift.ReadValue<float>(), 1f))
                return;
            
            // If the rotation delta hasn't changed, we don't need to move the camera.
            var delta = _fluxInput.Camera.Movement.ReadValue<Vector2>();
            if (delta == default)
                return;

            // Multiply by deltaTime to make frame independent, then set the speed of movement;
            delta *= Time.deltaTime * _speed;
            
            // Invert the delta to move the camera the opposite way of the user dragging.
            delta *= -1f;

            var basePosition = transform.position;
            var targetPosition = _target.transform.position;
            
            // Calculate the distance to the rotation center (target)
            // Used as the radius to calculate the sphere to move along.
            var radius = Vector3.Distance(basePosition, targetPosition);

            if (_targetCalculatorParent == null)
            {
                _targetCalculatorParent = new GameObject("Target Calculator (Parent)").transform;
                _targetCalculatorParent.SetParent(_calculationContainer);
            }

            if (_targetCalculatorChild == null)
            {
                _targetCalculatorChild = new GameObject("Target Calculator (Child)").transform;
                _targetCalculatorChild.SetParent(_targetCalculatorParent.transform);
            }

            _targetCalculatorParent.transform.position = basePosition;
            _targetCalculatorParent.transform.LookAt(targetPosition);
            _targetCalculatorChild.transform.localPosition = delta;

            var unProjectedPosition = _targetCalculatorChild.position;

            _targetCalculatorParent.position = targetPosition;
            _targetCalculatorParent.LookAt(unProjectedPosition);
            var computedCameraPosition = _targetCalculatorParent.position + _targetCalculatorParent.forward * radius;
            
            transform.position = computedCameraPosition;
            transform.LookAt(_target.transform);

            // Lock the mouse within the bounds of the screen.
            _mouseContainController.LockThisFrame();
        }
    }
}