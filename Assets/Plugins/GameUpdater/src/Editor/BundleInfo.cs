using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using LitJson;
using System.Collections.Generic;


internal class BundleInfo
{
	public string name;
	private string _name;
	public string md5;
	public uint size;
	public string[] include;
	public string[] dependency;

	public BundleInfo( )
	{
	}

	public BundleInfo(string name)
	{
		this.name = name;
		Update();
	}

	public void Update()
	{
//		UpdateSize();
//		UpdateCRC();
		UpdateInclude();
		UpdateDependency();
	}

	void UpdateSize()
	{
		string path = Config.bundlePoolRelativePath + "/" + Config.platform  + "/" + name;
		
		if( File.Exists( path ) ) {
			FileInfo fi = new FileInfo( path );
			size = ( uint )fi.Length;
		} else {
				size = 0;
		}

	}

	void UpdateCRC()
	{
//        BuildPipeline.GetCRCForAssetBundle(Config.bundlePoolRelativePath + "/" + name, out md5);
	}

	void UpdateInclude()
	{
		include = AssetDatabase.GetAssetPathsFromAssetBundle( name );
	}

	void UpdateDependency()
	{
		dependency = AssetDatabase.GetDependencies( AssetDatabase.GetAssetPathsFromAssetBundle( name ) );
	}

	public bool isExist()
	{
		string fileName = Config.bundleRelativePath + "/" + Config.versionFileName;
		string temporaryPath = Application.temporaryCachePath + "/" + fileName + Config.suffix;

		List<BundleInfo> existBundles = new List<BundleInfo>();
		if(File.Exists(temporaryPath))
		{
			string content = File.ReadAllText( temporaryPath );
			VersionConfig localVersionConfig =
				JsonMapper.ToObject<VersionConfig>(
					content);

			existBundles = localVersionConfig.bundles;
		}else{
			string content = Resources.Load<TextAsset>(fileName).text;
			VersionConfig localVersionConfig =
				JsonMapper.ToObject<VersionConfig>(
					content);

			existBundles = localVersionConfig.bundles;
		}


		foreach( var item in existBundles ) {
			if( item.name == name && item.md5 == md5 )
				return true;
		}
		return false;
	}
}


