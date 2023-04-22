using System.Collections.Generic;
using Infrastructure.Platforms;
using SpecialPlatforms;
using UnityEngine;

namespace RaftWars.Infrastructure.AssetManagement
{
    public class PlatformsLoader
    {
        public IEnumerable<SpecialPlatform> LoadPlatforms()
        {
            return Resources.LoadAll<SpecialPlatform>(PlatformsConstants.PlatformPath);
        }

        public PlatformEntry LoadPlatformEntry()
        {
            return Resources.Load<PlatformEntry>(PlatformsConstants.PlatformEntryPath);
        }

        public PlatformsMenu LoadPlatformsMenu()
        {
            return Resources.Load<PlatformsMenu>(PlatformsConstants.PlatformUIPath);
        }
    }
}