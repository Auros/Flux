using System;
using Elysium.Components;

namespace Elysium
{
    internal static class ElysiumBindings
    {
        public static event Action<ViewModelDefinition, ComponentPropertyBinding>? OnBindingRegistered; 
        public static event Action<ViewModelDefinition, ComponentPropertyBinding>? OnBindingUnregistered;

        public static void Add(ViewModelDefinition definition, ComponentPropertyBinding binding)
            => OnBindingRegistered?.Invoke(definition, binding);
        
        public static void Remove(ViewModelDefinition definition, ComponentPropertyBinding binding)
            => OnBindingUnregistered?.Invoke(definition, binding);
    }
}