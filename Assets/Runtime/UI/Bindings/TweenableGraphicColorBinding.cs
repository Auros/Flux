using Elysium.Components;
using UnityEngine;

namespace Flux.UI.Bindings
{
    [RequireComponent(typeof(TweenableGraphicColor))]
    public class TweenableGraphicColorBinding : ComponentPropertyBinding
    {
        private TweenableGraphicColor _tweenableGraphicColor = null!;

        protected override void Start()
        {
            base.Start();
            _tweenableGraphicColor = GetComponent<TweenableGraphicColor>();
        }

        public override void OnValueChanged(object? value)
        {
            if (value is not Color valueAsColor)
                return;

            _tweenableGraphicColor.Color = valueAsColor;
        }
    }
}