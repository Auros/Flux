using System;
using UnityEngine;

namespace Flux.UI.Bindings
{
    public sealed class EnumTweenableCanvasGroupBinding : TweenableCanvasGroupBinding
    {
        [SerializeField]
        private string _enumValue = string.Empty;
        
        public override void OnValueChanged(object? value)
        {
            if (value is not Enum valueAsEnum)
                return;

            var valueName = Enum.GetName(value.GetType(), Convert.ToInt32(valueAsEnum));
            _tweenableCanvasGroup.Activated = valueName == _enumValue;
        }
    }
}