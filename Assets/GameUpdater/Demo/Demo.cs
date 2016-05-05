using UnityEngine;
using System.Collections;
using newx;

public class Demo : MonoBehaviour
{

    private GameUpdater updater;
	//Please create a new version and Package In App first then run this demo!!!!!
	void Start ()
	{
		//GameUpdater is a singleton class,simplely call it every where.
		updater = GameUpdater.instance;
       
        //DoUpdate is the api which sync your assetbundles from server to local.
        //The first parameter is local version file path,please set it like {Bundle Output Folder}/{Version File Name}
        //The second parameter is remote version file path.If no such file there,the updater will use local version file instead.
        //The third parameter is Bundle Encrypt Key you set in GameUpdater pannel.
        updater.DoUpdate("publish/vc", "http://127.0.0.1/publish/vc", "password");
		//Callback when one assetbundle update complete
        updater.OnOneUpdated += OnOneUpdated;
		//Callback when all assetbundle update complete.Means you have finished the update work.
        updater.OnAllUpdated += OnAllUpdated;
		//Callback when one assetbundle is updating.Means you can use it to handler your progress bar.
	    updater.OnOneUpdating += OnOneUpdating;
		//Callback when one assetbundle is update failed.Means the network have problem.
        updater.OnOneFailed += OnOneFailed;
	}

	//The comment of UpdateInfo is in it declaration.
    void OnOneUpdating(UpdateInfo info)
    {
        Debug.Log("Update progress:" + info.totalSizeUpdated + "/" + info.totalSize);
    }

    void OnOneUpdated(UpdateInfo info)
    {
        Debug.Log("Bundle update completed!! Name: "+info.bundleName+" Size:" + info.bundleSize);
        Debug.Log("Update counter:" + info.numBundleUpdated + "/" + info.numBundle);
    }

    private void OnAllUpdated(UpdateInfo info)
    {
        Debug.Log("All bundles update COMPLETE!!!");
        LoadAssets();
    }

    void OnOneFailed(UpdateInfo info)
    {
        Debug.Log("Update error.File name:"+info.bundleName);
    }

    void LoadAssets()
    {
		//Load an assetbundle from disk by the assetbundle name.It will be cached,so if you load it again,it will not load from disk again
		//for saving memory and time.
        Instantiate(updater.LoadAssetBundle("hero").LoadAsset("Hero"));
        Instantiate(updater.LoadAssetBundle("gameobject1").LoadAsset("Gameobject1"));
        Instantiate(updater.LoadAssetBundle("gameobject2").LoadAsset("Gameobject2"));
        Instantiate(updater.LoadAssetBundle("gameobject3").LoadAsset("Gameobject3"));
		//Remove an assetbundle from memory cache and destroy it.If you want to load it again,you need to LoadAssetBundle first.
		updater.DestroyAssetBundle ("gameobject3");
    }
}
