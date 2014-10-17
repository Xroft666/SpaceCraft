using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

		private List<GameObject> fragmentList = new List<GameObject>();

		// Use this for initialization
		void Awake () {
			voxel = GetComponent<Voxel2D.VoxelSystem>();
			//print(voxel);
		}
		
		// Update is called once per frame
		void LateUpdate () {
			foreach(GameObject g in fragmentList){
				g.SetActive(true);
			}
			fragmentList.Clear();
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

			IEnumerator<IntVector2>[] testList = new IEnumerator<IntVector2>[col.contacts.Length];
			for(int i=0;i<testList.Length;i++){
				Vector2 pos = PosGlobalToLocal(col.contacts[i].point);
				testList[i] = voxel.GetClosestVoxelIndexIE(pos,15).GetEnumerator();
			}

			//TODO;: use material based impact threshhold
			int loopCounter = 50;
			while(totalImpactEnergy-energyAbsorbed > 0 && loopCounter >0){
				loopCounter--;
				for (int i = 0; i < col.contacts.Length; i++) {
					if(col.contacts[i].collider.tag != "VoxelFragment"){
						if(testList[i].MoveNext()){
							IntVector2 test = testList[i].Current;

							if(DestroyCollidingVoxel(col.contacts[i].point,totalImpactEnergy,ref energyAbsorbed, test)){

							}else{
								break;
							}
						}

					}
				}
			}
			
			
		}

		bool DestroyCollidingVoxel(Vector2 colPoint, float totalImpactEnergy, ref float energyAbsorbed, IntVector2 pos){
			//Vector2 pos = PosGlobalToLocal(colPoint);
			
			VoxelData vox = voxel.GetVoxel(pos.x,pos.y);
			if(vox != null){
				if(VoxelDestroyed != null){
					VoxelDestroyed(voxel,vox.GetPosition());
				}
				
				if(totalImpactEnergy-energyAbsorbed>vox.stats.destructionEnergy){
					energyAbsorbed += vox.stats.destructionEnergy;
					
					int voxelID = vox.GetID();
					//print(voxelID);
						GameObject fragment = VoxelUtility.CreateFragment(voxelID, colPoint, voxel);
					//fragment.SetActive(false);
					//fragmentList.Add(fragment);

					IntVector2 iv2 = vox.GetPosition();
					voxel.RemoveVoxel(iv2.x,iv2.y);
					return true;
					
					
					//TODO: use material based impact threshhold
				}else{
					energyAbsorbed = totalImpactEnergy;
				}
			}
			return false;
		}

		Vector2 PosGlobalToLocal(Vector2 pos){
			pos = transform.InverseTransformPoint(pos);
			pos.x = Mathf.Round(pos.x);
			pos.y = Mathf.Round(pos.y);
			return pos;
		}
		
		
		
	}
}
