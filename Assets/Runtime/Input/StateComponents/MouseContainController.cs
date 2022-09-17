using UnityEngine;
using UnityEngine.InputSystem;

namespace Flux.Input.StateComponents
{
    /// <summary>
    /// Locks the mouse inside the screen. If the mouse position overflows, it gets teleported to the other end.
    /// </summary>
    public sealed class MouseContainController : MonoBehaviour
    {
        private bool _wantsToUpdate;

        public void LockThisFrame() => _wantsToUpdate = true;
        
        private void LateUpdate()
        {
            if (!_wantsToUpdate)
                return;
            
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
            
            _wantsToUpdate = false;
        }
    }
}