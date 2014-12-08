﻿using System;
using System.Text;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SharpNeat.Phenomes;
using Voxel2D;

public class ShipBuilderBrain : UnitController {

	Dictionary<Voxel2D.IntVector2,int> takenPosition = new Dictionary<Voxel2D.IntVector2, int>();

    public VoxelSystem voxelSystem{get; private set; }
	IBlackBox box;

	int[] blockCounts;

	int shipSize = 10;

	bool isRunning = false;

	float _closestDistance = Mathf.Infinity;

    private float _prevRot;

    private string objectName = "E1";
    private string path = Application.dataPath + "/Data/SavedVoxelSystems/";

	List<Engine> engines = new List<Engine>();
    public float StayOnTargetScore;

    private Optimizer optimizer;

	private VoxelSystem selectedAsteroid;
	private Transform selectedEnemyship;

	static public bool mineSignal = false;
    static public bool attackSignal = false;

    public SimulationStats Stats;

    #region Classes
    public class SimulationStats
    {
        public ShipBuilderBrain ShipBuilderBrain { get; private set; }

        public SimulationStats(ShipBuilderBrain shipBuilderBrain)
        {
            if (shipBuilderBrain != null)
            {
                ShipBuilderBrain = shipBuilderBrain;
            }
            else { Debug.LogError("shipbuilderbrain is null!");}
        }

        public float Fitness { get; set; }

        public enemyDamage EnemyDamage = new enemyDamage();
        public astroidDamage AstroidDamage = new astroidDamage();
        public int ObsticleHits = 0;
        public float UsedFuel = 0;
    }
    public class enemyDamage
    {
        public float Damage { get; private set; }
        public int Hits { get; private set; }

        public void Hit(float damage)
        {
            Damage += damage;
            Hits++;
        }
    }
    public class astroidDamage
    {
        public float Damage { get; private set; }
        public int Hits { get; private set; }

        void Hit(float damage)
        {
            Damage += damage;
            Hits++;
        }
    }
#endregion 
    enum BlockType
	{
		engine
	}
	
	// Use this for initialization
	void Awake () {

		gameObject.layer = 8;
	    gameObject.transform.position = Vector3.zero;
	
		voxelSystem = gameObject.AddComponent<VoxelSystem>();
		voxelSystem.rigidbody2D.drag = 1;
	    voxelSystem.rigidbody2D.angularDrag = 1;
		voxelSystem.SetGridSize(shipSize);

		objectName = Optimizer.fileName;
	}
	
	// Update is called once per frame
	void Update () {

		if( isRunning )
		{

			ISignalArray inputArr = box.InputSignalArray;
			FillInputs(ref inputArr);

			box.Activate();
			
			ISignalArray outputArr = box.OutputSignalArray;

            ActivateEngines(outputArr);
			
            ActivateCannon(outputArr);

            CalculateAditionalData();
		}
	}

    private void CalculateAditionalData()
    {
        float currentDist = (voxelSystem.transform.position - GotoTarget.Position).magnitude;
        if (currentDist < _closestDistance)
            _closestDistance = currentDist;

        _prevRot = voxelSystem.transform.rotation.eulerAngles.z;
    }
    private float[] ActivateRangeFinders()
    {
		const int sensorsCount = 16;
		const float SensorRange = 20f;

		RaycastHit2D hit;
		LayerMask mask = 1 << 9; // "Obstacles" layer

		float[] sensors = new float[sensorsCount];
		for( int i = 0; i < sensorsCount; i++ )
		{
			Vector3 direction = new Vector3( Mathf.Cos (i * (2f * Mathf.PI / (float) sensorsCount) ), Mathf.Sin(i * (2f * Mathf.PI / (float) sensorsCount) ) , 0f );

			hit = Physics2D.Raycast(transform.TransformPoint(voxelSystem.GetCenter()), transform.TransformDirection(direction), SensorRange, mask);
			if( hit.collider != null )
				sensors[i] = 1f - hit.distance / SensorRange;
		}

		return sensors;
    }
    private void ActivateEngines(ISignalArray outputArr)
    {
        engines.Clear();

        foreach (VoxelData e in voxelSystem.GetVoxelData())
            if (e is Engine)
                engines.Add((Engine)e);

        for (int i = 0; i < engines.Count; i++)
        {
            float pullForce = (float)outputArr[i];
            engines[i].OnActivate(pullForce);
            Stats.UsedFuel += pullForce;
        }
    }

    private void ActivateCannon(ISignalArray outputArr)
    {
        if (outputArr[6] > 0.5f)
        {
            Cannon cannon = null;

            foreach (VoxelData c in voxelSystem.GetVoxelData())
                if (c is Cannon)
                    cannon = (Cannon) c;
            if (cannon != null) cannon.fire(this);
        }
    }
    public override void SetOptimizer(Optimizer o)
    {
        this.optimizer = o;
    }

    public void FillInputs(ref ISignalArray inputArr)
    {
        Vector3 shipPos = voxelSystem.transform.TransformPoint(voxelSystem.GetCenter());
		Vector3 moveToPos;


		Vector3 targetVelocity = Vector3.zero;
		Vector3 targetDir = Vector3.up;

		if( mineSignal )
		{
			moveToPos = selectedAsteroid.transform.TransformPoint(selectedAsteroid.GetCenter());
			targetVelocity = selectedAsteroid.rigidbody2D.velocity;
			targetDir = selectedAsteroid.transform.up;
		}
		else if( attackSignal )
		{
			moveToPos = selectedEnemyship.transform.position;
			targetVelocity = selectedEnemyship.rigidbody2D.velocity;
			targetDir = selectedEnemyship.transform.up;
		}
		// if just go to command
		else
		{
			moveToPos = GotoTarget.Position;
		}

		Vector3 toTargetDir = (moveToPos - shipPos).normalized;

//        Vector3 shipDir = voxelSystem.transform.up;
//		float angle = Vector3.Angle(shipDir, toTargetDir);
//        Vector3 cross = Vector3.Cross(shipDir, toTargetDir);
//        if (cross.y < 0) angle = -angle;
//        angle = angle / 360f;

		// angle to the target
//        inputArr[0] = angle;

		// distance to the target
		inputArr[0] = Mathf.Clamp01((shipPos - moveToPos).magnitude / 100f);

		Vector3 toTargetLocalDir = voxelSystem.transform.InverseTransformVector(toTargetDir);

		// remapping to (0;1) domain, and 90 deg become (0.5, 0.5)
		toTargetLocalDir += Vector3.one;
		toTargetLocalDir /= 2f;

//		toTargetLocalDir.x = (Mathf.Sin( toTargetLocalDir.x * Mathf.PI ) + 1f ) / 2f;
//		toTargetLocalDir.y = (Mathf.Cos( toTargetLocalDir.y * Mathf.PI ) + 1f ) / 2f;

		// relative direction to the target
		inputArr[1] = toTargetLocalDir.x;
		inputArr[2] = toTargetLocalDir.y;

		// this ship's velocity and and angular velocity
        inputArr[3] = Mathf.Clamp01(voxelSystem.rigidbody2D.velocity.magnitude / 100f);
        inputArr[4] = Mathf.Clamp01(voxelSystem.rigidbody2D.angularVelocity / 100f);

		// target's velocity
		inputArr[5] = targetVelocity.magnitude / 100f;

		targetDir = voxelSystem.transform.InverseTransformDirection( targetDir );

		// remapping to (0;1) domain, and 90 deg become (0.5, 0.5)
		targetDir += Vector3.one;
		targetDir /= 2f;

//		targetDir.x = (Mathf.Sin( targetDir.x * Mathf.PI ) + 1f ) / 2f;
//		targetDir.y = (Mathf.Cos( targetDir.y * Mathf.PI ) + 1f ) / 2f;

		// target's direction
		inputArr[6] = targetDir.x;
		inputArr[7] = targetDir.y;

		// combinations:
		// 0, 0: GO TO COMMAND
		// 0, 1: MINE COMMAND
		// 1, 0: ATTACK A SHIP COMMAND
		inputArr[8] = mineSignal ? 1f : 0f;
		inputArr[9] = attackSignal ? 1f : 0f;


		float[] sensors = ActivateRangeFinders();
		for( int i = 0; i < sensors.Length; i++ )
			inputArr[i + 10] = sensors[i];
    }

	// replacable by SELECT ASTEROID
	public VoxelSystem SearchForClosestAsteroid()
	{
		float minDist = Mathf.Infinity;
		VoxelSystem closestAsteroid = null;

		for( int i = 0; i < AstroidGenerator.generatedAsteroidList.Count; i++ )
		{
			VoxelSystem asteroid = AstroidGenerator.generatedAsteroidList[i];
		
			float distance = ( voxelSystem.transform.TransformPoint( voxelSystem.GetCenter() ) - 
			                           asteroid.transform.TransformPoint( asteroid.GetCenter() )).magnitude;
		
			if( distance < minDist )
			{
				minDist = distance;
				closestAsteroid = asteroid;
			}
		}

		return closestAsteroid;
	}

	// replacable by SELECT SHIP
	public Transform SearchForClosestShip()
	{
		float minDist = Mathf.Infinity;
        Transform closestShip = null;

//		List<UnitController> ships = Optimizer.Units;
		GameObject[] ships = GameObject.FindGameObjectsWithTag("Enemy");

		for( int i = 0; i < ships.Length; i++ )
		{
            Transform otherShip = ships[i].transform;//GetComponent<VoxelSystem>();
		
			float distance = ( transform.position  - 
			                           ships[i].transform.position).magnitude;
			if( distance < minDist )
			{
				minDist = distance;
                closestShip = otherShip;
			}
		}

		return closestShip;
	}


	IEnumerator StopCall(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		isRunning = false;
	}
	
	public override void Activate(IBlackBox box, params object[] blackBoxExtraData){
		this.box = box;

		isRunning = true;

        Stats = new SimulationStats(this);

	    StayOnTargetScore = 0;

        LoadShipFromFile();

		selectedEnemyship = SearchForClosestShip();
		selectedAsteroid = SearchForClosestAsteroid();
	}

    
    private void LoadShipFromFile()
    {
        VoxelSystemDataConverter VSD = Serializer.Load<VoxelSystemDataConverter>(path + objectName + ".space");
        VoxelSystem voxelSystem = GetComponent<VoxelSystem>();
        VSD.FillVoxelSystem(ref voxelSystem);
        voxelSystem.ForceUpdate();
        voxelSystem.transform.position -= (Vector3)voxelSystem.GetCenter();
    }

	public override void Stop(){
		Destroy(voxelSystem.gameObject);
	}
	
	public override float GetFitness()
	{

	    float fitness = optimizer.objective.GetFitness(this);
	    //Stats.Fitness = fitness;
        return fitness;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
       Stats.ObsticleHits++; 
	}

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Target")
        {
            StayOnTargetScore += 1f;
        }
    }
}
