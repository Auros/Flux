using UnityEngine;

namespace Flux.UI.Utilities
{
    public sealed class ApplicationQuitExposer : MonoBehaviour
    {
        public void Quit() => Application.Quit();
    }
}