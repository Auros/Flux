using Flux.Input;
using Flux.Input.StateComponents;
using Flux.Models.Avatars.Events;
using Flux.Systems.Avatars;
using Flux.ViewModels;
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
            
            RegisterViewModels(builder);
        }

        private static void RegisterMessageBrokers(IContainerBuilder builder, MessagePipeOptions options)
        {
            void WithType<T>() => builder.RegisterMessageBroker<T>(options);
            // void WithKeyedType<TKey, TValue>() => builder.RegisterMessageBroker<TKey, TValue>(options);

            WithType<AvatarClearedContext>();
            WithType<AvatarLoadingFailedContext>();
            WithType<AvatarLoadingStartedContext>();
            WithType<AvatarLoadingFinishedContext>();
        }

        private static void RegisterViewModels(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<LoadViewModel>().AsSelf();
            builder.RegisterEntryPoint<LoadingViewModel>().AsSelf();
            builder.RegisterEntryPoint<MenuPanelsViewModel>().AsSelf();
            builder.RegisterEntryPoint<ActiveModelViewModel>().AsSelf();
        }
    }
}