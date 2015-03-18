using UnityEngine;
using SpaceSandbox;
using Voxel2D;

public class Engine : VoxelData 
{
	// engine force in newtons
	public float engineForce = 0;

	[Range(0f, 1f)]
	private float engineSpeed = 1f;

	public bool enabled = false;

	private Rigidbody2D body;

	private ParticleSystem particle;
	
	public Engine(int elementID, Voxel2D.IntVector2 pos, int rotation, VoxelSystem voxel, float pullForce):base(elementID,pos,rotation, voxel){
		this.engineForce = pullForce;

		body = voxel.GetComponent<Rigidbody2D>();
		ParticleSetup();
	}

	public override void OnStart(params object[] input){


	}

	public override void OnActivate(params object[] input)
	{
		enabled = true;

	    if (input.Length >0)
	    {
	        engineSpeed = Mathf.Clamp01((float) input[0]);
	    }
	    else
	    {
	        engineSpeed = 1;
	    }

	    particle.emissionRate = 0;//100 * engineSpeed;
	}
	public override void OnDeactivate(params object[] input)
	{
		enabled = false;
		particle.emissionRate = 0;
	}

	public override void OnDelete()
	{
	    if (particle != null) Object.Destroy(particle.gameObject);
	}

    public override void OnUpdate()
	{
		if(enabled){
            if (voxel != null)
		    {
		        Vector3 v = Quaternion.Euler(0, 0, -rotation)*new Vector3(0, 1, 0);

		        Vector3 direction = voxel.transform.TransformDirection(v)*engineForce*engineSpeed;

		        Vector3 pos = new Vector3(position.x, position.y, 0);

		        body.AddForceAtPosition(direction, voxel.transform.TransformPoint(pos));
		    }
            else
            {
                OnDelete();
            }
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
