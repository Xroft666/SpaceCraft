using UnityEngine;
using System.Collections;

namespace Voxel2D{
	public class VoxelTextureHandler:MonoBehaviour{

		void Awake(){
			renderer.material = TextureHolder.Instance.TileMaterial;
		}
	
	}
}
