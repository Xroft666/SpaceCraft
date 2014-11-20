using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PcgWindow : EditorWindow
{
    private static PcgWindow _window;

    private AstroidGenerator _aGen;

    private AstroidGenerator.AstroidSettings _astroidObject;

    private int _currentAstroid;

    private float subWindowHeader = 12.5f;

    private Vector2 _astroidScrollPos;
    private Vector2 _astroidDataScrollPos;

    private int seed;

   // Rect fromRect = new Rect(50, 0, 100, 100);
    //Rect toRect = new Rect(100, 0, 100, 100);

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
        _aGen = AssetDatabase.LoadAssetAtPath("Assets/PCGData/Generator.prefab", typeof (AstroidGenerator)) as AstroidGenerator;
        if (_aGen == null)
        {
            Debug.LogError("Failed loading generator Data");
            return;
        }
        SetCurrentAstroid(0);
    }

    void OnGUI()
    {

        
        GUILayout.BeginArea(new Rect(200,0,1000,500));
        EditorGUILayout.BeginVertical("Window");
        RenderActionWindow();
        EditorGUILayout.EndVertical();
        GUILayout.EndArea();

        AstroidGUI();
    }

   

    void GenerateWindowRects()
    {
        _windowRects = new List<Rect>();
        for (int i = 0; i < _aGen.AstroidList[_currentAstroid].actions.Count; i++)
        {
            _windowRects.Add(new Rect(i * 160 + 250, 50, 150, 150));
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
}