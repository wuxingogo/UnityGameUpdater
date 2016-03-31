using System.Collections.Generic;
using System.Collections;
namespace wuxingogo.bundle
{
	[System.Serializable]
	public class VersionConfig
	{
		public string versionNum = "";
		public string bundleRelativePath = "";
		public List<BundleInfo> bundles = new List<BundleInfo>();
	}

	[System.Serializable]
	public class BundleInfo
	{
		public string name = "";
		public string md5 = "";
		public uint size = 0;
		public string[] include = new string[0];
		public string[] dependency = new string[0];

		public bool isExist()
		{
			var existBundles = BundleManager.GetInstance ().LocalVersionConfig.bundles;
			foreach( var item in existBundles ) {
				if( item.name == name && item.md5 == md5 )
					return true;
			}
			return false;
		}

		public BundleInfo()
		{
			
		}
		#if UNITY_EDITOR
		public BundleInfo(string name)
		{
			this.name = name;
			Update ();
		}
		void Update()
		{
			UpdateInclude ();
			UpdateDependency ();
		}

		void UpdateInclude()
		{
			include = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle( name );
		}

		void UpdateDependency()
		{
			dependency = UnityEditor.AssetDatabase.GetDependencies( UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle( name ) );
		}
		#endif
	}
}