using UnityEngine;
using System.Collections;
using Voxel2D;

public class Cannon : VoxelData {
	
	private float launchForce = 0;
	private float projectileMass;

	private bool enabled = false;


    private float fireRate = 1;
    private float coolDown;

	private Rigidbody2D body;
	
	public Cannon(int elementID, Voxel2D.IntVector2 pos, int rotation, VoxelSystem voxel, float launchForce, float projectileMass):base(elementID,pos,rotation, voxel){
		this.launchForce = launchForce;
		this.projectileMass = projectileMass;
		body = voxel.rigidbody2D;
		
	}
	
	
	public void fire(ShipBuilderBrain owner = null){

	    if (coolDown < 0)
	    {
	        coolDown = fireRate;
	        Vector3 v = Quaternion.Euler(0, 0, rotation)*new Vector3(0, 1, 0);

	        Vector3 direction = voxel.transform.TransformDirection(v);
	        Vector3 globalPos = voxel.transform.TransformPoint(new Vector3(position.x, position.y, 0));

	        Vector3 pos = new Vector3(position.x, position.y, 0);

	        GameObject g =
	            GameObject.Instantiate(TextureHolder.Instance.bulletPrefab, globalPos + direction.normalized*1.1f,
	                Quaternion.Euler(new Vector3(0, 0, rotation))) as GameObject;
	        g.AddComponent<Projectile>();

	        g.rigidbody2D.velocity = voxel.rigidbody2D.velocity;
	        g.rigidbody2D.mass = projectileMass;
	        g.rigidbody2D.AddForce(direction.normalized*launchForce);


	        body.AddForceAtPosition(-direction.normalized*launchForce, voxel.transform.TransformPoint(pos));

			if( owner != null )
				g.GetComponent<BulletController>().owner = owner;
	    }
	}
	
	public override void OnUpdate(){
		if(Input.GetKeyDown(KeyCode.LeftControl)){
			fire();
		}
	    coolDown -= Time.deltaTime;
        
	}
}
