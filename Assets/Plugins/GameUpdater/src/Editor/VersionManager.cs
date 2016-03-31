using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using LitJson;
using UnityEditor;


internal class VersionManager
{

	public VersionManager ()
	{

	}



	public void CreateVersionFile (List<BundleInfo> bundles, bool copyToStreamingAssets = false)
	{
		string destPath = copyToStreamingAssets
			? Config.resourcesPath + "/" + Config.bundleRelativePath
            : "AssetBundles/" + Config.platform + "/" + Config.bundleRelativePath;
		VersionConfig vc = new VersionConfig ();
		vc.versionNum = DateTime.Now.ToString ();
		vc.bundleRelativePath = Config.bundleRelativePath;

//		vc.bundles = bundles;
		foreach (var item in bundles) {
			if (!item.isExist () || copyToStreamingAssets) {
				vc.bundles.Add (item);
			}
		}
        
		string verJson = JsonMapper.ToJson (vc);

//		File.WriteAllText (destPath + "/" + Config.versionFileName + Config.suffix, verJson);
	}



}


