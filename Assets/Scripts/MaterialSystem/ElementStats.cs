using UnityEngine;
using System.Collections;


namespace MaterialSystem{
	public class ElementStats{
		public ElementStats(int ID){
			this.ID = ID;
			e = ElementList.Instance.elements[ID];
		}
		public ElementSpecs e{get;private set;}

		public int ID;

		public float sizeModifier = 0; 	//how much of the full size the object is

		public float temperature{get; set;}		//temperature	
		public float fragmention{get; private set;}		//increases each impact, overall strength is (hardness*flexibility)*(1-fragmentationRate)


		public void addFragmentation(float impactForce){
			fragmention += impactForce/destructionEnergy;
			fragmention = Mathf.Clamp(fragmention,0,1);
		}

		/// <summary>
		/// Gets the energy required to break this voxel
		/// </summary>
		/// <value>The destruction energy.</value>
		public float destructionEnergy {
			get{
				return e.flexibility*e.hardness*(1-fragmention)*(1-(temperature/e.melting))*sizeModifier;
			}
		}

		/// <summary>
		/// Gets the mass according to size modifier.
		/// </summary>
		/// <value>The mass.</value>
		public float mass{
			get{
				return e.mass*sizeModifier;
			}
		}

		/// <summary>
		/// Gets the thermal radiation rate per tick in joules 
		/// </summary>
		/// <value>The thermal radiation.</value>
		public float thermalRadiation{
			get{
				return temperature*e.radiationRate;
			}
		}

		/// <summary>
		/// Gets the total heat capacity of this block considering its mass.
		/// </summary>
		/// <value>The total heat capacity.</value>
		public float totalHeatCapacity{
			get{
				return e.heatCapacity*mass;
			}
		}

		/// <summary>
		/// Gets the thermal transfer rate per tick in joules
		/// </summary>
		/// <value>The thermal transfer.</value>
		public float thermalTransfer{
			get{
				return temperature*e.thermalConductivity;
			}
		}

		/// <summary>
		/// Adds thermal energy in form of joules.
		/// </summary>
		/// <param name="energy">Energy.</param>
		public void addThermalEnergy(float energy){
			temperature+= energy/(totalHeatCapacity);
		}

		/// <summary>
		/// Removes thermal energy in form of joules.
		/// </summary>
		/// <param name="energy">Energy.</param>
		public void removeThermalEnergy(float energy){
			temperature-= energy/(totalHeatCapacity);
		}
	
	}
}
