using UnityEngine;
using System.Collections;
using Voxel2D;

namespace MaterialSystem{
	public class PropertyOverlay : MonoBehaviour {
		
		enum Property
		{
			element,
			temperature,
			fragmentation
		}
		
		VoxelSystem voxel;
		VoxelFragment frag;

		Property property = Property.temperature;
		
		MeshFilter filter;
		
		Mesh mesh;
		
		void Awake(){
			voxel = GetComponent<VoxelSystem>();
			frag = GetComponent<VoxelFragment>();
			filter = GetComponent<MeshFilter>();
		}
		
		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			mesh = filter.mesh;
			
			Color[] colorArray = mesh.colors;
			VoxelData[,] data = new VoxelData[0,0];
			if(voxel != null){
				data = voxel.GetVoxelData();
			}else if(frag != null){
				data = new VoxelData[1,1];
				data[0,0] = frag.voxel;
			}
			for (int x = 0; x < data.GetLength(0); x++) {
				for (int y = 0; y < data.GetLength(1); y++) {
					if(data[x,y] != null){
						float value;
						switch(property){
						case Property.temperature:
							value = data[x,y].stats.temperature/data[x,y].stats.e.melting;
							colorArray[data[x,y].vertexIndexes[0]] = new Color(value,0,1-value);
							colorArray[data[x,y].vertexIndexes[1]] = new Color(value,0,1-value);
							colorArray[data[x,y].vertexIndexes[2]] = new Color(value,0,1-value);
							colorArray[data[x,y].vertexIndexes[3]] = new Color(value,0,1-value);
							break;	
						case Property.fragmentation:
							value = data[x,y].stats.fragmention;
							colorArray[data[x,y].vertexIndexes[0]] = new Color(value,0,0);
							colorArray[data[x,y].vertexIndexes[1]] = new Color(value,0,0);
							colorArray[data[x,y].vertexIndexes[2]] = new Color(value,0,0);
							colorArray[data[x,y].vertexIndexes[3]] = new Color(value,0,0);
							break;
						case Property.element:
							//value = data[x,y].stats.e.color;
							colorArray[data[x,y].vertexIndexes[0]] =  data[x,y].stats.e.color;
							colorArray[data[x,y].vertexIndexes[1]] =  data[x,y].stats.e.color;
							colorArray[data[x,y].vertexIndexes[2]] =  data[x,y].stats.e.color;
							colorArray[data[x,y].vertexIndexes[3]] =  data[x,y].stats.e.color;
							break;
						}
					}
				}
			}
			mesh.colors = colorArray;
			
		}
	}
}
