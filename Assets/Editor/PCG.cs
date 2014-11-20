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

    private Texture2D texture2;

    private Texture2D[] texture2Ds;

    private AstroidGenerator.AstroidSettings _astroidObject;

    private int _currentAstroid;

    private float subWindowHeader = 12.5f;

    private Vector2 _astroidScrollPos;
    private Vector2 _astroidDataScrollPos;

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
        texture2 = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/PCGData/t.jpg", typeof (Texture2D));
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
        GUILayout.BeginArea(new Rect(200,0,_window.position.width-400,_window.position.height-50), _astroidObject.name, GUIStyle.none);
        EditorGUILayout.BeginVertical("Window");
        RenderActionWindow();
        EditorGUILayout.EndVertical();
        GUILayout.EndArea();

        AstroidGUI();

        //AssetDatabase.SaveAssets();
    }

   

    void GenerateWindowRects()
    {
        _windowRects = new List<Rect>();
        for (int i = 0; i < _aGen.AstroidList[_currentAstroid].actions.Count; i++)
        {
            _windowRects.Add(new Rect(i * 160 + 250, 50, 150, 300));
        }
    }


    #region Action

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
        EditorGUILayout.LabelField("Method:");
        ActionEnum(windowID);
        EditorGUILayout.LabelField("Name:");
        _astroidObject.actions[windowID].name = EditorGUILayout.TextField("", _astroidObject.actions[windowID].name);
        EditorGUILayout.LabelField("Index:");
        _astroidObject.actions[windowID].index = EditorGUILayout.IntField("", _astroidObject.actions[windowID].index);

        GUI.DrawTexture(new Rect(0,150,150,150), texture2Ds[windowID]);

        GUI.DragWindow();
    }

    void ActionEnum(int windowID)
    {
        List<AstroidGenerator.AstroidSettings.Actions> aList =
            _astroidObject.actions;

        aList[windowID].method = (AstroidGenerator.AstroidSettings.Actions.Method)EditorGUILayout.EnumPopup("", aList[windowID].method);
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

       astroidLabel = GUILayout.TextField(astroidLabel);
        if (GUILayout.Button("Rename Current Astroid"))
        {
            _astroidObject.name = astroidLabel;
        }
        
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
        _astroidObject = _window._aGen.AstroidList[_currentAstroid];
        texture2Ds = new Texture2D[_astroidObject.actions.Count];
        UpdateTextures();
        GenerateWindowRects();
        
    }

    void AddAstroid()
    {
        AstroidGenerator.AstroidSettings a = new AstroidGenerator.AstroidSettings();
        _window._aGen.AstroidList.Add(a);
        _currentAstroid = _window._aGen.AstroidList.Count - 1;
    }

    void RemoveAstroid()
    {
        _window._aGen.AstroidList.RemoveAt(_currentAstroid);
    }

    #endregion

    void Refresh()
    {
        SetCurrentAstroid(_currentAstroid);
    }

    void UpdateTextures()
    {
        int size = _astroidObject.size;
        int[,] map = new int[size,size];
        map = NoiseGenerator.GenerateNoise(seed,map);
        GenerationProcedures GP = new GenerationProcedures(_aGen, ref map, seed, _astroidObject);  
        foreach (AstroidGenerator.AstroidSettings.Actions a in _astroidObject.actions)
        {
            GP.GenerateAction(ref map,a);
            int i = _astroidObject.actions.IndexOf(a);
            texture2Ds[i] = MapUtility.MapToBinaryTexture(map);
            texture2Ds[i].Apply();
            //AssetDatabase.CreateAsset(texture2Ds[i].EncodeToPNG(), path: "Assets/PCGData/tmpTex"+i+".png");
            //texture2Ds[i].SetPixel(2,2,new Color(0.5f,0.3f,0.1f));
            //Serializer.Save(Application.dataPath + "/Data/SavedVoxelSystems/tmpTex" + i + ".jpg", texture2Ds[i].EncodeToJPG());
        }
    }
}