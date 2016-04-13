using UnityEngine;
using System.Collections;
using wuxingogo.Runtime;

public class ResourcesLoad : XMonoBehaviour {

    [X]
    public void LoadResourcesText( string path )
    {
        var text = Resources.Load<TextAsset>( path ).text;
        Debug.Log( text );
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
