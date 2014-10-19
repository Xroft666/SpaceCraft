using UnityEngine;
using SpaceSandbox;
using Voxel2D;

public class Engine : VoxelData 
{
	// engine force in newtons
	public float engineForce = 0;

	private bool enabled = false;

	private Rigidbody2D body;

	private ParticleSystem particle;
	
	public Engine(int elementID, Voxel2D.IntVector2 pos, int rotation, VoxelSystem voxel, float pullForce):base(elementID,pos,rotation, voxel){
		deviceName = "Engine";
		this.engineForce = pullForce;

		body = voxel.rigidbody2D;
		ParticleSetup();
	}

	public override void OnStart(params object[] input){


	}

	public override void OnActivate(params object[] input)
	{
		enabled = true;
		particle.emissionRate = 100;
	}
	public override void OnDeactivate(params object[] input)
	{
		enabled = false;
		particle.emissionRate = 0;
	}

	public override void OnDelete(){
		Object.Destroy(particle.gameObject);
	}

	public override void OnUpdate()
	{
		if(enabled){
			Vector3 v = Quaternion.Euler(0,0,-rotation)*new Vector3(0,1,0);

			Vector3 direction = voxel.transform.TransformDirection(v)*engineForce;

			Vector3 pos = new Vector3(position.x,position.y,0);

			body.AddForceAtPosition(direction,voxel.transform.TransformPoint(pos));
		}
	}



	private void ParticleSetup(){
		GameObject g = new GameObject("Engine particle");
		g.transform.parent = voxel.transform;
		g.transform.localPosition = new Vector3(position.x,position.y,0);
		particle = g.AddComponent<ParticleSystem>();

		particle.emissionRate = 0;
		particle.simulationSpace = ParticleSystemSimulationSpace.World;

		particle.startSpeed = 50;
		particle.startLifetime = 0.1f;
		particle.transform.localRotation = Quaternion.Euler(new Vector3(rotation+90,90,0));

	}

}
