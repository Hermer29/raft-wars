using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEditor.BuildTarget;

namespace Editor
{
    public static class Builder
    {
        [MenuItem("Builds/🕸Build WebGL")]
        public static void BuildWebGl()
        {
            const BuildTarget Platform = WebGL;
            
            PredefinePlatformSpecificSettings(Platform);
            
            BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                locationPathName = $"../../artifacts/{DateTime.Today:d}_{PlayerSettings.productName}_{DateTime.Now.Hour}_{DateTime.Now.Minute}/",
                scenes = EditorBuildSettings.scenes.Select(x => x.path).ToArray(),
                target = Platform
            });
        }

        private static void PredefinePlatformSpecificSettings(BuildTarget target)
        {
            if(target == WebGL)
                PlayerSettings.colorSpace = ColorSpace.Gamma;
        }
    }
}