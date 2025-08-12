using UnityEditor;
using UnityEngine;


public class CustomImageImporter : AssetPostprocessor
{
    /*private void OnPreprocessTexture()
    {
        if(assetPath.EndsWith(".png") || assetPath.EndsWith(".jpg"))
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spritePixelsPerUnit = 100; 
            textureImporter.wrapMode = TextureWrapMode.Repeat;
            textureImporter.filterMode = FilterMode.Point;
            textureImporter.mipmapEnabled = true;

            int maxTextureSize = 256;

            string fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath).ToLower();
            if (fileName.Contains("_128"))
            {
                maxTextureSize = 128;
            }
            else if (fileName.Contains("_512"))
            {
                maxTextureSize = 512;
            }

            textureImporter.maxTextureSize = maxTextureSize;
        }
    }*/
}
