using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PcgWindow : EditorWindow
{
    

    private AstroidGenerator aGen;

   // Rect fromRect = new Rect(50, 0, 100, 100);
    //Rect toRect = new Rect(100, 0, 100, 100);

    private List<Rect> _windowRects;

        // Add menu named "My Window" to the Window menu
    [MenuItem("Generator/Astroid Generator")]
    static void Start()
    {
        // Get existing open window or if none, make a new one:
        PcgWindow window = (PcgWindow)EditorWindow.GetWindow(typeof(PcgWindow));
        window.Init();
    }

    void Init()
    {
        aGen = AssetDatabase.LoadAssetAtPath("Assets/PCGData/Generator.prefab", typeof (AstroidGenerator)) as AstroidGenerator;
        if (aGen == null)
        {
            Debug.LogError("Failed loading generator Data");
            return;
        }
        GenerateWindowRects(0);
    }

    void OnGUI()
    {

        Rect fromRect;
        Rect toRect;
        BeginWindows();
        for (int i = 0; i < _windowRects.Count; i++)
        {
            if (i == 0)
            {
                toRect = _windowRects[i];
                RenderWindows(null, toRect,i); 
            }
            else
            {
                fromRect = _windowRects[i - 1];
                toRect = _windowRects[i];
                RenderWindows(fromRect,toRect,i);
            }
            
        }
        EndWindows();
        
        propertyList();
       
    }

    void GenerateWindowRects(int index)
    {
        _windowRects = new List<Rect>();
        for (int i = 0; i < aGen.AstroidList[index].actions.Count; i++)
        {
            _windowRects.Add(new Rect(i * 160 + 200, 10, 150, 150));
        }
    }

    private void RenderWindows(Rect? nullableFromRect,Rect toRect, int i)
    {
        _windowRects[i] = GUI.Window(i, _windowRects[i], WindowFunction, "Action " + i);
        if (nullableFromRect != null)
        {
            Rect fromRect = nullableFromRect.Value;
            Handles.BeginGUI();
            Handles.DrawBezier(fromRect.center + new Vector2(fromRect.width/2, 0),
                toRect.center - new Vector2(fromRect.width/2, 0),
                fromRect.center + Vector2.right*fromRect.width, toRect.center - Vector2.right*toRect.width, Color.red,
                null, 5f);
            Handles.EndGUI();
        }
        
    }

    private void propertyList()
    {
        EditorGUILayout.BeginVertical("Box");
        for (int i = 0; i < aGen.AstroidList.Count; i++)
        {
            GUILayout.BeginArea(new Rect(0, i*25, 150, 500));
            if (GUI.Button(new Rect(0, 0, 75, 25), "Element " + i))
            {
               GenerateWindowRects(i);
               //_windowRects.Clear();
            }
            GUILayout.EndArea();
        }

        EditorGUILayout.EndVertical();
    }

    void WindowFunction(int windowID)
    {
        GUI.DragWindow();
    }
}