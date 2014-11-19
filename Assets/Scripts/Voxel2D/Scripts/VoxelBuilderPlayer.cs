using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpaceSandbox;


namespace Voxel2D{
    //[RequireComponent(typeof(VoxelSystemSaveLoadInterface))]
	public class VoxelBuilderPlayer : MonoBehaviour {
		
		VoxelSystem _voxel;

		int _selectedElementId;
		int _selectedRotation;
		DeviceData.DeviceType _selectedDevice = DeviceData.DeviceType.Wall;
		
		// Use this for initialization
		void Start () {
			
			_voxel = gameObject.AddComponent<VoxelSystem>();
		    gameObject.AddComponent<VoxelSystemSaveLoadInterface>();

			Camera.main.transform.parent = transform;
			Camera.main.orthographicSize = 15;
			
			InitShip();
		}
		
		void InitShip(){
			VoxelData[,] startShip = new VoxelData[20,20];
			
			for (int x = 9; x < 14; x++) {
				for (int y = 8; y < 16; y++) {
					startShip[x,y] = new Wall(2, new IntVector2(x,y),0,_voxel);
					//voxel.AddVoxel(x+5,y+5,2);
				}
			}
			_voxel.SetVoxelGrid(startShip);
			
			_voxel.AddVoxel(new ShipController(2, new IntVector2(11,7),0,_voxel));
			_voxel.AddVoxel(new Engine(2, new IntVector2(9,7),0,_voxel,1000));
			_voxel.AddVoxel( new Engine(2, new IntVector2(13,7),0,_voxel,1000));
			_voxel.AddVoxel(new Engine(2, new IntVector2(12,7),0,_voxel,1000));
			_voxel.AddVoxel( new Engine(2, new IntVector2(10,7),0,_voxel,1000));
			
			_voxel.AddVoxel(new Cannon(2,new IntVector2(12,16),_selectedRotation,_voxel,1000000,200));
			_voxel.AddVoxel(new Cannon(2,new IntVector2(10,16),_selectedRotation,_voxel,1000000,200));

			_voxel.AddVoxel(new Laser(2,new IntVector2(11,16),_selectedRotation,_voxel,100f));

			Vector2 center = _voxel.GetCenter();
			Camera.main.transform.position = transform.TransformPoint(new Vector3(center.x,center.y,-10));
		}
		
		
		// Update is called once per frame
		void Update () {
			CheckInput();
			_selectedElementId = Mathf.Clamp(_selectedElementId,1,MaterialSystem.ElementList.Instance.elements.Count-1);
			//Mathf.Clamp(selectedDevice,0,System.Enum.GetValues(typeof(device)).Length);
			
            if(_selectedDevice.GetHashCode() <0){
				_selectedDevice++;
			}else if(_selectedDevice.GetHashCode() >System.Enum.GetValues(typeof(DeviceData.DeviceType)).Length-1){
				_selectedDevice--;
			}
			_selectedRotation = Mathf.Clamp(_selectedRotation,0,270);
			
		}
		
		void CheckInput(){
			Vector2 point = Input.mousePosition;
			Vector3 world = Camera.main.ScreenToWorldPoint(point);
			Vector3 localPos = transform.InverseTransformPoint(world);
			IntVector2 RL = new IntVector2(Mathf.RoundToInt(localPos.x),Mathf.RoundToInt(localPos.y));
			
			if(Input.GetMouseButtonDown(0)){
				if(VoxelUtility.IsPointInBounds(_voxel.GetGridSize(),localPos)){
					if(!_voxel.IsVoxelEmpty(RL.x,RL.y)){
						_voxel.RemoveVoxel(RL.x,RL.y);
						List<bool[,]> islands = VoxelIslandDetector.findIslands(VoxelUtility.VoxelDataToBool(_voxel.GetVoxelData()));
						if(islands.Count == 1){
							
						}else{
							VoxelData[,] vox = VoxelIslandDetector.SplitAndReturnFirstIslands(_voxel.GetVoxelData(),_voxel);
							_voxel.SetVoxelGrid(vox);
						}
					}
				}
			}else if(Input.GetMouseButtonDown(1)){
				if(VoxelUtility.IsPointInBounds(_voxel.GetGridSize(),localPos)){
					if(VoxelUtility.IsPosNextToVoxel(_voxel.GetVoxelData(),RL)){
						if(_voxel.IsVoxelEmpty(RL.x,RL.y)){
							AddSelectedVoxelType(RL.x,RL.y);
						}
					}
				}
			}
			
			if(Input.GetKeyDown(KeyCode.U)){
				_selectedElementId++;
			}else if(Input.GetKeyDown(KeyCode.J)){
				_selectedElementId--;
			}
			
			if(Input.GetKeyDown(KeyCode.O)){
				_selectedRotation += 90;
			}else if(Input.GetKeyDown(KeyCode.L)){
				_selectedRotation -= 90;
			}
			
			if(Input.GetKeyDown(KeyCode.I)){
				_selectedDevice++;
			}else if(Input.GetKeyDown(KeyCode.K)){
				_selectedDevice--;
			}
			
			
			
		}
		
		void AddSelectedVoxelType(int x,int y){
			VoxelData vd = null;
			
			switch(_selectedDevice){
			case DeviceData.DeviceType.Engine:
				vd = new Engine(_selectedElementId,new IntVector2(x,y),_selectedRotation,_voxel,1000);
				break;
            case DeviceData.DeviceType.Floor:
				vd = new Floor(_selectedElementId,new IntVector2(x,y),_selectedRotation,_voxel);
				break;
            case DeviceData.DeviceType.Ore:
				vd = new Ore(_selectedElementId,new IntVector2(x,y),_selectedRotation,_voxel);
				break;
            case DeviceData.DeviceType.ShipController:
				vd = new ShipController(_selectedElementId,new IntVector2(x,y),_selectedRotation,_voxel);
				break;
            case DeviceData.DeviceType.Wall:
				vd = new Wall(_selectedElementId,new IntVector2(x,y),_selectedRotation,_voxel);
				break;
            case DeviceData.DeviceType.Laser:
				vd = new Laser(_selectedElementId,new IntVector2(x,y),_selectedRotation,_voxel,100);
				break;
            case DeviceData.DeviceType.Cannon:
				vd = new Cannon(_selectedElementId,new IntVector2(x,y),_selectedRotation,_voxel,1000000,200);
				break;
				
			}
			_voxel.AddVoxel(vd);
			
		}
		
		void OnGUI(){
			GUI.Label(new Rect(Screen.width/2-150,0,300,20),"element: "+_selectedElementId+" device: "+_selectedDevice.ToString()+" rotation: "+_selectedRotation);
		}
		
	}
	
}