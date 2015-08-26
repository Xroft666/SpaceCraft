
using System.Collections.Generic;

namespace SpaceSandbox
{
	public class Container : Entity 
	{
		public ContainerView View { get; set; }

		public virtual void InitializeView() {}
		public virtual void UpdateView() {}

		public virtual void OnDrawGizmos(){}
	}
}