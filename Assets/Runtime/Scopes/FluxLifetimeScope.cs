using Flux.Input;
using Flux.Input.StateComponents;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Flux.Scopes
{
    public sealed class FluxLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private FluxInputController _fluxInputController = null!;

        [SerializeField]
        private MouseContainController _mouseContainController = null!;
        
        protected override void Configure(IContainerBuilder builder)
        {
            // Input registration
            builder.RegisterInstance(_fluxInputController.FluxInput);
            builder.RegisterComponent(_mouseContainController);
            
            // Message Pipe registration
            var msgPipeOptions = builder.RegisterMessagePipe();
            builder.RegisterBuildCallback(container => GlobalMessagePipe.SetProvider(container.AsServiceProvider()));
            RegisterMessageBrokers(builder, msgPipeOptions);
        }

        private static void RegisterMessageBrokers(IContainerBuilder builder, MessagePipeOptions options)
        {
            void WithType<T>() => builder.RegisterMessageBroker<T>(options);
            void WithKeyedType<TKey, TValue>() => builder.RegisterMessageBroker<TKey, TValue>(options);
            
            WithType<int>();
            WithKeyedType<string, int>();
        }
    }
}