using JetBrains.Annotations;
using UnityEngine;

namespace Elysium.Components
{
    [PublicAPI, ExecuteAlways, DefaultExecutionOrder(-8500)]
    public abstract class ComponentPropertyBinding : MonoBehaviour
    {
        [SerializeField] private string _name = string.Empty;
        
        private ViewModelDefinition? _previousDefinition;

        public string Name => _name;

        public abstract void OnValueChanged(object? value);

        protected virtual void Start() => UpdateDefinition();
        
        protected virtual void OnTransformParentChanged() => UpdateDefinition();

        protected internal virtual void SetInteraction(bool value) { } 

        private void UpdateDefinition()
        {
            var definition = GetComponentInParent<ViewModelDefinition>(true);
            
            // The definition hasn't changed, nothing to do!
            if (definition == _previousDefinition)
                return;
            
            // Tell the view model definitions that this binding no longer is needed.
            if (_previousDefinition != null)
                ElysiumBindings.Remove(_previousDefinition, this);
            
            // Tell the view model definitions that this binding can be registered.
            ElysiumBindings.Add(definition, this);
            _previousDefinition = definition;
        }

        protected void SetValue(object value)
        {
            if (_previousDefinition == null)
                return;
            
            _previousDefinition.SetPropertyOnViewModel(Name, value);
        }

        protected void SendCommand()
        {
            if (_previousDefinition == null)
                return;
            
            _previousDefinition.SendCommandEvent(Name);
        }
    }
}