
using System;
using System.Collections.Generic;
using System.Linq;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class ObjectiveHandler:MonoBehaviour
{

    private readonly float[] _targetFitnes = {300,200,150,150, 5,5};

    private int _currentObjective;
    private int _checkingObjective;

    public GameObject Maze;
    public GameObject Enemy;

    public int ObjectiveNum { get {
        if (_checkingObjectives)
        {
            return _checkingObjective;
        }
        else
        {
            return _currentObjective;
        }
    } }

    private GameObject _target;

    private readonly List<Action> _objectiveList = new List<Action>();


    private bool _checkingObjectives;

    public Optimizer Optimizer;

    EvolutionDataHistory dataHistory;

    public void Start()
    {
       

        

        InitTarget();
        
        
        _objectiveList.Add(ObjectiveRandomPos);
        _objectiveList.Add(SetObjectiveMovingTarget);
        _objectiveList.Add(SetObjectiveMaze);
		_objectiveList.Add(SetObjectiveGetOutOfMaze);

		_objectiveList.Add(SetObjectiveShootEnemy);
		_objectiveList.Add(SetObjectiveMazeAndShoot);


    }

    private void InitTarget()
    {
        _target = new GameObject();
        _target.AddComponent<MeshRenderer>();
        MeshFilter f = _target.AddComponent<MeshFilter>();
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        f.mesh = g.GetComponent<MeshFilter>().mesh;
        Object.Destroy(g);
        CircleCollider2D c = _target.AddComponent<CircleCollider2D>();
        c.radius = 0.5f;
        c.isTrigger = true;
        _target.transform.tag = "Target";
        _target.name = "Target";
        _target.AddComponent<GotoTarget>();
    }

    public void NextGen()
    {
        if (dataHistory == null)
        {
            dataHistory = new EvolutionDataHistory(Optimizer._ea, Optimizer.fileName, Optimizer);
        }
        
        NeatEvolutionAlgorithm<NeatGenome> ea = Optimizer._ea;

        
        float meanFit = getMeanFitness(ea);

        dataHistory.NextGeneration(meanFit, _targetFitnes[_currentObjective]);

        
        

        if (!_checkingObjectives)
        {
            if (meanFit >= 1f )//_targetFitnes[_currentObjective])
            {
                _currentObjective++;
                if (_currentObjective == _objectiveList.Count)
                {
                    _currentObjective = 0;
                }
                _checkingObjectives = true;
                _checkingObjective = 0;
                _objectiveList[_checkingObjective]();
                Debug.Log("OBJECTIVE " + (_currentObjective - 1) + " COMPLETED, CHECKING OBJECTIVES" + "... " + meanFit + "/" + _targetFitnes[_currentObjective]);
            }
            else
            {
                _objectiveList[_currentObjective]();
                Debug.Log("TRAINING OBJECTIVE " + (_currentObjective) + "... "+meanFit+"/"+_targetFitnes[_currentObjective]);
            }
        }
        else
        {
            if (meanFit >= 1f )//_targetFitnes[_currentObjective])
            {
                if (_checkingObjective < _currentObjective)
                {
                    _checkingObjective ++;
                    _objectiveList[_checkingObjective]();
					Debug.Log("CHECKING OBJECTIVE " + (_checkingObjective - 1) + " COMPLETED" + "... " + meanFit + "/" + _targetFitnes[_checkingObjective]);
                }
                else
                {
                    _checkingObjectives = false;
                    _objectiveList[_currentObjective]();
                    Debug.Log("CHECKING OBJECTIVES COMPLETE, BACK TO TRAINING" + "... " + meanFit + "/" + _targetFitnes[_currentObjective]);
                }
            }
            else
            {
                _checkingObjectives = false;
                _currentObjective = _checkingObjective;
                _objectiveList[_currentObjective]();
                _checkingObjective = 0;
                Debug.Log("OBJECTIVE " + _currentObjective + " FAILED, RETRAINING THIS OBJECTIVE" + "... " + meanFit + "/" + _targetFitnes[_currentObjective]);
            }
        }

    }

    private float getMeanFitness(NeatEvolutionAlgorithm<NeatGenome> ea)
    {
        List<float> fitList = ea.GenomeList.Select(n => (float) n.EvaluationInfo.Fitness).ToList();
        fitList.Sort();
        //fitList.Reverse();

        for (int i = ea.GenomeList.Count / 2 - 1; i >= 0; i--)
        {
            fitList.RemoveAt(i);
        }

        return fitList.Sum() / fitList.Count;
    }

    public float GetFitness(ShipBuilderBrain s)
    {
        float fitness =0;
        int t = _checkingObjectives ? _checkingObjective : _currentObjective;

        switch( t )
		{
		case 0:
		case 1:
		case 2:
        case 3:
            fitness = FitnessFunctions.GetFitnessStayOnTarget(s, _targetFitnes[t]);
			break;
		case 4:
		case 5:
            fitness = FitnessFunctions.GetFitnessHitTarget(s, _targetFitnes[t]);
			break;
        }
        return fitness;
    }

    private void ResetScene()
    {
        _target.transform.position = Vector3.zero;
        Enemy.SetActive(false);
        Maze.SetActive(false);

		ShipBuilderBrain.attackSignal = false;
		ShipBuilderBrain.mineSignal = false;
    }

    private void SetObjectiveMaze()
    {
        ResetScene();
        _target.transform.position = new Vector3(30, 0, 0);

        Maze.SetActive(true);
    }

	private void SetObjectiveGetOutOfMaze()
	{
		ResetScene();
		_target.transform.position = new Vector3(65f, 13f, 0);
		
		Maze.SetActive(true);
	}

    private void SetObjectiveShootEnemy()
    {
        ResetScene();
		Enemy.SetActive(true);
        Enemy.transform.position = new Vector3(Random.Range(-30, 30), Random.Range(-30, 30), 0);
        while (Enemy.transform.position.magnitude < 20)
        {
            Enemy.transform.position = new Vector3(Random.Range(-30, 30), Random.Range(-30, 30), 0);
        }
		_target.transform.position = Enemy.transform.position;

		ShipBuilderBrain.attackSignal = true;
    }

	private void SetObjectiveMazeAndShoot()
	{
		ResetScene();
		Enemy.SetActive(true);
		Maze.SetActive(true);
        Enemy.transform.position = new Vector3(65f, 13f, 0);
		_target.transform.position = Enemy.transform.position;

		ShipBuilderBrain.attackSignal = true;
	}

    private void ObjectiveRandomPos()
    {
        ResetScene();
        _target.transform.position = new Vector3(Random.Range(-20,20), Random.Range(-20,20),0);
        while (_target.transform.position.magnitude < 10)
        {
            _target.transform.position = new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), 0);  
        }
    }
    private void SetObjectiveMovingTarget()
    {
        ResetScene();
        _target.transform.position = new Vector3(10, -10, 0);
        
        Hashtable h = new Hashtable
        {
            {"position", new Vector3(10, 10, 0)},
            {"time", Optimizer.TrialDuration - 0.1f},
            {"easetype", "linear"}
        };

        iTween.MoveTo(_target,h);
        
    }

}
