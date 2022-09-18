using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Flux.UI.Windows
{
    public class MoveableWindow : MonoBehaviour, IDragHandler
    {
        private Canvas? _canvas;

        [SerializeField]
        private RectTransform _moveableRect = null!;
        
        public void OnDrag(PointerEventData eventData)
        {
            var mousePos = Mouse.current.position;
            var mouseX = mousePos.x.ReadValue();
            var mouseY = mousePos.y.ReadValue();

            var screenWidth = Screen.width;
            var screenHeight = Screen.height;
            
            // While being dragged, if the mouse goes off screen, force it back in.
            
            if (IsOutsideOf(0f, screenWidth, mouseX))
                Mouse.current.WarpCursorPosition(new Vector2(ClosestToValue(0, screenWidth, mouseX), mouseY));
            
            if (IsOutsideOf(0f, screenHeight, mouseY))
                Mouse.current.WarpCursorPosition(new Vector2(mouseX, ClosestToValue(0, screenHeight, mouseY)));
            
            var scaleFactor = _canvas != null ? _canvas.scaleFactor : 1f;
            _moveableRect.anchoredPosition += eventData.delta / scaleFactor;
        }

        private void Awake() => AcquireCanvas();

        // If this component's parent have changed, ensure that we are on the most recent canvas.
        private void OnTransformParentChanged() => AcquireCanvas();

        private void AcquireCanvas() => _canvas = GetComponentInParent<Canvas>();

        private static bool IsOutsideOf(float min, float max, float value) => value > max || value < min;

        private static float ClosestToValue(float a, float b, float value) => Mathf.Abs(value - a) >= Mathf.Abs(value - b) ? b : a;
    }
}