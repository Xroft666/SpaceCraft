using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using UnityEngine;
using UnityEditor;
using WorldGen;

public class PcgWindow : EditorWindow
{
    private static PcgWindow _window;

    private AstroidGenerator _aGen;
    private GameObject prefab;

    public Texture2D[] Texture2Ds;

    public AstroidGenerator.AstroidSettings AstroidObject;

    private int _currentAstroid;

    private float subWindowHeader = 12.5f;

    private Vector2 _astroidScrollPos;
    private Vector2 _astroidDataScrollPos;
    private Vector2 _nodeFieldScrollPos;

    private int seed;

    private List<NodeWindow> windows;

    private readonly string filePath = "Assets/PCGData/";
    private readonly string objectName = "Generator.prefab";

    public Handle SelectedHandle;
    

    // Add menu named "My Window" to the Window menu
    [MenuItem("Generator/Astroid Generator")]
    static void Start()
    {
        // Get existing open window or if none, make a new one:
        _window = (PcgWindow)EditorWindow.GetWindow(typeof(PcgWindow));
        _window.Init();
    }

    void Init(int i=0)
    {
        //_texture2 = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/PCGData/t.jpg", typeof (Texture2D));
        GameObject go = (GameObject) AssetDatabase.LoadAssetAtPath(filePath+"Generator.prefab", typeof (GameObject));
        prefab = (GameObject) GameObject.Instantiate(go);
        _aGen = prefab.GetComponent<AstroidGenerator>();
        if (_aGen == null)
        {
            Debug.LogError("Failed loading generator Data");
            return;
        }
        SetCurrentAstroid(i);
        
    }

    void OnGUI()
    {
        Rect nodeRect = new Rect(200,0,_window.position.width-400,_window.position.height-50);

        GUILayout.BeginArea(nodeRect, AstroidObject.name, GUIStyle.none);
      
        _nodeFieldScrollPos = GUILayout.BeginScrollView(_nodeFieldScrollPos, true, true);
        EditorGUILayout.BeginVertical("Window");
        EditorGUILayout.LabelField("",GUILayout.Width(10000), GUILayout.Height(10000));
        RenderActionWindow();
        foreach (NodeWindow w in windows)
        {
            w.Render(_window);
        }
        EditorGUILayout.EndVertical();
        GUILayout.EndScrollView();
        
        GUILayout.EndArea();

        AstroidGUI();
        ActionButtons();
        //AssetDatabase.SaveAssets();

        
    }

   

    public void GenerateWindows()
    {

        windows = new List<NodeWindow>();
        for (int i = 0; i < _aGen.AstroidList[_currentAstroid].actions.Count; i++)
        {
            AstroidGenerator.AstroidSettings.Action a = _aGen.AstroidList[_currentAstroid].actions[i];
            Rect r = new Rect(a.windowEditor.windowPos.x, a.windowEditor.windowPos.y, 150, 450);
            NodeWindow n = new NodeWindow(r,_aGen.AstroidList[_currentAstroid].actions[i]);
            windows.Add(n);
        }
    }


    #region Action

    private void ActionButtons()
    {
        GUILayout.BeginArea(new Rect(_window.position.width-200, 0, 200, 1000));
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Action"))
        {
            AstroidObject.actions.Add(new AstroidGenerator.AstroidSettings.Action());
            SetCurrentAstroid(_currentAstroid);
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void RenderActionBezier(Rect? nullableFromRect, Rect toRect, int i)
    {
        if (nullableFromRect != null)
        {
            Rect fromRect = nullableFromRect.Value;
            Handles.BeginGUI();
            Handles.DrawBezier(fromRect.center + new Vector2(fromRect.width / 2, 0),
                toRect.center - new Vector2(fromRect.width / 2, 0),
                fromRect.center + Vector2.right * fromRect.width / 1.5f,
                toRect.center - Vector2.right * toRect.width / 1.5f,
                Color.red, null, 5f);
            Handles.EndGUI();
        }

    }

    private void RenderActionWindow()
    {
        Rect fromRect;
        Rect toRect;
        BeginWindows();
        for (int i = 0; i < windows.Count; i++)
        {
            windows[i].WindowRect = GUI.Window(i, windows[i].WindowRect, ActionWindowFunction, "Action " + i);
            if (i == 0)
            {
                toRect = windows[i].WindowRect;
                RenderActionBezier(null, toRect, i);
            }
            else
            {
                fromRect = windows[i - 1].WindowRect;
                toRect = windows[i].WindowRect;
                RenderActionBezier(fromRect, toRect, i);
            }

        }
        EndWindows();
    }

    /// <summary>
    /// functionality of action windows
    /// </summary>
    /// <param name="windowID"></param>
    void ActionWindowFunction(int windowID)
    {
        NodeWindowRenderer.PrepareRender(_window,windowID,_aGen);

        GUI.DragWindow();
    }

    
    #endregion

    #region Astroid

    void AstroidGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, 200, 500));
        EditorGUILayout.BeginVertical("Box");
        if (GUILayout.Button("Save"))
        {
            Save();
        }
        if (GUILayout.Button("Refresh"))
        {
            Refresh();
        }
        EditorGUILayout.LabelField("Seed:");
        seed = EditorGUILayout.IntField("", seed);
        
        AstroidList();
        AstroidDataManipulators();
        EditorGUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private string astroidLabel = "MyName";

    /// <summary>
    /// Buttons for manipulating the astroid list
    /// </summary>
    void AstroidDataManipulators()
    {
      
       
       // GUILayout.BeginArea(new Rect(0, 260, 200, 500));
        
        
        if (GUILayout.Button("Add Astroid"))
        {
            AddAstroid();
        }
        if (GUILayout.Button("Remove Current Astroid"))
        {
            RemoveAstroid();
        }

        EditorGUILayout.LabelField("Astroid Name");
        AstroidObject.name = EditorGUILayout.TextField(AstroidObject.name);
        EditorGUILayout.LabelField("Astroid Size");
        AstroidObject.size = EditorGUILayout.IntField(AstroidObject.size);
        //GUILayout.EndArea();
        
       
    }

    /// <summary>
    /// Selection of astroid
    /// </summary>
    private void AstroidList()
    {
        

        _astroidScrollPos = EditorGUILayout.BeginScrollView(_astroidScrollPos, false, true, GUILayout.Width(190), GUILayout.Height(250));
        for (int i = 0; i < _aGen.AstroidList.Count; i++)
        {
            GUILayout.BeginArea(new Rect(0, i * 25, 170, 500));
            if (GUI.Button(new Rect(0, 0, 170, 25), _aGen.AstroidList[i].name))
            {
                SetCurrentAstroid(i);

            }
            GUILayout.EndArea();
        }
        EditorGUILayout.EndScrollView();
        
    }

    void SetCurrentAstroid(int i)
    {
        _currentAstroid = i;
        AstroidObject = _window._aGen.AstroidList[_currentAstroid];
        Texture2Ds = new Texture2D[AstroidObject.actions.Count];
        UpdateTextures();
        GenerateWindows();  
        
    }

    void AddAstroid()
    {
        AstroidGenerator.AstroidSettings a = new AstroidGenerator.AstroidSettings();
        _window._aGen.AstroidList.Add(a);
        SetCurrentAstroid(_window._aGen.AstroidList.Count - 1);
        
    }

    void RemoveAstroid()
    {
        _window._aGen.AstroidList.RemoveAt(_currentAstroid);
        SetCurrentAstroid(_window._aGen.AstroidList.Count - 1);
    }

    #endregion

    private void Save()
    {
        AssetDatabase.DeleteAsset(filePath + objectName);
        PrefabUtility.CreatePrefab(filePath + objectName, prefab);
    }

    public void Refresh()
    {
        UpdateTextures();
        foreach (NodeWindow n in windows)
        {
            n.Save();
        }
    }

    void UpdateTextures()
    {
        int size = AstroidObject.size;
        int[,] map = new int[size,size];

        GenerationProcedures GP = new GenerationProcedures(_aGen, ref map, seed, AstroidObject);  
       
        for (int j = 0; j < AstroidObject.actions.Count; j++)
        {
            AstroidGenerator.AstroidSettings.Action a = AstroidObject.actions[j];
            GP.GenerateAction(ref map, a);
            Texture2Ds[j] = MapUtility.MapToBinaryTexture(map);
            
            Texture2Ds[j].wrapMode = TextureWrapMode.Clamp;
            Texture2Ds[j].filterMode = FilterMode.Point;
            Texture2Ds[j].Apply();
            
        }
    }
}