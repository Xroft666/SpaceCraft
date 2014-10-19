using UnityEngine;
using System.Collections;

namespace Voxel2D{
	public class VoxelTextureHandler:MonoBehaviour{

		void Awake(){

			renderer.sharedMaterial = TextureHolder.Instance.DeviceMaterial;
		}
	
	}
}
