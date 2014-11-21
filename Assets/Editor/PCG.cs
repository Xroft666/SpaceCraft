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

    private Texture2D _texture2;

    public Texture2D[] Texture2Ds;

    public AstroidGenerator.AstroidSettings AstroidObject;

    private int _currentAstroid;

    private float subWindowHeader = 12.5f;

    private Vector2 _astroidScrollPos;
    private Vector2 _astroidDataScrollPos;
    private Vector2 _nodeFieldScrollPos;

    private int seed;

    private List<Rect> _windowRects;
    

    // Add menu named "My Window" to the Window menu
    [MenuItem("Generator/Astroid Generator")]
    static void Start()
    {
        // Get existing open window or if none, make a new one:
        _window = (PcgWindow)EditorWindow.GetWindow(typeof(PcgWindow));
        _window.Init();
    }

    void Init()
    {
        //_texture2 = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/PCGData/t.jpg", typeof (Texture2D));
        _aGen = (AstroidGenerator) AssetDatabase.LoadAssetAtPath("Assets/PCGData/Generator.prefab", typeof (AstroidGenerator));
        if (_aGen == null)
        {
            Debug.LogError("Failed loading generator Data");
            return;
        }
        SetCurrentAstroid(0);
    }

    void OnGUI()
    {
        Rect nodeRect = new Rect(200,0,_window.position.width-400,_window.position.height-50);

        GUILayout.BeginArea(nodeRect, AstroidObject.name, GUIStyle.none);
      
        _nodeFieldScrollPos = EditorGUILayout.BeginScrollView(_nodeFieldScrollPos, true, true);
        EditorGUILayout.BeginVertical("Window");
        RenderActionWindow();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        
        GUILayout.EndArea();

        AstroidGUI();
        ActionButtons();
        //AssetDatabase.SaveAssets();
    }

   

    void GenerateWindowRects()
    {
        Debug.Log(("Generating windows"));
        Debug.Log("astroid " + _currentAstroid + " " + _aGen.AstroidList[_currentAstroid].name);
        Debug.Log("num " + _aGen.AstroidList[_currentAstroid].actions.Count);
        _windowRects = new List<Rect>();
        for (int i = 0; i < _aGen.AstroidList[_currentAstroid].actions.Count; i++)
        {
            _windowRects.Add(new Rect(i * 160 + 25, 50, 150, 450));
        }
    }


    #region Action

    private void ActionButtons()
    {
        GUILayout.BeginArea(new Rect(_window.position.width-200, 0, 200, 1000));
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Add Action"))
        {
            AstroidObject.actions.Add(new AstroidGenerator.AstroidSettings.Action());
            SetCurrentAstroid(_currentAstroid);
        }
        EditorGUILayout.EndVertical();
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
        for (int i = 0; i < _windowRects.Count; i++)
        {
            _windowRects[i] = GUI.Window(i, _windowRects[i], ActionWindowFunction, "Action " + i);
            if (i == 0)
            {
                toRect = _windowRects[i];
                RenderActionBezier(null, toRect, i);
            }
            else
            {
                fromRect = _windowRects[i - 1];
                toRect = _windowRects[i];
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
        GenerateWindowRects();
        
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

    void Refresh()
    {
        UpdateTextures();
        AssetDatabase.SaveAssets();
     
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