using ElRaccoone.Tweens;
using ElRaccoone.Tweens.Core;
using UnityEngine;

namespace Runtime.UI.Animated
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TweenableCanvasGroup : MonoBehaviour
    {
        [SerializeField, Range(0, 1)]
        private float _inactive;

        [SerializeField, Range(0, 1)]
        private float _active = 1f;

        [SerializeField, Min(0)]
        private float _duration = 0.5f;

        [SerializeField]
        private EaseType _easeType = EaseType.CubicOut;

        [field: SerializeField]
        public bool Activated { get; set; }

        private bool _valueLastFrame;
        private Tween<float>? _activeTween;
        private CanvasGroup _canvasGroup = null!;
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            
            // Set the initial canvas state based on the serialized value in the component.
            _canvasGroup.alpha = Activated ? _active : _inactive;
            _valueLastFrame = Activated;
        }

        private void Update()
        {
            // If the state of the component hasn't changed, skip over for this frame.
            if (_valueLastFrame == Activated)
                return;

            // Cancel the tween if one is already running;
            if (_activeTween != null)
                _activeTween.Cancel();
            
            _activeTween = _canvasGroup
                .TweenCanvasGroupAlpha(Activated ? _active : _inactive, _duration)
                .SetEase(_easeType);
            
            // Set the value of the last frame.
            _valueLastFrame = Activated;
        }
    }
}