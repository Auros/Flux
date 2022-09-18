using Elysium.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Flux.UI.Bindings
{
    [RequireComponent(typeof(RawImage))]
    public class RawImageTextureBinding : ComponentPropertyBinding
    {
        private RawImage _rawImage = null!;
        
        protected override void Start()
        {
            base.Start();
            _rawImage = GetComponent<RawImage>();
        }

        public override void OnValueChanged(object? value)
        {
            if (value is not Texture2D valueAsTexture)
                return;

            _rawImage.texture = valueAsTexture;
        }
    }
}