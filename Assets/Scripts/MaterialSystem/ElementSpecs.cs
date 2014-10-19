using UnityEngine;
using System.Collections;

namespace MaterialSystem{
	[System.Serializable]
	public class ElementSpecs{
		//Utility
		public string name;
		public int ID;
		public Texture2D texture;
		public Color color;


		public float mass;

		//strength			strength = hardness*flexibility
		public float hardness;		//
		public float flexibility;	//absorb impact to heat?	lower mean higher fragmentation rate(translates impact energy to neighbours)	


		public float thermalConductivity;	
		public float electricaConductivity;	//lower increases temperature raise and waste of electrical power

		//heat
		public float radiationRate;	//percentage of temperature
		public float heatCapacity;	// 1 degree increase = mass*heatCapacity
		public float melting;	//temperature to melt
		public float boiling; 	//temperature to boil

		public bool magnetic;	//effected by magnetic fields?
	}
}