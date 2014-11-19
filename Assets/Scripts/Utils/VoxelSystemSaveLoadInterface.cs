
using UnityEngine;
using System.Collections;
using Voxel2D;

[RequireComponent(typeof(VoxelSystem))]
public class VoxelSystemSaveLoadInterface : MonoBehaviour
{

    public string objectName = "MyName";
    private string path = Application.dataPath + "/Data/SavedVoxelSystems/";

    private void Load()
    {
        VoxelSystemDataConverter VSD = Serializer.Load<VoxelSystemDataConverter>(path+objectName+".space");
        VoxelSystem voxelSystem = GetComponent<VoxelSystem>();
        VSD.FillVoxelSystem(ref voxelSystem);
    }
    private void Save()
    {
        VoxelSystemDataConverter VSD = new VoxelSystemDataConverter(GetComponent<VoxelSystem>());
        Serializer.Save<VoxelSystemDataConverter>(path+objectName+".space", VSD);
    }

    private void OnGUI()
    {        
        if (GUI.Button(new Rect(Screen.width/2, 25, 150, 50), "Save")) 
        {
            Save();  
        }else if (GUI.Button(new Rect(Screen.width/2+150, 25, 150, 50), "Load"))
        {
            Load();   
        }

        objectName = GUI.TextField(new Rect(Screen.width/2, 50+25, 300, 20), objectName);
    }
}
