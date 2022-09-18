using ElRaccoone.Tweens;
using ElRaccoone.Tweens.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Flux.UI
{
    [RequireComponent(typeof(Graphic))]
    public class TweenableGraphicColor : MonoBehaviour
    {
        [SerializeField]
        private EaseType _easeType = EaseType.ExpoOut;
        
        [SerializeField, Min(0)]
        private float _duration = 0.5f;
        
        [field: SerializeField]
        public Color Color { get; set; }
        
        private Color _valueLastFrame;
        private Graphic _graphic = null!;
        private Tween<Color>? _activeTween;
        
        private void Awake()
        {
            _graphic = GetComponent<Graphic>();
            
            // Set the initial graphic color based on the serialized value in the component.
            _graphic.color = Color;
            _valueLastFrame = Color;
        }

        private void Update()
        {
            if (_valueLastFrame == Color)
                return;
            
            if (_activeTween != null)
                _activeTween.Cancel();

            _activeTween = _graphic
                .TweenGraphicColor(Color, _duration)
                .SetEase(_easeType);

            _valueLastFrame = Color;
        }
    }
}
