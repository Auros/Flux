using System.Collections.Specialized;
using Elysium.Components;
using UnityEngine;

namespace Elysium.Bindings.Rendering
{
    [RequireComponent(typeof(ObservableCollectionRenderer))]
    public class ObservableCollectionRendererBinding : ComponentPropertyBinding
    {
        private ObservableCollectionRenderer _renderer = null!;

        private void Awake() => _renderer = GetComponent<ObservableCollectionRenderer>();
        
        public override void OnValueChanged(object? value)
        {
            _renderer.SetRenderer(value as INotifyCollectionChanged);
        }
    }
}