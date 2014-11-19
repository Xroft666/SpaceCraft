using System;

namespace Voxel2D
{
    [Serializable]
    public class VoxelSystemDataConverter
    {
        private int[,] ElementIDs;
        private int[,] EntityIDs;
        private int[,] Rotations;

        public VoxelSystemDataConverter(VoxelSystem vox)
        {
            VoxelData[,] voxData = vox.GetVoxelData();
            ElementIDs = new int[voxData.GetLength(0), voxData.GetLength(1)];
            EntityIDs = new int[voxData.GetLength(0), voxData.GetLength(1)];
            Rotations = new int[voxData.GetLength(0), voxData.GetLength(1)];

            for (int x = 0; x < voxData.GetLength(0); x++)
            {
                for (int y = 0; y < voxData.GetLength(1); y++)
                {
                    if (voxData[x, y] != null)
                    {
                        ElementIDs[x, y] = voxData[x, y].GetElementID();
                        EntityIDs[x, y] = TextureHolder.Instance.GetDeviceIndex(voxData[x, y].deviceName);
                        Rotations[x, y] = voxData[x, y].rotation;
                    }
                    else
                    {
                        ElementIDs[x, y] = -1;
                        EntityIDs[x, y] = -1;
                        Rotations[x, y] = -1;
                    }
                }
            }
        }

        public void FillVoxelSystem(ref VoxelSystem voxelSys)
        {
            voxelSys.SetEmpty();

            VoxelData[,] voxData = new VoxelData[ElementIDs.GetLength(0), ElementIDs.GetLength(1)];

            for (int x = 0; x < ElementIDs.GetLength(0); x++)
            {
                for (int y = 0; y < ElementIDs.GetLength(1); y++)
                {
                    if (ElementIDs[x, y] != -1)
                    {
                        VoxelData VD = null;

                        switch (TextureHolder.Instance.GetDeviceName(EntityIDs[x, y]))
                        {
                            case "Engine":
                                VD = new Engine(ElementIDs[x, y], new IntVector2(x, y), Rotations[x, y], voxelSys, 1000);
                                break;
                            case "Floor":
                                VD = new Floor(ElementIDs[x, y], new IntVector2(x, y), Rotations[x, y], voxelSys);
                                break;
                            case "Ore":
                                VD = new Ore(ElementIDs[x, y], new IntVector2(x, y), Rotations[x, y], voxelSys);
                                break;
                            case "ShipController":
                                VD = new ShipController(ElementIDs[x, y], new IntVector2(x, y), Rotations[x, y],
                                    voxelSys);
                                break;
                            case "Wall":
                                VD = new Wall(ElementIDs[x, y], new IntVector2(x, y), Rotations[x, y], voxelSys);
                                break;
                            case "Laser":
                                VD = new Laser(ElementIDs[x, y], new IntVector2(x, y), Rotations[x, y], voxelSys, 100);
                                break;
                            case "Cannon":
                                VD = new Cannon(ElementIDs[x, y], new IntVector2(x, y), Rotations[x, y], voxelSys,
                                    1000000, 200);
                                break;

                        }
                        voxData[x, y] = VD;
                    }
                    
                }
            }
            voxelSys.SetVoxelGrid(voxData);
        }
    }
}
