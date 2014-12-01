using System;
using System.Collections.Generic;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using UnityEngine;
using System.Collections;

public class ObjectiveHandler
{

    private float[] targetFitnes = new float[4] {250,180,170,150};

    private int currentObjective = 0;
    private int checkingObjective = 0;

    private int _generationNum;

    private GameObject target;

    private readonly List<Action> _objectiveList = new List<Action>();


    private bool _checkingObjectives;

    public ObjectiveHandler()
    {
        InitTarget();
        
        _objectiveList.Add(SetObjective0);
        _objectiveList.Add(SetObjective1);
        _objectiveList.Add(SetObjective2);
        _objectiveList.Add(SetObjective3);
        //_objectiveList.Add(SetObjective4);
        //_objectiveList.Add(SetObjective5);



        _objectiveList[0]();

       
    }

    private void InitTarget()
    {
        target = new GameObject();
        CircleCollider2D c = target.AddComponent<CircleCollider2D>();
        c.radius = 1;
        c.isTrigger = true;
        target.transform.tag = "Target";
        target.name = "Target";
        target.AddComponent<GotoTarget>();
    }

    public void NextGen()
    {
        NeatEvolutionAlgorithm<NeatGenome> ea = Optimizer._ea;

        _generationNum = (int) ea.Statistics._generation;
        float maxFit = (float) ea.Statistics._maxFitness;
        float meanFit = (float) ea.Statistics._meanFitness;

        if (!_checkingObjectives)
        {
            if (meanFit >= targetFitnes[currentObjective])
            {
                currentObjective++;
                _checkingObjectives = true;
                checkingObjective = 0;
                _objectiveList[checkingObjective]();
                Debug.Log("OBJECTIVE "+(currentObjective-1)+" COMPLETED, CHECKING OBJECTIVES");
            }
            else
            {
                Debug.Log("TRAINING OBJECTIVE " + (currentObjective) + "... "+meanFit+"/"+targetFitnes[currentObjective]);
            }
        }
        else
        {
            if (meanFit >= targetFitnes[currentObjective])
            {
                if (checkingObjective < currentObjective)
                {
                    checkingObjective ++;
                    _objectiveList[checkingObjective]();
                    Debug.Log("CHECKING OBJECTIVE " + (checkingObjective - 1) + " COMPLETED");
                }
                else
                {
                    _checkingObjectives = false;
                    _objectiveList[currentObjective]();
                    Debug.Log("CHECKING OBJECTIVES COMPLETE, BACK TO TRAINING");
                }
            }
            else
            {
                _checkingObjectives = false;
                currentObjective = checkingObjective;
                _objectiveList[currentObjective]();
                checkingObjective = 0;
                Debug.Log("OBJECTIVE " + currentObjective + " FAILED, RETRAINING THIS OBJECTIVE");
            }
        }
    }

    public float GetFitness(ShipBuilderBrain S)
    {
        float fitness =0;
        int t = _checkingObjectives ? checkingObjective : currentObjective;

        if (t == 0 || t==1 || t==2 || t==3)
        {
            fitness = FitnessFunctions.GetFitnessStayOnTarget(S);
        }
        return fitness;
    }

    private void SetObjective0()
    {
       target.transform.position = Vector3.zero;
    }

    private void SetObjective1()
    {
        target.transform.position = Vector3.up*10;
    }

    private void SetObjective2()
    {
        target.transform.position = Vector3.down*5 + Vector3.left*5;
    }
    private void SetObjective3()
    {
        target.transform.position = Vector3.down * 6 + Vector3.right * 11;
    }
    private void SetObjective4()
    {

    }
    private void SetObjective5()
    {

    }
}
