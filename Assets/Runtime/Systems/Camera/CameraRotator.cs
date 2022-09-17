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
        private CameraTarget _target = null!;

        private GameObject? _targetCalculatorParent;
        private GameObject? _targetCalculatorChild;

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

            delta *= Time.deltaTime;
            delta *= -1f;

            var basePosition = transform.position;
            var targetPosition = _target.transform.position;
            
            // Calculate the distance to the rotation center (target)
            // Used as the radius to calculate the sphere to move along.
            var radius = Vector3.Distance(basePosition, targetPosition);

            if (_targetCalculatorParent == null)
                _targetCalculatorParent = new GameObject("Target Calculator (Parent)");

            if (_targetCalculatorChild == null)
            {
                _targetCalculatorChild = new GameObject("Target Calculator (Child)");
                _targetCalculatorChild.transform.SetParent(_targetCalculatorParent.transform);
            }

            if (_newCompute == null)
                _newCompute = new GameObject("Computed Camera Position");

            _targetCalculatorParent.transform.position = basePosition;
            _targetCalculatorParent.transform.LookAt(targetPosition);
            _targetCalculatorChild.transform.localPosition = delta;

            var unProjectedPosition = _targetCalculatorChild.transform.position;

            var tcpt = _targetCalculatorParent.transform;
            tcpt.transform.position = targetPosition;
            tcpt.transform.LookAt(unProjectedPosition);
            var computedCameraPosition = tcpt.position + tcpt.forward * radius;
            
            transform.position = computedCameraPosition;
            transform.LookAt(_target.transform);

            var screenWidth = Screen.width;
            var screenHeight = Screen.height;
            
            var mousePos = Mouse.current.position;
            var mouseX = mousePos.x.ReadValue();
            var mouseY = mousePos.y.ReadValue();
            if (mouseX > screenWidth)
                Mouse.current.WarpCursorPosition(new Vector2(0, mouseY));
            else if (mouseX < 0)
                Mouse.current.WarpCursorPosition(new Vector2(screenWidth, mouseY));
            
            if (mouseY > screenHeight)
                Mouse.current.WarpCursorPosition(new Vector2(mouseX, 0));
            else if (mouseY < 0)
                Mouse.current.WarpCursorPosition(new Vector2(mouseX, screenHeight));
        }
    }
}