using System;
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

	bool invalid = false;

	bool isRunning = false;

    public int _wallHits = 0; 
	float _closestDistance = Mathf.Infinity;

    public float _totalRotation = 0;
    private float _prevRot;


    private string objectName = "E1";
    private string path = Application.dataPath + "/Data/SavedVoxelSystems/";

	List<Engine> engines = new List<Engine>();
    public float _targetScore;

    private Optimizer optimizer;

	private VoxelSystem selectedAsteroid;
	private VoxelSystem selectedEnemyship;

	private bool mineSignal = false;
	private bool attackSignal = false;

    enum BlockType
	{
		engine
	}
	
	// Use this for initialization
	void Awake () {

//		GameObject g = new GameObject("Ship");
		gameObject.layer = 8;
		gameObject.transform.position = Vector3.zero;//new Vector3(Random.Range(-10,10),Random.Range(-10,10),0);
	
		voxelSystem = gameObject.AddComponent<VoxelSystem>();
		voxelSystem.rigidbody2D.drag = 1;
	    voxelSystem.rigidbody2D.angularDrag = 1;
		voxelSystem.SetGridSize(shipSize);
	}
	
	// Update is called once per frame
	void Update () {
		// testing goes here
		// let's say we need to move the vessel from A to B through bunch of obstalces

		// here we use the brain:
		// let's assume that we have inputs:
		// 	- sensors which tell where are the obstacles
		//  the output would be:
		//  - which direction to take

		// and the fitness would be distance to the goal

		if( isRunning )
		{
			
            /*
            float frontSensor = 0f;
			float leftFrontSensor = 0f;
			float leftSensor = 0f;
			float rightFrontSensor = 0f;
			float rightSensor = 0f;
			float SensorRange = 10f;

			RaycastHit2D hit;
			LayerMask mask = 1 << 9; // "Obstacles" layer

			hit = Physics2D.Raycast(transform.position, transform.TransformDirection(new Vector3(0f, 1f, 0f).normalized), SensorRange, mask);
			if( hit.collider != null )
				frontSensor = 1f - hit.distance / SensorRange;

			hit = Physics2D.Raycast(transform.position, transform.TransformDirection(new Vector3(0.5f, 1f, 0f).normalized), SensorRange, mask);
			if( hit.collider != null )
				rightFrontSensor = 1f - hit.distance / SensorRange;

			hit = Physics2D.Raycast(transform.position, transform.TransformDirection(new Vector3(1f, 0f, 0f).normalized), SensorRange, mask);
			if( hit.collider != null )
				rightSensor = 1f - hit.distance / SensorRange;

			hit = Physics2D.Raycast(transform.position, transform.TransformDirection(new Vector3(-0.5f, 1f, 0f).normalized), SensorRange, mask);
			if( hit.collider != null )
				leftFrontSensor = 1f - hit.distance / SensorRange;

			hit = Physics2D.Raycast(transform.position, transform.TransformDirection(new Vector3(-1f, 0f, 0f).normalized), SensorRange, mask);
			if( hit.collider != null )
				leftSensor = 1f - hit.distance / SensorRange;
            */

			ISignalArray inputArr = box.InputSignalArray;
			FillInputs(ref inputArr);
            /*
            inputArr[0] = frontSensor;
			inputArr[1] = leftFrontSensor;
			inputArr[2] = leftSensor;
			inputArr[3] = rightFrontSensor;
			inputArr[4] = rightSensor;

			inputArr[5] = (voxelSystem.transform.position - GotoTarget.Position).magnitude;
			*/
			box.Activate();
			
			ISignalArray outputArr = box.OutputSignalArray;

			engines.Clear();

			foreach(VoxelData e in voxelSystem.GetVoxelData())
				if(e is Engine)
					engines.Add( (Engine)e );

			for( int i = 0; i < engines.Count; i++ )
			{
				float pullForce = (float) outputArr[i];
				engines[i].OnActivate( pullForce );
			}
			
//			float steer = (float)outputArr[0] * 2f - 1f;
//			float gas = (float)outputArr[1] * 2f - 1f;

//			List<Engine> leftEngines = new List<Engine>();
//			List<Engine> rightEngines = new List<Engine>();
//			List<Engine> forwardEngines = new List<Engine>();
//			List<Engine> backwardEngines = new List<Engine>();
			
//			CollectThrusters(ref leftEngines, ref rightEngines, ref forwardEngines, ref backwardEngines);
//			InputThrusters(gas, steer, ref leftEngines, ref rightEngines, ref forwardEngines, ref backwardEngines);


			float currentDist = (voxelSystem.transform.position - GotoTarget.Position).magnitude;
			if( currentDist < _closestDistance )
				_closestDistance = currentDist;


		    _totalRotation += Mathf.Abs(voxelSystem.transform.rotation.eulerAngles.z - _prevRot);
		    _prevRot = voxelSystem.transform.rotation.eulerAngles.z;
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

		if( mineSignal )
		{
			moveToPos = selectedAsteroid.GetCenter();
		}
		else if( attackSignal )
		{
			moveToPos = selectedEnemyship.GetCenter();
		}
		// if just go to command
		else
		{
			moveToPos = GotoTarget.Position;
		}

		Vector3 targetDir = shipPos - moveToPos;
        Vector3 shipDir = voxelSystem.transform.up;

        float angle = Vector3.Angle(shipDir, targetDir);
        Vector3 cross = Vector3.Cross(shipDir, targetDir);
        if (cross.y < 0) angle = -angle;

        angle = angle / 360;

        inputArr[0] = angle;
		inputArr[1] = Mathf.Clamp((shipPos - moveToPos).magnitude / 100, 0, 1);


		// shouldnt it be relative to voxelSystem.GetCenter instead?
		Vector3 localDeltaPos = voxelSystem.transform.InverseTransformPoint(moveToPos);
		localDeltaPos.x = Mathf.Clamp01(localDeltaPos.x / 100f);
		localDeltaPos.y = Mathf.Clamp01(localDeltaPos.y / 100f);
		
		inputArr[2] = localDeltaPos.x;
		inputArr[3] = localDeltaPos.y;

        inputArr[4] = Mathf.Clamp(voxelSystem.rigidbody2D.velocity.magnitude, 0, 25);
        inputArr[5] = Mathf.Clamp(voxelSystem.rigidbody2D.angularVelocity, 0, 10);

        inputArr[6] = Mathf.Clamp((voxelSystem.transform.rotation.z - _prevRot), -1, 1);

		// im not sure, if distances should be the input, or vectors which show directions?
		// distance to the closest asteroid

		// combinations:
		// 0, 0: GO TO COMMAND
		// 0, 1: MINE COMMAND
		// 1, 0: ATTACK A SHIP COMMAND
		inputArr[7] = mineSignal ? 1f : 0f;
		inputArr[8] = attackSignal ? 1f : 0f;
    }

	public void CollectThrusters(ref List<Engine> left, ref List<Engine> right, ref List<Engine> forward, ref List<Engine> backward)
	{
		foreach(VoxelData e in voxelSystem.GetVoxelData())
		{
			if(e is Engine)
			{
				switch(e.rotation)
				{
				case 0:
					forward.Add((Engine)e);
					break;
				case 90:
					left.Add((Engine)e);
					break;
				case 270:
					right.Add((Engine)e);
					break;
				case 180:
					backward.Add((Engine)e);
					break;
				}
			}
		}
	}
	
    
	public void InputThrusters(float gas, float steer, ref List<Engine> left, ref List<Engine> right, ref List<Engine> forward, ref List<Engine> backward )
	{
		foreach(Engine e in forward)
			if( gas > 0f )
				e.OnActivate(gas);

		foreach(Engine e in forward)
			if( gas == 0f && e.enabled )
				e.OnDeactivate();

		foreach(Engine e in backward)
			if( gas > 0f )
				e.OnActivate(gas);

		foreach(Engine e in backward)
			if( gas == 0f && e.enabled )
				e.OnDeactivate();

		foreach(Engine e in right)
			if( steer > 0f )
				e.OnActivate(steer);

		foreach(Engine e in right)
			if( gas == 0f && e.enabled )
				e.OnDeactivate();

		foreach(Engine e in left)
			if( steer < 0f )
				e.OnActivate(steer);

		foreach(Engine e in left)
			if( gas == 0f && e.enabled )
				e.OnDeactivate();
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
	public VoxelSystem SearchForClosestShip()
	{
		float minDist = Mathf.Infinity;
		VoxelSystem closestShip = null;

		List<UnitController> ships = Optimizer.Units;

		for( int i = 0; i < ships.Count; i++ )
		{
			ShipBuilderBrain otherShip = ships[i] as ShipBuilderBrain;
			if( this  == otherShip )
				continue;
		
			float distance = ( voxelSystem.transform.TransformPoint( voxelSystem.GetCenter() ) - 
			                           otherShip.transform.TransformPoint( otherShip.voxelSystem.GetCenter() )).magnitude;
			if( distance < minDist )
			{
				minDist = distance;
				closestShip = otherShip.voxelSystem;
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

	    _targetScore = 0;
	    _totalRotation = 0;

        Load();

//		selectedEnemyship = SearchForClosestShip();
//		selectedAsteroid = SearchForClosestAsteroid();
	}

    
    private void Load()
    {
        VoxelSystemDataConverter VSD = Serializer.Load<VoxelSystemDataConverter>(path + objectName + ".space");
        VoxelSystem voxelSystem = GetComponent<VoxelSystem>();
        VSD.FillVoxelSystem(ref voxelSystem);
    }

	private void GenerateVoxelSystem(List<VoxelRawData> voxelData)
	{
		foreach( VoxelRawData voxel in voxelData )
		{
			Voxel2D.IntVector2 localCoord = new Voxel2D.IntVector2(voxel._xPos, voxel._yPos);
			
			if(!takenPosition.ContainsKey(localCoord) && voxelSystem.CanAddVoxel(localCoord))
			{
				takenPosition.Add(localCoord,voxel._deviceType);
				VoxelData vd = null;

				int elementType = voxel._materialType;
				
			    int rotationAngle = voxel._rotation*90;
				

				switch( voxel._deviceType )
				{
				case 0:
					vd = new Wall(elementType,localCoord,rotationAngle,voxelSystem);
					break;
				case 1:
					vd = new Cannon(elementType,localCoord,rotationAngle,voxelSystem,10,1);
					break;
				case 2:
					vd = new Laser(elementType,localCoord,rotationAngle,voxelSystem,250);
					break;
				case 3:
					vd = new Engine(elementType,localCoord,rotationAngle,voxelSystem,100);
					break;
				case 4:
					vd = new Wall(elementType,localCoord,rotationAngle,voxelSystem);
					break;
				default:
					break;
				}

				voxelSystem.AddVoxel(vd);
			}
		}
	}
	
	public override void Stop(){
		Destroy(voxelSystem.gameObject);
	}
	
	public override float GetFitness(){

	    return optimizer.objective.GetFitness(this);
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
        _wallHits++; 
	}

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Target")
        {
            _targetScore += 1f;
        }
    }
}
