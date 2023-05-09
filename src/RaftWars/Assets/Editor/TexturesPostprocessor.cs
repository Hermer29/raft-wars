using System;
using System.Text.RegularExpressions;
using UnityEditor;

public class TexturesPostprocessor : AssetPostprocessor
{ 
    private void OnPreprocessTexture()
    {
        TextureImporter importer = (TextureImporter)assetImporter;
        string importerName = importer.assetPath;
        const string importerIgnorePattern = @".{0,}_ii.{0,}";
        
        if (Regex.IsMatch(importerName, importerIgnorePattern))
            return;
        importer.textureCompression = TextureImporterCompression.Compressed;
        importer.maxTextureSize = 256;
    }
}