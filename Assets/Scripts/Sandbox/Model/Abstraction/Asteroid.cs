using UnityEngine;
using System.Collections.Generic;

namespace SpaceSandbox
{
	public class Asteroid : Container 
	{
		public Resource Containment { get; set; }

		private float fidelity = 30f;
		private List<Vector2> vertices = new List<Vector2>();

		private PolygonCollider2D collider;
	
		public override void InitializeView()
		{
			GameObject newContainer = new GameObject( "Asteroid" );
			MeshFilter filter = newContainer.AddComponent<MeshFilter>();
			MeshRenderer renderer = newContainer.AddComponent<MeshRenderer>();
			collider = newContainer.AddComponent<PolygonCollider2D>();
			Rigidbody2D rigid = newContainer.AddComponent<Rigidbody2D>();
			ContainerView view = newContainer.AddComponent<ContainerView>();
			
			view.m_contain = this;
			this.View = view;

			



			float size = Random.Range(0.3f, 2f);
			AsteroidGenerator.GenerateCircular(size, 20f, 60f, out vertices);




			int[] indices;
			Mesh mesh = new Mesh();
			mesh.MarkDynamic();

			Triangulator tr = new Triangulator(vertices.ToArray());
			indices = tr.Triangulate();
			
			Vector3[] vertices3d = new Vector3[vertices.Count];
			for( int j = 0; j < vertices3d.Length; j++ )
			{
				vertices3d[j].x = vertices[j].x;
				vertices3d[j].y = vertices[j].y;
			}
			
			mesh.vertices = vertices3d;
			mesh.triangles = indices;
			mesh.RecalculateBounds();
			
			filter.sharedMesh = mesh;
			
			
			
			collider.points = vertices.ToArray();
			
			rigid.mass = size * 100f;
			
			renderer.sharedMaterial = new UnityEngine.Material(Shader.Find("Diffuse"));
		}

		public override void UpdateView()
		{
			Triangulator tr = new Triangulator(vertices.ToArray());
			int[] indices = tr.Triangulate();
			
			Vector3[] vertices3d = new Vector3[vertices.Count];
			for( int j = 0; j < vertices3d.Length; j++ )
			{
				vertices3d[j].x = vertices[j].x;
				vertices3d[j].y = vertices[j].y;
			}

			MeshFilter filter = View.GetComponent<MeshFilter>();

			Mesh mesh = filter.sharedMesh;
			mesh.Clear();

		
			mesh.vertices = vertices3d;
			mesh.triangles = indices;


			mesh.RecalculateBounds();
			
			filter.sharedMesh = mesh;







			collider.points = vertices.ToArray();
		}

		public override void Initialize() 
		{

		}
	
		public override void Update()
		{

		}
	
		public override void TakeDamage( float damage, float radius, UnityEngine.Vector2 center ) 
		{
			// Generate smaller asteroids here 

			SubstructVertices( radius, center );
			UpdateView();
		}
	
		public override void Destroy() 
		{
	
		}

		public override void OnDrawGizmos()
		{
			for( int i = 0; i < vertices.Count; i++ )
			{
				float indexValue = i / (float) vertices.Count;
					
				Gizmos.color = Color.Lerp( Color.red, Color.green, indexValue );

				Gizmos.DrawSphere( View.transform.TransformPoint( vertices[i] ), 0.05f );
			}
		}

		private void SubstructVertices(float radius, UnityEngine.Vector2 center)
		{
			List<Vector2> newVerts = new List<Vector2>();
			int injectionIdx = -1;
			float minDist = float.MaxValue;
			
			float section = 2f * Mathf.PI / (fidelity * radius );
			for( float arg = 0f; arg > -2f * Mathf.PI; arg -= section )
			{
				Vector2 radiusPoint;
				radiusPoint.x = Mathf.Cos(arg) * radius;
				radiusPoint.y = Mathf.Sin(arg) * radius;
				
				if( collider.OverlapPoint( center + radiusPoint ) )
					newVerts.Add( View.transform.InverseTransformPoint( center + radiusPoint ));
			}
			
			for( int i = 0; i < vertices.Count; i++ )
			{
				Vector2 worldPos = Vector2.zero;
				worldPos.x = View.transform.position.x + vertices[i].x;
				worldPos.y = View.transform.position.y + vertices[i].y;
				
				float distance = (worldPos - center).magnitude;
				if( distance < minDist )
				{
					minDist = distance;
					injectionIdx = i;
				}
			}
			
			
			
			if( injectionIdx == -1 )
			{
				Debug.LogError("injectionIdx is -1");
				return;
			}
			
			vertices.InsertRange( injectionIdx, newVerts );
			
			for( int i = 0; i < vertices.Count; i++ )
			{
				Vector2 worldPos = Vector2.zero;
				worldPos.x = View.transform.position.x + vertices[i].x;
				worldPos.y = View.transform.position.y + vertices[i].y;
				
				float distance = (worldPos - center).magnitude;
				
				if( distance  < radius )
				{
					vertices.RemoveAt(i);
					i--;
				}
			}
		}
	}
}
