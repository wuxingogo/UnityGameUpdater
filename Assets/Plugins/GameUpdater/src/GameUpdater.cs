using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System.IO;
using Decoder = SevenZip.Compression.LZMA.Decoder;
using Xxtea;

namespace newx
{
    public delegate void OneUpdating(UpdateInfo info);

    public delegate void OneUpdated(UpdateInfo info);

    public delegate void AllUpdated(UpdateInfo info);

    public delegate void OneFailed(UpdateInfo info);
    public class GameUpdater : MonoBehaviour
    {
    	[SerializeField]
        private VersionConfig localVersionConfig;
		[SerializeField]
        private VersionConfig remoteVersionConfig;
		[SerializeField]
        private List<BundleInfo> updateList;
        private string localVersionFileRelativePath;
        private string remoteVersionFileURL;
        public float refreshInterval = 0.2f; 
        public string password = "";
        private static GameUpdater _instance;

        public event OneUpdating OnOneUpdating;
        public event OneUpdated OnOneUpdated;
        public event AllUpdated OnAllUpdated;
        public event OneFailed OnOneFailed;

        public int updatingIndex = 0;
        public string updatingName = "";
        public uint bundleSizeUpdated = 0;
        public uint bundleSize = 0;
        public uint totalSizeUpdated = 0;
        public uint totalSize = 0;
        public int numBundlesUpdated = 0;
        public int numBundles = 0;

        public static string suffix = ".bytes";

        Dictionary<string, AssetBundle> assetBundleCache = new Dictionary<string, AssetBundle>(); 
        private GameUpdater()
        {
            
        }

        public static GameUpdater instance 
        {
            get
            {
                if (_instance == null)
                {
					_instance = UnityEngine.Object.FindObjectOfType<GameUpdater>();
					if( _instance != null )
						return _instance;
                    GameObject go = new GameObject();
                    go.name = "GameUpdater";
                    DontDestroyOnLoad(go);
					_instance = go.AddComponent<GameUpdater>();
                }
                return _instance;
            }
            
        }

        public void JustLoadLocalVersionFile(string localVersionFilePath, string password = "")
        {
            this.password = password;
            LoadLocalVersionFile(localVersionFilePath);
        }

        public void DoUpdate(string localVersionFilePath, string remoteVersionFilePath, string password = "")
        {
            this.password = password;
            LoadLocalVersionFile(localVersionFilePath);
            LoadRemoteVersionFile(remoteVersionFilePath);
        }

        void LoadLocalVersionFile(string relativePath)
        {
            localVersionFileRelativePath = relativePath;
			if (File.Exists(Application.temporaryCachePath + "/" + relativePath + suffix))
            {
                localVersionConfig =
                    JsonMapper.ToObject<VersionConfig>(
                        File.ReadAllText(Application.temporaryCachePath + "/" + relativePath + suffix));
            }
            else
            {
				TextAsset objInResources = Resources.Load(relativePath, typeof(TextAsset)) as TextAsset;
                localVersionConfig =
                    JsonMapper.ToObject<VersionConfig>(objInResources.text);
				Resources.UnloadAsset(objInResources);
            }
            if (!Directory.Exists(Application.temporaryCachePath + "/" + localVersionConfig.bundleRelativePath))
            {
                Directory.CreateDirectory(Application.temporaryCachePath + "/" + localVersionConfig.bundleRelativePath);
            }
        }
        
        void LoadRemoteVersionFile(string url)
        {
            remoteVersionFileURL = url + suffix;
			WWW www = new WWW(remoteVersionFileURL);
			StartCoroutine("LoadRemoteVersionFileHandler", www);
        }

        IEnumerator LoadRemoteVersionFileHandler(WWW www)
        {
            yield return www;
            if (www.error == null)
            {
                remoteVersionConfig = JsonMapper.ToObject<VersionConfig>(www.text);
                if (NeedUpdate())
                {
                    MakeUpdateList();
                    UpdateFile();
                }
                else
                {
                    UpdateInfo info = new UpdateInfo(updatingName, bundleSizeUpdated, bundleSize, totalSizeUpdated,
                        totalSize, numBundlesUpdated, numBundles);
                    if (OnAllUpdated != null)
                    {
                        OnAllUpdated(info);
                    }
                }
            }
            else
            {
                //第一次上线肯定还没有更新包，所以查不到自动认为更新完成
                UpdateInfo info = new UpdateInfo(updatingName, bundleSizeUpdated, bundleSize, totalSizeUpdated,
                        totalSize, numBundlesUpdated, numBundles);
                if (OnAllUpdated != null)
                {
                    OnAllUpdated(info);
                }
            }
        }

        bool NeedUpdate()
        {
            return !localVersionConfig.versionNum.Equals(remoteVersionConfig.versionNum);
        }

        void MakeUpdateList()
        {
            updateList = new List<BundleInfo>();
            foreach (var remoteBundle in remoteVersionConfig.bundles)
            {
                bool isNew = true;
                foreach (var localBundle in localVersionConfig.bundles)
                {              
                    if (remoteBundle.name == localBundle.name)
                    {
                        isNew = false;
                        if (!remoteBundle.md5.Equals(localBundle.md5))
                        {
                            totalSize += remoteBundle.size;
                            updateList.Add(remoteBundle);
                            break;
                        }                   
                    }
                }
                if (isNew)
                {
                    totalSize += remoteBundle.size;
                    updateList.Add(remoteBundle);
                }
            }
            numBundles = updateList.Count;
        }

        void UpdateFile()
        {
            if (updatingIndex < updateList.Count)
            {
                var updating = updateList[updatingIndex];
                updatingName = updating.name;
                bundleSize = updating.size;
                WWW www = new WWW(remoteVersionFileURL + suffix);
                StartCoroutine("DownloadUpdateFileHandler", www);
            }
            else
            {
                File.WriteAllText(Application.temporaryCachePath + "/" + localVersionFileRelativePath + suffix, JsonMapper.ToJson(remoteVersionConfig));
                UpdateInfo info = new UpdateInfo(updatingName, bundleSizeUpdated, bundleSize, totalSizeUpdated,
                        totalSize, numBundlesUpdated, numBundles);
                if (OnAllUpdated != null)
                {
                    OnAllUpdated(info);
                }
            }
        }

        IEnumerator DownloadUpdateFileHandler(WWW www)
        {
            
            while (!www.isDone)
            {
                bundleSizeUpdated = (uint)www.bytesDownloaded;
                if (www.error == null)
                {
                    UpdateInfo info = new UpdateInfo(updatingName, bundleSizeUpdated, bundleSize, totalSizeUpdated + bundleSizeUpdated,
                        totalSize, numBundlesUpdated, numBundles);
                    if (OnOneUpdating != null)
                    {
                        OnOneUpdating(info);
                    }
                    yield return new WaitForSeconds(refreshInterval);
                }
                else
                {
                    bundleSizeUpdated = (uint)www.bytesDownloaded;
                    UpdateInfo info = new UpdateInfo(updatingName, bundleSizeUpdated, bundleSize, totalSizeUpdated + bundleSizeUpdated,
                        totalSize, numBundlesUpdated, numBundles);
                    if (OnOneFailed != null)
                    {
                        OnOneFailed(info);
                    }
                    
                    //下载出错
                    break;
                }        
            }
            if (www.isDone)
            {
                totalSizeUpdated += bundleSize;
                int childFolder = updatingName.LastIndexOf("/");
                if (childFolder > 0)
                {
                    string childFolderPath = Application.temporaryCachePath + "/" +
                                             remoteVersionConfig.bundleRelativePath + "/" +
                                             updatingName.Substring(0, childFolder);
                    if (!Directory.Exists(childFolderPath))
                    {
                        Directory.CreateDirectory(childFolderPath);
                    }
                }
                File.WriteAllBytes(Application.temporaryCachePath + "/" + remoteVersionConfig.bundleRelativePath + "/" + updatingName + suffix, www.bytes);
                UpdateLocalVersionConfig();
                numBundlesUpdated++;     
                UpdateInfo info = new UpdateInfo(updatingName, bundleSizeUpdated, bundleSize, totalSizeUpdated,
                        totalSize, numBundlesUpdated, numBundles);
                    if (OnOneUpdated != null)
                    {
                        OnOneUpdated(info);
                    }
                updatingIndex++;
                UpdateFile();
            }
            
        }

        void UpdateLocalVersionConfig()
        {      
            BundleInfo downloadedBundle = updateList[updatingIndex];
            bool isNew = true;
            foreach (var localBundle in localVersionConfig.bundles)
            {
                if (localBundle.name == downloadedBundle.name)
                {
                    isNew = false;
                    localBundle.size = downloadedBundle.size;
                    localBundle.md5 = downloadedBundle.md5;
                    localBundle.include = downloadedBundle.include;
                    localBundle.dependency = downloadedBundle.dependency;
                    break;
                }
            }
            if (isNew)
            {
                localVersionConfig.bundles.Add(downloadedBundle);
            }
			string savePath = Application.temporaryCachePath + "/" + localVersionFileRelativePath;
			File.WriteAllText(savePath, JsonMapper.ToJson(localVersionConfig));
        }

        public AssetBundle LoadAssetBundle(string bundleName, bool cache = true)
        {
            if (assetBundleCache.ContainsKey(bundleName))
            {
                return assetBundleCache[bundleName];
            }
            string path1 = Application.temporaryCachePath + "/" + localVersionConfig.bundleRelativePath + "/" + bundleName + suffix;
			string path2 = localVersionConfig.bundleRelativePath + "/" + bundleName;
            AssetBundle bundle = null;
            if (File.Exists(path1))
            {
                using (var bundleStream = DeompressAndDecryptLZMA(path1))
                {
                    if (bundleStream == null)
                        return bundle;
                    bundle = AssetBundle.CreateFromMemoryImmediate(bundleStream.ToArray());
                    if (bundle == null)
                        return bundle;
                    if (cache)
                        assetBundleCache.Add(bundleName, bundle);
                }
                
            }
            else
            {
                using (var bundleStream = DeompressAndDecryptLZMA(path2, true))
                {
                    if (bundleStream == null)
                        return bundle;
                    bundle = AssetBundle.CreateFromMemoryImmediate(bundleStream.ToArray());
                    if (bundle == null)
                        return bundle;
                    if (cache)
                        assetBundleCache.Add(bundleName, bundle);
                }
            }
            return bundle;
        }

        public void DestroyAssetBundle(string bundleName)
        {
            if (assetBundleCache.ContainsKey(bundleName))
            {
                assetBundleCache[bundleName].Unload(true);
                assetBundleCache.Remove(bundleName);
            }
        }

        MemoryStream DeompressAndDecryptLZMA(string path, bool fromResourcesPath = false)
        {
            MemoryStream output = new MemoryStream();
			byte[] inputBytes = null;
			TextAsset objInResources = null;
			if (fromResourcesPath) {
				objInResources = Resources.Load(path, typeof(TextAsset)) as TextAsset;
                if (objInResources == null)
                    return null;
				inputBytes = objInResources.bytes;
			} else {
				inputBytes = File.ReadAllBytes(path);
			}
            Decoder coder = new Decoder();
            byte[] decryptedBytes = string.IsNullOrEmpty(password) ? inputBytes : XXTEA.Decrypt(inputBytes, password);
            using (MemoryStream mem = new MemoryStream())
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write(decryptedBytes);
                    mem.Position = 0;
                    using (BinaryReader binReader = new BinaryReader(mem))
                    {
                        byte[] properties = new byte[5];
                        binReader.Read(properties, 0, 5);
                        byte[] fileLengthBytes = new byte[8];
                        binReader.Read(fileLengthBytes, 0, 8);
                        long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);
                        coder.SetDecoderProperties(properties);
                        coder.Code(mem, output, inputBytes.Length, fileLength, null);
                    }        
                }
                  
            }
			if (objInResources != null) {
				Resources.UnloadAsset(objInResources);
			}
            return output;
        }

        void Awake()
        {
			_instance = this;
        }
    }

    public class UpdateInfo
    {
		//The assetbundle name of current assetbundle
        public string bundleName;
		//The downloaded file size of current assetbundle
        public uint bundleSizeUpdated;
		//The file size of current assetbundle
        public uint bundleSize;
		//The total file size of all assetbundles already update completed.
        public uint totalSizeUpdated;
		//The total file size of all assetbundles need to update.
        public uint totalSize;
		//The number of assetbundles already update completed.
        public int numBundleUpdated;
		//The total number of assetbundles  need to update.
        public int numBundle;

        public UpdateInfo(string bundleName, uint bundleSizeUpdated, uint bundleSize, uint totalSizeUpdated, uint totalSize, int numBundleUpdated, int numBundle)
        {
            this.bundleName = bundleName;
            this.bundleSizeUpdated = bundleSizeUpdated;
            this.bundleSize = bundleSize;
            this.totalSizeUpdated = totalSizeUpdated;
            this.totalSize = totalSize;
            this.numBundleUpdated = numBundleUpdated;
            this.numBundle = numBundle;
        }
    }
}
