using Flux.Input;
using Flux.Input.StateComponents;
using Flux.Systems.Avatars;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Flux.Scopes
{
    public sealed class FluxLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private AvatarController _avatarController = null!;
        
        [SerializeField]
        private FluxInputController _fluxInputController = null!;

        [SerializeField]
        private MouseContainController _mouseContainController = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            // Avatar service registration 
            builder.RegisterInstance(_avatarController);
            
            // Input service registration
            builder.RegisterInstance(_fluxInputController.FluxInput);
            builder.RegisterComponent(_mouseContainController);
            
            // MessagePipe registration
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