using System;
using UnityEngine;

namespace Flux.UI.Utilities
{
    public class DeactivateWhenCanvasGroupsAreZero : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup[] _canvasGroups = Array.Empty<CanvasGroup>();

        private void Update()
        {
            foreach (var canvasGroup in _canvasGroups)
                canvasGroup.gameObject.SetActive(!Mathf.Approximately(canvasGroup.alpha, 0));
        }
    }
}