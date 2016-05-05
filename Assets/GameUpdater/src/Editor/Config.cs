using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


internal class Config
{
    public static string bundleRelativePath = "publish";
    public static string versionFileName = "vc";
    public static string bundlePoolRelativePath = "AssetBundlePool";
	public static TargetPlatform platform = TargetPlatform.StandaloneWindows;
    public static bool compress = true;
    public static string password = "password";
    public static string suffix = ".bytes";
	public static readonly string resourcesPath = Application.dataPath + "/Resources";
}

internal class VersionConfig
{
    public string versionNum;
    public string bundleRelativePath;
	public List<BundleInfo> bundles = new List<BundleInfo>();
}

