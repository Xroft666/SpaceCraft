using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MaterialSystem;

namespace Voxel2D{
	public class VoxelImpacter : MonoBehaviour {
		
		public delegate void VoxelDestroyedAction(Voxel2D.VoxelSystem voxelSystem, IntVector2 localPosition);
		public static event VoxelDestroyedAction VoxelDestroyed;
		
		Voxel2D.VoxelSystem voxel;
		
		private List<GameObject> fragmentList = new List<GameObject>();
		
		// Use this for initialization
		void Awake () {
			voxel = GetComponent<Voxel2D.VoxelSystem>();
		}
		
		// Update is called once per frame
		void LateUpdate () {
			foreach(GameObject g in fragmentList){
				g.SetActive(true);
			}
			fragmentList.Clear();
		}
		
		
		
		void OnCollisionEnter2D(Collision2D col){ 
			
			StartCoroutine(HandleCollision(col));
			
		}
		
		IEnumerator HandleCollision(Collision2D col){	//TODO: include angular velocity
			
			float totalImpactEnergy = calculateTotalImpactForce(col);
			if(totalImpactEnergy >0){
				
				List<VoxelData> voxelList = new List<VoxelData>();
				
				
				//extract local colission points
				List<Vector2> colPos = new List<Vector2>();
				for(int i=0;i<1;i++){
					if(col.contacts[i].collider.tag != "VoxelFragment"){
						colPos.Add(PosGlobalToLocal(col.contacts[i].point));
					}
				}
				if(colPos.Count>0){
					object[] o = new object[]{
						totalImpactEnergy,
						colPos.ToArray(),
						voxelList
					};
					
					Thread t = new Thread(() => CollectVoxels(o));
					t.Start(o);
					while(t.IsAlive){
						yield return new WaitForEndOfFrame();
					}
					//wait (hopefully) for collider to be generated //TODO: base this on event
					yield return new WaitForEndOfFrame();
					CreateFragments(voxelList);
				}
			}
		}
		
		void CollectVoxels(object[] o){
			
			float totalImpactEnergy = (float)o[0];
			Vector2[] colPos = (Vector2[])o[1];
			List<VoxelData> voxelList = (List<VoxelData>)o[2];
			
			float energyAbsorbed =0;
			
			IEnumerator<IntVector2>[] ClosestVoxelIEnum = new IEnumerator<IntVector2>[colPos.Length];
			for(int i=0;i<ClosestVoxelIEnum.Length;i++){
				ClosestVoxelIEnum[i] = voxel.GetClosestVoxelIndexIE(colPos[i],15).GetEnumerator();
			}
			int loopCounter = 150;
			while(totalImpactEnergy-energyAbsorbed > 0 && loopCounter >0){
				loopCounter--;
				for (int i = 0; i < colPos.Length; i++) {
					if(ClosestVoxelIEnum[i].MoveNext()){
						IntVector2 closestVoxel = ClosestVoxelIEnum[i].Current;
						VoxelData vox = voxel.GetVoxel(closestVoxel.x,closestVoxel.y);
						if(vox != null){
							if(totalImpactEnergy-energyAbsorbed>vox.stats.destructionEnergy){
								energyAbsorbed += vox.stats.destructionEnergy;
								voxelList.Add(vox);
							}else{
								//add fragment value voxel stats
								energyAbsorbed = totalImpactEnergy;
							}
						}
					}else{
						break;
					}
				}
			}
		}
		
		void CreateFragments(List<VoxelData> fragmentList){
			if(fragmentList.Count >0){
				for(int i=0;i<fragmentList.Count;i++){
					//foreach(VoxelData vox in fragmentList){
					if(fragmentList[i] != null){
						GameObject fragment = VoxelUtility.CreateFragment(fragmentList[i],PosLocalToGlobal(fragmentList[i].GetPosition()), voxel);
						voxel.RemoveVoxel(fragmentList[i].GetPosition().x,fragmentList[i].GetPosition().y);
						
						//event call
						if(VoxelDestroyed != null){
							VoxelDestroyed(voxel,fragmentList[i].GetPosition());
						}
					}
				}
			}
		}
		
		float calculateTotalImpactForce(Collision2D col){	//TODO: use col.relative velocity and current relative velocity to calculate energy
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

			return impactEnergyThis+impactEnergyOther;
		}
		
		Vector2 PosGlobalToLocal(Vector2 pos){
			pos = transform.InverseTransformPoint(pos);
			pos.x = Mathf.Round(pos.x);
			pos.y = Mathf.Round(pos.y);
			return pos;
		}
		
		Vector2 PosLocalToGlobal(IntVector2 pos){
			return transform.TransformPoint(new Vector3(pos.x,pos.y,0));
		}
		
	}
}
