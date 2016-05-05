using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Security.Policy;


[CustomEditor(typeof(BundleInfoInspectorObj))]
internal class BundleInfoEditor : Editor
{
    static BundleInfoInspectorObj bundleInfoInspectorObj = null;
    public static BundleInfo bundle = null;
    public static BundleInfoEditor current = null;

    void OnEnable()
    {
        BundleInfoEditor.current = this;
    }

    public static void Show(BundleInfo bundle)
    {
        if (bundleInfoInspectorObj == null)
        {
            bundleInfoInspectorObj = ScriptableObject.CreateInstance<BundleInfoInspectorObj>();
            bundleInfoInspectorObj.hideFlags = HideFlags.DontSave;
            bundleInfoInspectorObj.name = "AssetBundle Detail";
        }
        Selection.activeObject = bundleInfoInspectorObj;

        BundleInfoEditor.bundle = bundle;
        if (BundleInfoEditor.current != null)
        {
            BundleInfoEditor.current.Repaint();
        }
    }

    public override void OnInspectorGUI()
    {
        if (bundle == null) return;

        EditorGUILayout.BeginVertical();
        /*
        GUILayout.Label("文件大小:", GameUpdaterStyle.GetStyle("Title"));
        GUILayout.Label(Mathf.CeilToInt(bundle.size / 1024f) + " KB");

        GUILayout.Label("文件MD5:", GameUpdaterStyle.GetStyle("Title"));
        GUILayout.Label(bundle.md5.ToString());
         * */

        GUILayout.Label("Include Resource", GameUpdaterStyle.GetStyle("Title"));
        foreach (var path in bundle.include)
        {
            GUILayout.Label(path);
        }

        GUILayout.FlexibleSpace();

        GUILayout.Label("Dependency Resource", GameUpdaterStyle.GetStyle("Title"));
        //EditorGUILayout.BeginVertical();
        foreach (var path in bundle.dependency)
        {
            GUILayout.Label(path);
        }
        EditorGUILayout.EndVertical();



    }

}

