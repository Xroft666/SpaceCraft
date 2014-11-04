using UnityEngine;
using System.Collections;

namespace Voxel2D{
	public class VoxelTextureHandler:MonoBehaviour{

		void Awake(){
			if(TextureHolder.Instance !=null)
			renderer.sharedMaterial = TextureHolder.Instance.DeviceMaterial;
		}
	
	}
}
