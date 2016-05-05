using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


internal  enum ViewState
{
    Package,
    List
}
internal class GameUpdaterWin : EditorWindow
{
    List<BundleInfo> selections = new List<BundleInfo>();
    BundleInfo lastSelection;
    VersionManager vm = new VersionManager();
    BundleManager bm = new BundleManager();
    bool copyToStreamingAssets;
    ViewState viewState = ViewState.Package;
	// Use this for initialization
    void OnEnable()
    {
        
    }

    void OnDestroy()
    {
        
    }

    void OnFocus()
    {  
        bm.UpdateAll();
    }

	
	// Update is called once per frame
	void Update () {
        if (lastSelection != GetLastSelection())
        {
            lastSelection = GetLastSelection();
            BundleInfoEditor.Show(lastSelection);
        }
	}

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
        {
            Rect createRect = GUILayoutUtility.GetRect(new GUIContent("Version Manager"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            if (GUI.Button(createRect, "Version Manager", EditorStyles.toolbarDropDown))
            {
                viewState = ViewState.Package;
            }
            Rect listRect = GUILayoutUtility.GetRect(new GUIContent("AssetBundle List"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            if (GUI.Button(listRect, "AssetBundle List", EditorStyles.toolbarDropDown))
            {
                viewState = ViewState.List;
            }
        }
        EditorGUILayout.EndHorizontal();
        if (viewState == ViewState.List)
        {
            EditorGUILayout.BeginVertical();
            {
                foreach (var bundle in bm.bundles)
                {
                    var selected = bundle == lastSelection ? true : false;
                    Rect itemRect =
                        EditorGUILayout.BeginHorizontal(selected
                            ? GameUpdaterStyle.GetStyle("SelectItem")
                            : GameUpdaterStyle.GetStyle("UnselectItem"));
                    EditorGUILayout.LabelField(bundle.name);
                    EditorGUILayout.EndHorizontal();
                    ProcessSelection(itemRect, bundle);
                }
            }
            EditorGUILayout.EndVertical();
        }
        else if(viewState == ViewState.Package)
        {
            EditorGUILayout.BeginVertical();
            {
				Config.versionFileName = EditorGUILayout.TextField("Version File Name", Config.versionFileName);
                Config.password = EditorGUILayout.TextField("Bundle Encrypt Key", Config.password);
                Config.bundleRelativePath = EditorGUILayout.TextField("Bundle Output Folder", Config.bundleRelativePath);
                copyToStreamingAssets = EditorGUILayout.Toggle("Package In App", copyToStreamingAssets);
                Config.platform = (TargetPlatform)EditorGUILayout.EnumPopup("Target Platform", Config.platform, GUILayout.MaxWidth(300));
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginHorizontal();
            bool isHorizontalBlockActive = true; 
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create New Version", GUILayout.MaxWidth(150)))
            {
                bm.BuildAll();
                bm.CopyAll(copyToStreamingAssets);
                vm.CreateVersionFile(bm.bundles, copyToStreamingAssets);
                isHorizontalBlockActive = false;
            }
            GUILayout.FlexibleSpace();
            if (isHorizontalBlockActive)
                EditorGUILayout.EndHorizontal();
        }
        
    }

    void ProcessSelection(Rect itemRect, BundleInfo bundle)
    {
        if (IsRectClicked(itemRect))
        {
            if (Event.current.button == 0 || !selections.Contains(bundle))
			{
                selections.Clear();
                selections.Add(bundle);
                Repaint();
			}
        }
        
    }

    bool IsRectClicked(Rect rect)
    {
        return Event.current.type == EventType.MouseDown && IsMouseOn(rect);
    }

    bool IsMouseOn(Rect rect)
    {
        return rect.Contains(Event.current.mousePosition);
    }

    BundleInfo GetLastSelection()
    {
        if (selections.Count > 0)
            return selections[0];
        else
            return null;
    }

    [MenuItem("Window/Game Updater")]
    static void Init()
    {
        EditorWindow.GetWindow<GameUpdaterWin>("Game Updater");
    }
 
}
  



