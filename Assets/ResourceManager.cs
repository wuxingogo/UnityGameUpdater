using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using newx;

public class ResourceManager : MonoBehaviour
{

	public static ResourceManager Instance {    
		get {
			if( instance == null ) {
				GameObject go = new GameObject( "ResourceManager" );
				DontDestroyOnLoad( go );
				instance = go.AddComponent<ResourceManager>();
			}
			return instance;
		}    

	}

	static ResourceManager instance;
	private GameUpdater updater;

	public string VersionFilePath = "publish/vc";
	//	public string UpdateServerPath = "http://127.0.0.1/publish/vc";
	public string UpdateServerPath = "file:///Users/ly-user/Desktop/publish/vc";
	public string UpdatePassword = "password";
	public string temporaryPath;

	void PrepareUpdater()
	{
		updater.DoUpdate( VersionFilePath, UpdateServerPath, UpdatePassword );
		//Callback when one assetbundle update complete
		updater.OnOneUpdated += OnOneUpdated;
		//Callback when all assetbundle update complete.Means you have finished the update work.
		updater.OnAllUpdated += OnAllUpdated;
		//Callback when one assetbundle is updating.Means you can use it to handler your progress bar.
		updater.OnOneUpdating += OnOneUpdating;
		//Callback when one assetbundle is update failed.Means the network have problem.
		updater.OnOneFailed += OnOneFailed;
	}

	void OnOneUpdating(UpdateInfo info)
	{
		Debug.Log( "Update progress:" + info.totalSizeUpdated + "/" + info.totalSize );
	}

	void OnOneUpdated(UpdateInfo info)
	{
		Debug.Log( "Bundle update completed!! Name: " + info.bundleName + " Size:" + info.bundleSize );
		Debug.Log( "Update counter:" + info.numBundleUpdated + "/" + info.numBundle );
	}

	private void OnAllUpdated(UpdateInfo info)
	{
		Debug.Log( "All bundles update COMPLETE!!!" );
		//LoadAssets();
	}

	void OnOneFailed(UpdateInfo info)
	{
		Debug.Log( "Update error.File name:" + info.bundleName );
	}

	public void GetAllBundlesName()
	{
		AssetBundle ab = updater.LoadAssetBundle( "character" );
		Object[] objects = ab.LoadAllAssets();
		foreach( Object o in objects ) {
			Debug.Log( o.name );
		}
	}

	public TextAsset GetTextAsset(string BigType, string ResourceKey)
	{
		TextAsset result = CreateTextAssetFromResource( BigType, ResourceKey );
		if( result == null ) {
			string path = "TextData/" + BigType + "/" + ResourceKey;
			result = Resources.Load<TextAsset>( path );
		}
		return result;
	}
	protected TextAsset CreateTextAssetFromResource(string BigType, string ResourceKey)
	{
		AssetBundle ab = updater.LoadAssetBundle( BigType );
		if( ab == null ) {
			Debug.Log( "AssetBundle was not found,name is " + BigType );
			return null;
		}
		TextAsset go = ab.LoadAsset<TextAsset>( ResourceKey );
		if( go == null ) {
			Debug.Log( "Resource was not found,name is " + BigType + "/" + ResourceKey );
			return null;
		}
		return go;
	}
	public T LoadResource<T>(string BigType, string ResourceKey) where T : Object
	{
		T result = CreateAssetFromBundle<T>( BigType, ResourceKey );
		if( result == null ) {
			string path = BigType + "/" + ResourceKey;
			return Resources.Load<T>( path );
		}
		return result;
	}


	public GameObject LoadPrefab(string BigType, string ResourceKey)
	{
		GameObject result = CreateFromAssetBundle( BigType, ResourceKey );
		if( result == null ) {
			string path = "Prefabs/" + BigType + "/" + ResourceKey;
			result = Resources.Load<GameObject>( path );
		}
		if( result == null ) {
			string path = BigType + "/" + ResourceKey;
			result = Resources.Load<GameObject>( path );
		}
		return result;
	}

	protected GameObject CreateFromAssetBundle(string BigType, string ResourceKey)
	{
		GameObject go = CreateAssetFromBundle<GameObject>( BigType, ResourceKey );
		return go; 
	}

	protected T CreateAssetFromBundle<T>(string BigType, string ResourceKey) where T : Object
	{
		AssetBundle ab = updater.LoadAssetBundle( BigType );
		if( ab == null ) {
			Debug.Log( "AssetBundle was not found,name is " + BigType );
			return null;
		}
		T go = ab.LoadAsset<T>( ResourceKey );
		if( go == null ) {
			Debug.Log( "Resource was not found,name is " + BigType + "/" + ResourceKey );
			return null;
		}
		return go; 
	}

	public GameObject GetPrefabFromResource(string BigType, string ResourceKey)
	{
		AssetBundle ab = updater.LoadAssetBundle( BigType );
		if( ab == null ) {
			Debug.Log( "AssetBundle was not found,name is " + BigType );
			return null;
		}
		GameObject go = ab.LoadAsset<GameObject>( ResourceKey );
		if( go == null ) {
			Debug.Log( "Resource was not found,name is " + BigType + "/" + ResourceKey );
			return null;
		}
		return go;
	}
		
	// Use this for initialization
	void Awake()
	{
		instance = this;
		updater = GameUpdater.instance;
		updater.JustLoadLocalVersionFile( "publish/vc", "password" );
		DontDestroyOnLoad( this.gameObject );
	}

	void Start()
	{
		temporaryPath = Application.temporaryCachePath;
		PrepareUpdater();

	}

	// Update is called once per frame
	void Update()
	{

	}
}
