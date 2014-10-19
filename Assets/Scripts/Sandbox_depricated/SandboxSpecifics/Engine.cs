using UnityEngine;
using SpaceSandbox;
using Voxel2D;

public class Engine : VoxelData 
{
	// engine force in newtons
	public float pullForce = 1000;

	public float Speed;

	private bool enabled = false;

	public Vector2 position = Vector2.zero;
	public float rotation = 0f;

	private VoxelSystem voxel;
	private Rigidbody2D body;

	private ParticleSystem particle;

	public override void OnStart(params object[] input){
		voxel = input[0] as VoxelSystem;
		Vector2 pos = (Vector2)input[1];
		pullForce = (float)input[2];
		rotation = (float)input[3];

		position = pos;
		body = voxel.rigidbody2D;

		GameObject g = new GameObject("Engine particle");
		g.transform.parent = voxel.transform;
		g.transform.localPosition = new Vector3(pos.x,pos.y,0);
		particle = g.AddComponent<ParticleSystem>();

		ParticleSetup();
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

			Vector3 direction = voxel.transform.TransformDirection(v)*pullForce;
			
			body.AddForceAtPosition(direction,voxel.transform.TransformPoint(position));
		}
	}



	private void ParticleSetup(){
		particle.emissionRate = 0;
		particle.simulationSpace = ParticleSystemSimulationSpace.World;

		particle.startSpeed = 50;
		particle.startLifetime = 0.1f;
		particle.transform.localRotation = Quaternion.Euler(new Vector3(rotation+90,90,0));
	}

}
