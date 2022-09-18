using Elysium.Components;
using Runtime.UI.Animated;
using UnityEngine;

namespace Flux.UI
{
    [RequireComponent(typeof(TweenableCanvasGroup))]
    public sealed class TweenableCanvasGroupBinding : ComponentPropertyBinding
    {
        private TweenableCanvasGroup _tweenableCanvasGroup = null!;

        protected override void Start()
        {
            base.Start();
            _tweenableCanvasGroup = GetComponent<TweenableCanvasGroup>();
        }

        public override void OnValueChanged(object? value)
        {
            if (value is not bool valueAsBool)
                return;

            _tweenableCanvasGroup.Activated = valueAsBool;
        }
    }
}