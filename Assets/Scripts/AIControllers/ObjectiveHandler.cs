using UnityEngine;
using System.Collections;

public class ObjectiveHandler
{

    private float[] targetFitnes = new float[4] {250,100,75,50};

    private int currentObjective = 0;

    private int generationNum;


    public ObjectiveHandler()
    {
  
    }

    public void NextGen(int generation)
    {
        generationNum = generation;
    }

    public float GetFitness(ShipBuilderBrain S)
    {
        float fitness =0;
        switch (currentObjective)
        {
            case 0:
               fitness = FitnessFunctions.GetFitness0(S);
                break;


        }
        return fitness;
    }

    

}
