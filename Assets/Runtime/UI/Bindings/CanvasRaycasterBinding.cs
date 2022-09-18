using Elysium.Components;
using UnityEngine;

namespace Flux.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class CanvasRaycasterBinding : ComponentPropertyBinding
    {
        private CanvasGroup _canvasGroup = null!;

        protected override void Start()
        {
            base.Start();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public override void OnValueChanged(object? value)
        {
            if (value is not bool valueAsBool)
                return;

            _canvasGroup.blocksRaycasts = valueAsBool;
        }
    }
}