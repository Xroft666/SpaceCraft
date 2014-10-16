using UnityEngine;
using System.Collections;
using MaterialSystem;

namespace Voxel2D{
	public class VoxelImpacter : MonoBehaviour {
		
		public delegate void VoxelDestroyedAction(Voxel2D.VoxelSystem voxelSystem, IntVector2 localPosition);
		public static event VoxelDestroyedAction VoxelDestroyed;
		
		Voxel2D.VoxelSystem voxel;
		
		Vector2 forceToAdd;
		Vector2 forcePoint;
		
		//HACK: should be calculated based on element stats
		private float impactEnergyThreshHold = 1000;
		
		// Use this for initialization
		void Awake () {
			voxel = GetComponent<Voxel2D.VoxelSystem>();
			print(voxel);
		}
		
		// Update is called once per frame
		void Update () {
			
		}
		
		void FixedUpdate(){
			if(forceToAdd != Vector2.zero){
				//rigidbody2D.AddForceAtPosition(forceToAdd,forcePoint);
				forceToAdd = Vector2.zero;
			}
		}
		
		void OnCollisionEnter2D(Collision2D col){ //TODO: include angular velocity

			float energyAbsorbed = 0;
			
			Vector2 deltaVelocityThis = rigidbody2D.velocity-voxel.previousVelocity[0];
			float massThis = voxel.totalMass;
			float impactEnergyThis = (massThis*Mathf.Pow(deltaVelocityThis.magnitude,2))*0.5f;
			
			Voxel2D.VoxelSystem voxelOther;
			Vector2 deltaVelocityOther = Vector2.zero;
			float massOther = 0;
			float impactEnergyOther = 0;
			
			voxelOther = col.gameObject.GetComponent<Voxel2D.VoxelSystem>();
			if(voxelOther != null){
				deltaVelocityOther = voxelOther.rigidbody2D.velocity-voxelOther.previousVelocity[0];
				massOther = voxelOther.totalMass;
				impactEnergyOther = (massOther*Mathf.Pow(deltaVelocityOther.magnitude,2))*0.5f;
			}
			
			float totalImpactEnergy = impactEnergyThis+impactEnergyOther;
			
			forceToAdd = -deltaVelocityThis.normalized*impactEnergyThis;
			forcePoint = col.contacts[0].point;
			
			//print(impactEnergyThis);

			//TODO: use material based impact threshhold
			if(totalImpactEnergy-energyAbsorbed>impactEnergyThreshHold){
				for (int i = 0; i < col.contacts.Length; i++) {
					if(col.contacts[i].collider.tag != "VoxelFragment"){
						Vector2 pos = col.contacts[i].point;
						
						pos = transform.InverseTransformPoint(pos);
						pos.x = Mathf.Round(pos.x);
						pos.y = Mathf.Round(pos.y);
						
						IntVector2? vox = voxel.GetClosestVoxelIndex(pos,7);
						if(vox.Value.x >=0){
							IntVector2 voxNotNull = vox.Value;
							if(VoxelDestroyed != null){
								VoxelDestroyed(voxel,voxNotNull);
							}
							VoxelData theVoxel = voxel.GetVoxel(voxNotNull.x,voxNotNull.y);

							energyAbsorbed += theVoxel.stats.destructionEnergy;

							int voxelID = theVoxel.GetID();
							VoxelUtility.CreateFragment(voxelID, col.contacts[i].point, voxel);
							voxel.RemoveVoxel(voxNotNull.x,voxNotNull.y);



							//TODO: use material based impact threshhold
						}
					}
				}
			}else{
				//print("too low energy");
			}
			
		}



	}
}
