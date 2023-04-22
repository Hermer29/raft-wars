using System.Collections.Generic;
using System.Linq;
using Infrastructure.Platforms;
using LanguageChanger;
using RaftWars.Infrastructure.AssetManagement;
using RaftWars.Infrastructure.Services;
using SpecialPlatforms;
using UnityEngine;
using static UnityEngine.Object;

namespace RaftWars.Infrastructure
{
    public class PlatformsFactory
    {
        private readonly SaveService _saveService;
        private readonly PlatformsLoader _loader;

        public PlatformsFactory(SaveService saveService, PlatformsLoader loader)
        {
            _saveService = saveService;
            _loader = loader;
        }

        public IEnumerable<SpecialPlatform> CreatePlatforms()
        {
            var platforms = _loader.LoadPlatforms();
            var specialPlatforms = platforms as SpecialPlatform[] ?? platforms.ToArray();
            foreach (SpecialPlatform platform in specialPlatforms)
            {
                _saveService.Bind(platform);
            }
            return specialPlatforms;
        }

        public PlatformsMenu CreatePlatformsMenu()
        {
            PlatformsMenu platformMenu = Instantiate(_loader.LoadPlatformsMenu());
            platformMenu.Construct(this, 
                Game.AdverisingService, 
                Game.PropertyService, 
                AllServices.GetSingle<OwningSequence<SpecialPlatform>>(),
                CrossLevelServices.LevelService,
                Game.MoneyService,
                AllServices.GetSingle<IEnumerable<SpecialPlatform>>());
            return platformMenu;
        }

        public PlatformEntry CreatePlatformEntry(Transform parent)
        {
            PlatformEntry instance = Instantiate(_loader.LoadPlatformEntry(), parent, false);
            instance.Construct(AllServices.GetSingle<DescriptionProvider>());
            return instance;
        }
    }
}