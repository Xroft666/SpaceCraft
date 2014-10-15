using UnityEngine;
using System.Collections;


namespace MaterialSystem{
	public class ElementStats{
		public ElementStats(int ID){
			this.ID = ID;
		}

		public int ID;

		public float temperature;	//temperature
		public float fragmentionRate;	//increases each impact, overall strength is (hardness*flexibility)*(1-fragmentationRate)
	
		public float destructionEnergy {
			get{
				ElementSpecs e = ElementList.Instance.elements[ID];
				return e.flexibility*e.hardness*(1-fragmentionRate);
			}
		}
	
	
	
	
	
	
	
	}
}
