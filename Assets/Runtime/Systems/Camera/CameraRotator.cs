using Flux.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Flux.Systems.Camera
{
    public class CameraRotator : MonoBehaviour
    {
        [Inject]
        private FluxInput _fluxInput = null!;

        [SerializeField]
        private Transform? _calculationContainer;
        
        [SerializeField]
        private CameraTarget _target = null!;

        [SerializeField]
        private float _speed = 1f;

        private Transform? _targetCalculatorParent;
        private Transform? _targetCalculatorChild;

        private GameObject? _newCompute;
        
        private void Update()
        {
            // If the user isn't holding down the rotation trigger,
            // don't do anything.
            if (!Mathf.Approximately(_fluxInput.Camera.Rotate.ReadValue<float>(), 1f))
                return;
            
            // If the rotation delta hasn't changed, we don't need to move the camera.
            var delta = _fluxInput.Camera.Rotation.ReadValue<Vector2>();
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

            if (_newCompute == null)
                _newCompute = new GameObject("Computed Camera Position");

            _targetCalculatorParent.transform.position = basePosition;
            _targetCalculatorParent.transform.LookAt(targetPosition);
            _targetCalculatorChild.transform.localPosition = delta;

            var unProjectedPosition = _targetCalculatorChild.position;

            _targetCalculatorParent.position = targetPosition;
            _targetCalculatorParent.LookAt(unProjectedPosition);
            var computedCameraPosition = _targetCalculatorParent.position + _targetCalculatorParent.forward * radius;
            
            transform.position = computedCameraPosition;
            transform.LookAt(_target.transform);

            // Get the current screen position
            var screenWidth = Screen.width;
            var screenHeight = Screen.height;
            
            // Get the current mouse position
            var mousePos = Mouse.current.position;
            var mouseX = mousePos.x.ReadValue();
            var mouseY = mousePos.y.ReadValue();
            
            // If the mouse exits the X bounds of the screen, teleport it to the other side.
            if (mouseX > screenWidth)
                Mouse.current.WarpCursorPosition(new Vector2(0, mouseY));
            else if (mouseX < 0)
                Mouse.current.WarpCursorPosition(new Vector2(screenWidth, mouseY));
            
            // If the mouse exits the Y bounds of the screen, teleport it to the other side.
            if (mouseY > screenHeight)
                Mouse.current.WarpCursorPosition(new Vector2(mouseX, 0));
            else if (mouseY < 0)
                Mouse.current.WarpCursorPosition(new Vector2(mouseX, screenHeight));
        }
    }
}