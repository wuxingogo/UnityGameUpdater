using UnityEngine;
using System.Collections;
using wuxingogo.bundle;
public class TestBundleManager : MonoBehaviour {

    public string remoteAddress = @"127.0.0.1/vc";
    BundleManager updater = null;
    // Use this for initialization
    void Start () {
        updater = BundleManager.GetInstance();
        updater.LoadFromWWW( remoteAddress );

        updater.OnAllUpdated += OnFinishUpdated;

    }

    void OnFinishUpdated( VersionConfig versionConfig)
    {
        var capsule = updater.LoadAssetBundle( "testbundle3" ).LoadAsset<GameObject>( "Capsule");
        Instantiate( capsule );


    }

	// Update is called once per frame
	void Update () {
	
	}
}
