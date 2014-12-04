using System;
using System.Threading;
using UnityEngine;
using System.Collections;
using WorldGen;

public class AstroidEvaluatorGenerator : MonoBehaviour
{

    public AstroidGenerator generator;

    public int Rounds;

    private AsteroidEvaluator evaluator;

    private int[,] intMap;

    private int astroidID;

    private GameObject renderQuad ;

    private Texture2D visual;

	// Use this for initialization
	void Start ()
	{
	    renderQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        evaluator = new AsteroidEvaluator();
	    
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Evaluate()
    {
        int mapSize = generator.AstroidList[astroidID].size;

        for (int i = 0; i < Rounds; i++)
        {
            foreach (AstroidGenerator.AstroidSettings.Action t in generator.AstroidList[astroidID].actions)
            {
                t.Randomize(mapSize);
            }
            int[,] map = new int[mapSize,mapSize];

            GenerationProcedures GP = new GenerationProcedures(generator, ref map, 0, generator.AstroidList[astroidID]);


            Thread thread = new Thread(GP.Generate);
            thread.Start();
            while (thread.IsAlive)
            {
                yield return new WaitForEndOfFrame();
            }

            AsteroidEvaluator.CollectData(ref map);
        }

        float[,] f = AsteroidEvaluator.GetNormalizedData();
        visual = new Texture2D(f.GetLength(0),f.GetLength(1));
        for (int x = 0; x < f.GetLength(0); x++)
        {
            for (int y = 0; y < f.GetLength(1); y++)
            {
                visual.SetPixel(x,y,new Color(f[x,y],0,0,0));
            }
        }
        visual.Apply();
        renderQuad.renderer.material.mainTexture = visual;
    }

    void PrepareData()
    {
          
    }

    void OnGUI()
    {
        astroidID = Convert.ToInt32(GUI.TextField(new Rect(0, 0, 150, 25), astroidID.ToString()));
        if (GUI.Button(new Rect(0, 25, 150, 25), "Evaluate"))
        {
           StartCoroutine(Evaluate()); 
        }
    }
}
