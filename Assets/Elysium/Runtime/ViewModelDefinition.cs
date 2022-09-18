using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Elysium.Components;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Elysium
{
    /// <summary>
    /// Acts as a bridge to define a view model.
    /// </summary>
    [PublicAPI, ExecuteAlways, DisallowMultipleComponent]
    public class ViewModelDefinition : NotifiableBehaviour
    {
        private object? _viewModel;
        private readonly List<CommandContext> _commandContexts = new();
        private readonly List<ComponentPropertyBinding> _bindings = new();

        [field: SerializeField]
        internal Object? ViewModelObject { get; private set; }
        
        /// <summary>
        /// The View Model in this definition.
        /// </summary>
        public object? ViewModel
        {
            // We check the unity object reference first to ensure that the ViewModel hasn't been destroyed.
            get => ViewModelObject != null ? ViewModelObject : _viewModel;
            set
            {
                if (!SetField(ref _viewModel, value))
                    return;

                ViewModelObject = value switch
                {
                    // If we have successfully set the variable, update the ViewModelObject so it can get serialized 
                    // properly.
                    Object valueAsUnityObject => valueAsUnityObject,
                    // However if the value is null, we also want to make sure the serialized field is updated
                    null => null,
                    _ => ViewModelObject
                };
            }
        }

        private void Start()
        {
            // Gets the bindings scoped to this view definition.
            var componentPropertyBindings = GetScopedBindings(gameObject);
            foreach (var binding in componentPropertyBindings)
                BindingRegistered(this, binding);
            
            ElysiumBindings.OnBindingRegistered += BindingRegistered;
            ElysiumBindings.OnBindingUnregistered += BindingUnregistered;
            
            // Send some property change events to start the ViewModel events in case it was assigned via serialization
            OnPropertyChanging(nameof(ViewModel));
            OnPropertyChanged(nameof(ViewModel));
        }

        private void Update()
        {
            foreach (var ctx in _commandContexts)
            {
                // If the command is null, we skip over it.
                if (ctx.Command == null)
                    continue;
                
                foreach (var binding in _bindings)
                {
                    // If the binding name doesn't match, continue the search
                    if (binding.Name != ctx.Name)
                        continue;
                    
                    // Set the interactability of the binding to the can execute value.
                    binding.SetInteraction(ctx.Command.CanExecute(null));
                }
            }
        }

        private void OnDestroy()
        {
            ElysiumBindings.OnBindingUnregistered -= BindingUnregistered;
            ElysiumBindings.OnBindingRegistered -= BindingRegistered; 
        }
        
        private void BindingRegistered(ViewModelDefinition definition, ComponentPropertyBinding binding)
        {
            // Make sure the binding is relevant to us.
            if (definition != this)
                return;
            
            // Add the binding to our internal list.
            _bindings.Add(binding);
            
            // Set the initial value if applicable
            if (ViewModel != null)
                binding.OnValueChanged(ElysiumInvoker.GetPropertyValue(ViewModel, binding.Name));
        }

        private void BindingUnregistered(ViewModelDefinition definition, ComponentPropertyBinding binding)
        {
            // Make sure the binding is relevant to us.
            if (definition != this)
                return;
            
            // Remove the binding from our internal list
            _bindings.Remove(binding);
        }

        protected override void OnPropertyChanging(string? propertyName = null)
        {
            // Ensure we're working with the right property.
            if (propertyName != nameof(ViewModel))
                return;

            // We can only unsubscribe from property changes if the view model supports it.
            if (ViewModel is not INotifyPropertyChanged propertyChanger)
                return;

            // Unsubscribe us from the property changed event.
            propertyChanger.PropertyChanged -= ViewModel_PropertyChanged;
        }

        protected override void OnPropertyChanged(string? propertyName = null)
        {
            // Ensure we're working with the right property.
            if (propertyName != nameof(ViewModel))
                return;

            _commandContexts.Clear();
            foreach (var property in ElysiumInvoker.GetPropertyNamesOfType<ICommand>(ViewModel?.GetType()))
                _commandContexts.Add(new CommandContext(property, (ElysiumInvoker.GetPropertyValue(ViewModel, property) as ICommand)!));
            
            // We can only listen into property changes if the view model supports it.
            if (ViewModel is not INotifyPropertyChanged propertyChanger)
                return;

            // Subscribe us to the property changed event.
            propertyChanger.PropertyChanged += ViewModel_PropertyChanged;
            
            // Set the initial values
            foreach (var binding in _bindings)
                binding.OnValueChanged(ElysiumInvoker.GetPropertyValue(ViewModel, binding.Name));
        }
        
        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ViewModel == null)
                return;
            
            foreach (var binding in _bindings)
            {
                // Skip over the bindings that don't match this property update.
                if (binding.Name != e.PropertyName)
                    continue;

                var value = ElysiumInvoker.GetPropertyValue(ViewModel, e.PropertyName);
                var ctx = _commandContexts.FirstOrDefault(c => c.Name == e.PropertyName);
                if (ctx is not null)
                    ctx.Command = value as ICommand;

                binding.OnValueChanged(value);
            }
        }

        /// <summary>
        /// Sets a property on the current View Model
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The value to set the property to.</param>
        public void SetPropertyOnViewModel(string propertyName, object value)
            => ElysiumInvoker.SetPropertyValue(ViewModel, propertyName, value);

        public void SendCommandEvent(string propertyName)
        {
            // Search for the command from what we know about the View Model.
            var command = _commandContexts.FirstOrDefault(c => c.Name == propertyName)?.Command;
            if (command is not null)
            {
                command.Execute(null);
                return;
            }

            // If not, try to find a parameterless method.
            ElysiumInvoker.ParameterlessMethod(ViewModel, propertyName);
        }
        
        /// <summary>
        /// Gets the bindings relevantly scoped to a GameObject
        /// </summary>
        /// <param name="gameObject">The GameObject to search on.</param>
        /// <returns>The scoped bindings from the GameObject</returns>
        protected static List<ComponentPropertyBinding> GetScopedBindings(GameObject gameObject)
        {
            List<ComponentPropertyBinding> contexts = new();
            
            var localRecurses = LocalRecurseUntilComponentWithType(GetDirectChildren(gameObject), typeof(ViewModelDefinition));
            localRecurses.Add(gameObject);
            
            foreach (var local in localRecurses)
            {
                var components = local.GetComponents<ComponentPropertyBinding>();

                foreach (var component in components)
                {
                    if (!component || !component.isActiveAndEnabled)
                        continue;
                
                    contexts.Add(component);   
                }
            }
            return contexts;
        }

        /// <summary>
        /// Gets every GameObject within the children of a GameObject until a specific type is seen, ending that branch
        /// to search on.
        /// </summary>
        /// <param name="roots">The original roots.</param>
        /// <param name="type">The type to end a branch search on.</param>
        /// <returns>The relevant GameObjects in the search.</returns>
        private static List<GameObject> LocalRecurseUntilComponentWithType(IEnumerable<GameObject> roots, Type type)
        {
            List<GameObject> gameObjects = new();
            foreach (var root in roots)
            {
                var component = root.GetComponent(type);
                gameObjects.Add(root);
                
                if (component)
                    continue;

                var directChildren = GetDirectChildren(root);
                var localRecurses = LocalRecurseUntilComponentWithType(directChildren, type);
                gameObjects.AddRange(localRecurses);
            }
            return gameObjects;
        }
        
        /// <summary>
        /// Gets the GameObjects of the direct children of a GameObject 
        /// </summary>
        /// <param name="gameObject">The GameObject to get the children of.</param>
        /// <returns>The children of the GameObject.</returns>
        private static IEnumerable<GameObject> GetDirectChildren(GameObject gameObject)
        {
            var gameObjects = new GameObject[gameObject.transform.childCount];
            for (int i = 0; i < gameObjects.Length; i++)
                gameObjects[i] = gameObject.transform.GetChild(i).gameObject;
            return gameObjects;
        }

        private class CommandContext
        {
            public string Name { get; }
            public ICommand? Command { get; set; }

            public CommandContext(string name, ICommand? command)
            {
                Name = name;
                Command = command;
            }
        }
    }
}