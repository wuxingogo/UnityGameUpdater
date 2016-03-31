using UnityEngine;
using System.Collections;
using UnityEditor;

internal class GameUpdatePoster : AssetPostprocessor
{

    public static BundleManager bm;

    void OnPostprocessAssetbundleNameChanged(string filePath, string oldBundleName, string newBundleName)
    {
        
    }
}
