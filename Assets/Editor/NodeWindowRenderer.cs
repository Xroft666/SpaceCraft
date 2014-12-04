using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections;
using WorldGen;

public static class NodeWindowRenderer {

    /// <summary>
    /// Takes care of rendering of the node windows
    /// </summary>
    /// <param name="window"></param>
    /// <param name="id"></param>
    /// <param name="aGen"></param>
    public static void PrepareRender(PcgWindow window,int id, AstroidGenerator aGen)
    {
        AstroidGenerator.AstroidSettings.Action action = window.AstroidObject.actions[id];

        if (GUI.Button(new Rect(0, 0, 15, 15), "X"))
        {
            window.AstroidObject.actions.RemoveAt(id);
            window.GenerateWindows();
            window.Refresh();
        }

        EditorGUILayout.LabelField("Method:");
        ActionEnum(window,id);
        EditorGUILayout.LabelField("Name:");
        action.name = EditorGUILayout.TextField("", action.name);
        
        switch (action.method)
        {
            case AstroidGenerator.AstroidSettings.Action.Method.CellularAutomata:
                RenderCA(action,id);
                break;
            case AstroidGenerator.AstroidSettings.Action.Method.Noise:
                RenderNoise(action,id);
                break;
            case AstroidGenerator.AstroidSettings.Action.Method.MapEdgeCleaning:
                RenderMapEdgeClear(action, id);
                break;
            case AstroidGenerator.AstroidSettings.Action.Method.PerlinNoise:
                RenderPerlinNoise(action,id);
                break;
            case AstroidGenerator.AstroidSettings.Action.Method.Invert:
                RenderInvert(action, id);
                break;

        }

        GUI.DrawTexture(new Rect(0, 300, 150, 150), window.Texture2Ds[id], ScaleMode.StretchToFill,true);

		if( GUILayout.Button("Debug") )
		{
			AsteroidEvaluator.ClearData();

			int[,] map = window.Maps[id];
			AsteroidEvaluator.Evaluate(ref map);
		}
    }

    /// <summary>
    /// Renderer for Cellular Automata
    /// </summary>
    /// <param name="CAS"></param>
    public static void RenderCA(AstroidGenerator.AstroidSettings.Action action, int id)
    {
        CellularAutomataStats CAS = action.cellularAutomataStats;
        
        
        
        EditorGUILayout.LabelField("BlackChangeThreshold:");
        CAS.BlackChangeThreshold = EditorGUILayout.FloatField("", CAS.BlackChangeThreshold);
        EditorGUILayout.LabelField("WhileChangeThreshold:");
         CAS.WhileChangeThreshold = EditorGUILayout.FloatField("", CAS.WhileChangeThreshold);
        EditorGUILayout.LabelField("Radius:");
         CAS.Radius = EditorGUILayout.IntField("", CAS.Radius);
        EditorGUILayout.LabelField("Rounds:");
         CAS.Rounds = EditorGUILayout.IntField("", CAS.Rounds);
    }

    public static void RenderInvert(AstroidGenerator.AstroidSettings.Action action, int id)
    {
        EditorGUILayout.LabelField("From:");
        action.invertStats.x = EditorGUILayout.IntField("", action.invertStats.x);
        EditorGUILayout.LabelField("To:");
        action.invertStats.y = EditorGUILayout.IntField("", action.invertStats.y);
    }

    public static void RenderPerlinNoise(AstroidGenerator.AstroidSettings.Action action, int id)
    {
        PerlinNoiseStats PS = action.perlinNoiseStats;
        
        EditorGUILayout.LabelField("Octav Number:");
        PS.octNum = EditorGUILayout.IntField("", PS.octNum);
        EditorGUILayout.LabelField("Frequenzy:");
        PS.frq = EditorGUILayout.FloatField("", PS.frq);
        EditorGUILayout.LabelField("Amplitude:");
        PS.amp = EditorGUILayout.FloatField("", PS.amp);
    }

    public static void RenderNoise(AstroidGenerator.AstroidSettings.Action action, int id)
    {
        EditorGUILayout.LabelField("Black Chance:");
        action.noiseThreshold = EditorGUILayout.FloatField("", action.noiseThreshold);
    }

    public static void RenderMapEdgeClear(AstroidGenerator.AstroidSettings.Action action, int id)
    {
        EditorGUILayout.LabelField("Thikness:");
        action.mapEdgeCleaning = EditorGUILayout.IntField("", action.mapEdgeCleaning);
    }

    static void ActionEnum(PcgWindow window, int windowID)
    {
        List<AstroidGenerator.AstroidSettings.Action> aList =
            window.AstroidObject.actions;

        aList[windowID].method = (AstroidGenerator.AstroidSettings.Action.Method)EditorGUILayout.EnumPopup("", aList[windowID].method);
    }
}
