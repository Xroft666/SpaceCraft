using System;
using System.IO;
using System.Threading;
using UnityEngine;
using System.Collections;
using WorldGen;

public class AstroidEvaluatorGenerator : MonoBehaviour
{

    public AstroidGenerator generator;

    public int Rounds;

    private int _astroidId;

    private GameObject _renderQuad ;

    private Texture2D _visual;
    private string _filename = "";
    private bool _autoSave;
    private int _progress;
    private bool _running;

    // Use this for initialization
	void Start ()
	{
	    _renderQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        new AsteroidEvaluator();
	    
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Evaluate()
    {

        Debug.Log("STARTING EVALUATION...");
        _running = true;
        int mapSize = generator.AstroidList[_astroidId].size;

        for (int i = 0; i < Rounds; i++)
        {
            foreach (AstroidGenerator.AstroidSettings.Action t in generator.AstroidList[_astroidId].actions)
            {
                t.Randomize(mapSize);
            }
            int[,] map = new int[mapSize, mapSize];

            GenerationProcedures GP = new GenerationProcedures(generator, ref map, 0,
                generator.AstroidList[_astroidId]);


            Thread thread = new Thread(GP.Generate);
            thread.Start();
            while (thread.IsAlive)
            {
                yield return new WaitForEndOfFrame();
            }

            try{
                AsteroidEvaluator.CollectData(ref map);
                _progress = i;
                Debug.Log("Progress:" + (i + 1) + "/" + Rounds);
            }
            catch (Exception e)
            { 
                Debug.LogException(e);
                i--;
            }

            if (i%100 == 0)
            {
               TmpSave(i); 
            }
        }

        float[,] f = AsteroidEvaluator.GetNormalizedData();

        _visual = MapUtility.MapToBinaryTexture(f);
        _renderQuad.renderer.material.mainTexture = _visual;
        _running = false;
        
        if (_autoSave)
        {
            Export();
        }
    }

    

    void TmpSave(int i)
    {
        float[,] f = AsteroidEvaluator.GetNormalizedData();
        Texture2D t = MapUtility.MapToBinaryTexture(f);
        byte[] b = t.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/Data/Evaluation/tmpsave/" + _filename +""+i+".png", b);
        Debug.Log("Tmp save "+i);
        Destroy(t);
    }

    void Export()
    {
        byte[] b = _visual.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/Data/Evaluation/" + _filename + ".png", b);
        Debug.Log("Export Complete");
    }

    void OnGUI()
    {
        
        _astroidId = Convert.ToInt32(GUI.TextField(new Rect(0, 0, 150, 25), _astroidId.ToString()));
        GUI.Label(new Rect(150, 0, 150, 25),"Astroid ID");
        _filename = GUI.TextField(new Rect(0, 25, 150, 25), _filename);
        GUI.Label(new Rect(150, 25, 150, 25), "File Name");
        Rounds = Convert.ToInt32(GUI.TextField(new Rect(0, 50, 150, 25), Rounds.ToString()));
        GUI.Label(new Rect(150, 50, 150, 25), "Rounds");

        if (GUI.Button(new Rect(0, 75, 150, 25), "Evaluate"))
        {
           StartCoroutine(Evaluate()); 
        }
        
        _autoSave = GUI.Toggle(new Rect(150, 75, 150, 25), _autoSave, "Autosave");

        if (GUI.Button(new Rect(0, 100, 150, 25), "Export"))
        {
            Export();
        }

        if(_running)
        GUI.Label(new Rect(0, Screen.height / 2, 150, 50), "Progress:" + (_progress + 1) + "/" + Rounds);

        
    }
}
