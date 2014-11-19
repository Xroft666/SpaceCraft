using System;
using UnityEngine;
using System.Collections;

namespace SpaceSandbox
{
    public static class DeviceData
    {
       public enum DeviceType
        {
            Engine,Floor,Ore,ShipController,Wall,Laser,Cannon
        }

        public static Type Device;

    }
}
