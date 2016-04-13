using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using System;
using Object = UnityEngine.Object;
using wuxingogo.Tools;

namespace wuxingogo.bundle
{
    public delegate void AllUpdated( VersionConfig config );

	public sealed class BundleManager : MonoBehaviour
	{
		#region BundleManager
		public static string suffix = ".bytes";
		public static string password = "password";

		private static BundleManager _instance = null;

        public event AllUpdated OnAllUpdated;

		public static BundleManager GetInstance()
		{
			if(_instance == null)
			{
				_instance = Object.FindObjectOfType<BundleManager> ();
				if (_instance != null)
					return _instance;
				
				GameObject go = new GameObject ("BundleManager");
				DontDestroyOnLoad (go);
				go.hideFlags = HideFlags.DontSaveInEditor;
				_instance = go.AddComponent<BundleManager> ();
			}
			return _instance;
		}

		Dictionary<string, AssetBundle> assetBundleCache = new Dictionary<string, AssetBundle>(); 


		public VersionConfig LocalVersionConfig{
			get{
				if(localVersionConfig == null)
					localVersionConfig = BundleLoader.LoadLocalVersion (BundleConfig.bundleRelativePath + "/" + BundleConfig.versionFileName + BundleConfig.suffix);
				return localVersionConfig;
			}
		}

		public void LoadFromWWW(string remoteVersionFileURL)
		{
			WWW www = new WWW(remoteVersionFileURL);
			StartCoroutine("LoadRemoteVersionFileHandler", www);
		}

		IEnumerator LoadRemoteVersionFileHandler(WWW www)
		{
			yield return www;
			if (www.error == null){
				GetAssetPoolBundles (www.bytes);
			}else{
                OnAllUpdated( LocalVersionConfig );
            }
            
        }
		void GetAssetPoolBundles (byte[] memory)
		{

			using (MemoryStream memoryStream = new MemoryStream())
			{
				byte[] length = null;
				int offset = System.Runtime.InteropServices.Marshal.SizeOf (typeof(int));
				memoryStream.Write (memory, 0, offset);
				length = memoryStream.ToArray ();
				var versionLength = BitConverter.ToInt32 (length, 0);


				memoryStream.Position = 0;
				memoryStream.Write (memory, offset, versionLength);
				byte[] versionByte = memoryStream.ToArray ();
				VersionConfig diffVersion = null;
				using (MemoryStream versionStream = new MemoryStream(versionByte))
				{
					string versionContent = "";
					StreamUtils.Read (versionStream, out versionContent);
					diffVersion = JsonMapper.ToObject <VersionConfig>(versionContent);
				}

				offset = offset + versionByte.Length;
				int limit = memory.Length - offset;

				memoryStream.Position = 0;
				memoryStream.Write (memory, offset, limit);
				byte[] buffer = memoryStream.ToArray ();
				int index = 0;
				int count = 0;
				foreach (var item in diffVersion.bundles) {
					Debug.Log ("Item is : " + item.name);
					count = (int)item.size;
					using (MemoryStream bundleStream = new MemoryStream()){
						bundleStream.Write ( buffer, index,  count);
						File.WriteAllBytes (Application.temporaryCachePath + "/" + BundleConfig.bundleRelativePath + "/" + item.name + BundleConfig.suffix, bundleStream.ToArray ());
					}
					index += count;
				}

				UpdateLocalVersionConfig (diffVersion, Application.temporaryCachePath + "/" + BundleConfig.bundleRelativePath + "/" + BundleConfig.versionFileName + BundleConfig.suffix);

			}

		}
		public void UpdateLocalVersionConfig(VersionConfig remoteBundle, string savePath)
		{      
			for (int i = 0; i < remoteBundle.bundles.Count; i++) {
				var bundleInfo = remoteBundle.bundles [i];
				bool isNew = true;
				for (int j = 0; j < LocalVersionConfig.bundles.Count; j++) {
					var localBundle = LocalVersionConfig.bundles [j];
					if(bundleInfo.name == localBundle.name){
						isNew = false;
						if (localBundle.md5 != bundleInfo.md5) {
                            LocalVersionConfig.bundles [j] = bundleInfo;
							break;
						}
					}
				}
				if(isNew)
				{
                    LocalVersionConfig.bundles.Add (bundleInfo);
				}
			}
			File.WriteAllText(savePath, JsonMapper.ToJson( LocalVersionConfig ) );
        }

		private VersionConfig localVersionConfig;

		public AssetBundle LoadAssetBundle(string bundleName, bool cache = true)
		{
			if (assetBundleCache.ContainsKey(bundleName))
			{
				return assetBundleCache[bundleName];
			}
			AssetBundle bundle = null;

			var memory = BundleLoader.LoadFileMemory ( LocalVersionConfig.bundleRelativePath + "/" + bundleName + BundleConfig.suffix);

			using (var bundleStream = BundleEncode.DeompressAndDecryptLZMA(memory, password))
			{
				if (bundleStream == null)
					return bundle;
				bundle = AssetBundle.CreateFromMemoryImmediate(bundleStream.ToArray());
				if (bundle == null)		// 如果没有则直接返回
					return bundle;
			}
			if (cache)
				assetBundleCache.Add(bundleName, bundle);
			return bundle;
		}
		#endregion
	}
}