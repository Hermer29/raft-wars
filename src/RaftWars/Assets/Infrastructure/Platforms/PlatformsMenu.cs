using System;
using System.Collections.Generic;
using System.Linq;
using InputSystem;
using RaftWars.Infrastructure;
using RaftWars.Infrastructure.Services;
using Services;
using SpecialPlatforms;
using UnityEngine;

namespace Infrastructure.Platforms
{
    public class PlatformsMenu : MonoBehaviour
    {
        private PlatformsFactory _factory;
        private AdvertisingService _advertisingService;
        private PropertyService _propertyService;
        private OwningSequence<SpecialPlatform> _owningSequence;
        private LevelService _levelService;
        private PlayerMoneyService _moneyService;
        private List<PlatformsPresenter> _presenters;
        
        [SerializeField] private Transform _platformsParent;
        [SerializeField] private CanvasGroup _group;

        public void Construct(PlatformsFactory factory, AdvertisingService advertisingService, PropertyService propertyService,
            OwningSequence<SpecialPlatform> owningSequence, LevelService levelService, PlayerMoneyService moneyService, IEnumerable<SpecialPlatform> platforms)
        {
            _factory = factory;
            _advertisingService = advertisingService;
            _propertyService = propertyService;
            _owningSequence = owningSequence;
            _levelService = levelService;
            _moneyService = moneyService;

            CreateEntries(platforms);
            HideImmediately();
        }

        private void CreateEntries(IEnumerable<SpecialPlatform> platforms)
        {
            _presenters = new List<PlatformsPresenter>();
            foreach (SpecialPlatform specialPlatform in platforms.OrderBy(x => x.Serial))
            {
                var presenter = new PlatformsPresenter(_factory.CreatePlatformEntry(_platformsParent),
                    specialPlatform,
                    _advertisingService,
                    _propertyService,
                    _owningSequence,
                    _levelService,
                    _moneyService);
                _presenters.Add(presenter);
            }
        }
        
        public void ShowImmediately()
        {
            _group.alpha = 1;
            _group.interactable = true;
            _group.blocksRaycasts = true;
        }

        public void HideImmediately()
        {
            _group.alpha = 0;
            _group.interactable = false;
            _group.blocksRaycasts = false;
        }

        private void OnDestroy()
        {
            foreach (PlatformsPresenter platformsPresenter in _presenters)
            {
                platformsPresenter.Dispose();
            }
        }
    }
}