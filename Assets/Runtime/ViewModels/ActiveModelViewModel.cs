using System;
using System.Linq;
using System.Windows.Input;
using Elysium;
using Flux.Models.Avatars.Events;
using MessagePipe;
using PropertyChanged.SourceGenerator;
using UnityEngine;
using VContainer.Unity;
using VRM;

namespace Flux.ViewModels
{
    public sealed partial class ActiveModelViewModel : IStartable, IDisposable
    {
        [Notify]
        private string _modelName = string.Empty;

        [Notify]
        private Texture2D? _thumbnail;

        [Notify]
        private string _triangles = string.Empty;

        [Notify]
        private string _materials = string.Empty;

        [Notify]
        private string _skinnedMeshes = string.Empty;

        [Notify]
        private string _animations = string.Empty;

        [Notify]
        private string _blendshapes = string.Empty;
        
        [Notify]
        private string _author = string.Empty;

        [Notify]
        private string _version = string.Empty;

        [Notify]
        private string _reference = string.Empty;

        [Notify]
        private string _contactInformation = string.Empty;

        [Notify]
        private string _allowedUser = string.Empty;

        [Notify]
        private string _violence = string.Empty;

        [Notify]
        private string _sexual = string.Empty;

        [Notify]
        private string _commercial = string.Empty;

        [Notify]
        private ICommand? _openAdditionalPermissionInfo;

        [Notify]
        private ICommand? _openAdditionalLicenseInfo;

        [Notify]
        private string _license = string.Empty;

        [Notify]
        private bool _hasAdditionalInfo;
        
        private const string _positive = "#8aff8a";
        private const string _neutral = "#c9460e";
        private const string _negative = "#ff3030";
        private const string _unknown = "#777777";

        private IDisposable? _disposer;
        private readonly ISubscriber<AvatarClearedContext> _avatarCleared;
        private readonly ISubscriber<AvatarLoadingFinishedContext> _avatarLoaded;

        public ActiveModelViewModel(
            ISubscriber<AvatarClearedContext> avatarCleared,
            ISubscriber<AvatarLoadingFinishedContext> avatarLoaded)
        {
            _avatarCleared = avatarCleared;
            _avatarLoaded = avatarLoaded;
        }

        public void Start()
        {
            var builder = DisposableBag.CreateBuilder();
            _avatarCleared.Subscribe(AvatarCleared).AddTo(builder);
            _avatarLoaded.Subscribe(AvatarLoaded).AddTo(builder);
            _disposer = builder.Build();
        }

        public void Dispose() => _disposer?.Dispose();
        
        private void AvatarLoaded(AvatarLoadingFinishedContext ctx)
        {
            string MakeInfoTemplate(string? value, string propertyName = "unknown")
                => $"{propertyName} - <color=#cfcfcf>{(string.IsNullOrWhiteSpace(value) ? "Not Provided" : value)}</color>";

            var avatar = ctx.Avatar;
            var metadata = ctx.Metadata;
            
            ModelName = metadata.Title;
            Thumbnail = metadata.Thumbnail;

            Triangles = MakeInfoTemplate(avatar.Meshes.Sum(m => m.triangles.Length).ToString(), nameof(Triangles));
            Materials = MakeInfoTemplate(avatar.Materials.Count.ToString(), nameof(Materials));
            SkinnedMeshes = MakeInfoTemplate(avatar.SkinnedMeshRenderers.Count.ToString(), "Skinned Meshes");
            Animations = MakeInfoTemplate(avatar.AnimationClips.Count.ToString(), nameof(Animations));
            Blendshapes = MakeInfoTemplate(avatar.Meshes.Sum(m => m.blendShapeCount).ToString(), nameof(Blendshapes));

            Author = MakeInfoTemplate(metadata.Author, nameof(Author));
            Version = MakeInfoTemplate(metadata.Version, nameof(Version));
            Reference = MakeInfoTemplate(metadata.Reference, nameof(Reference));
            ContactInformation = MakeInfoTemplate(metadata.ContactInformation, "Contact Information");

            var allowedUserInfo = metadata.AllowedUser switch
            {
                VRM.AllowedUser.Everyone => (_positive, nameof(VRM.AllowedUser.Everyone)),
                VRM.AllowedUser.ExplicitlyLicensedPerson => (_neutral, "Explicitly Licensed To Person"),
                VRM.AllowedUser.OnlyAuthor => (_negative, "Only Author"),
                _ => (_unknown, metadata.AllowedUser.ToString())
            };

            (string, string) GetImprovedUsageInfo(UssageLicense license) => license switch
            {
                UssageLicense.Disallow => (_negative, "Disallowed"),
                UssageLicense.Allow => (_positive, "Allowed"),
                _ => (_unknown, license.ToString())
            };

            var violenceInfo = GetImprovedUsageInfo(metadata.ViolentUssage);
            var sexualInfo = GetImprovedUsageInfo(metadata.SexualUssage);
            var commercialInfo = GetImprovedUsageInfo(metadata.CommercialUssage);
            
            AllowedUser = $"Allowed To Use - <color={allowedUserInfo.Item1}>{allowedUserInfo.Item2}</color>";
            Violence =
                $"Use For Violent Content - <color={violenceInfo.Item1}>{violenceInfo.Item2}</color>";
            Sexual =
                $"Use For Sexual Content - <color={sexualInfo.Item1}>{sexualInfo.Item2}</color>";
            Commercial =
                $"Commercial Usage - <color={commercialInfo.Item1}>{commercialInfo.Item2}";
            
            OpenAdditionalPermissionInfo = string.IsNullOrWhiteSpace(metadata.OtherPermissionUrl) ? null : new RelayCommand(
                () => Application.OpenURL(metadata.OtherPermissionUrl), 
                () => !string.IsNullOrWhiteSpace(metadata.OtherPermissionUrl));

            OpenAdditionalLicenseInfo = metadata.LicenseType is not LicenseType.Other ? null : new RelayCommand(
                () => Application.OpenURL(metadata.OtherLicenseUrl), 
                () => metadata.LicenseType is LicenseType.Other && !string.IsNullOrWhiteSpace(metadata.OtherLicenseUrl));
            
            HasAdditionalInfo = OpenAdditionalPermissionInfo is not null || OpenAdditionalLicenseInfo is not null;

            License = $"License - <color={_neutral}>{metadata.LicenseType.ToString().Replace("_", " ")}</color>";
        }

        private void AvatarCleared(AvatarClearedContext ctx)
        {
            _ = true;
        }
    }
}