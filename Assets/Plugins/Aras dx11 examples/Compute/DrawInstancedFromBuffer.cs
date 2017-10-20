using UnityEngine;

public class DrawInstancedFromBuffer : MonoBehaviour {
	
	public Material material;
	public ComputeShader updateCS;

	public int vertexCount = 30;
	public int instanceCount = 100;
	
	private ComputeBuffer bufferPoints;
	private ComputeBuffer bufferPos;
	private ComputeBuffer outBufferPos;
	private ComputeBuffer cbDrawArgs;

	private Vector3[] origPos;
	private Vector3[] pos;
	
	void Start () {

		cbDrawArgs = new ComputeBuffer (1, 16, ComputeBufferType.IndirectArguments);
		var args = new int[4];
		args[0] = vertexCount;
		args[1] = instanceCount;
		args[2] = 0;
		args[3] = 0;
		cbDrawArgs.SetData (args);

		// making a circle figure
		var verts = new Vector3[vertexCount];
		for (var i = 0; i < vertexCount; ++i)
		{
			float phi = i * Mathf.PI * 2.0f / (vertexCount-1);
			verts[i] = new Vector3(Mathf.Cos(phi), Mathf.Sin(phi), 0.0f);
		}

		// setting one circle figure to the material
		bufferPoints = new ComputeBuffer (vertexCount, 12);
		bufferPoints.SetData (verts);
		material.SetBuffer ("buf_Points", bufferPoints);

		origPos = new Vector3[instanceCount];
		for (var i = 0; i < instanceCount; ++i)
			origPos[i] = Random.insideUnitSphere * 5.0f;
		pos = new Vector3[instanceCount];

		// original set of points
		bufferPos = new ComputeBuffer (instanceCount, 12);
		// dynamuc set of points
		outBufferPos = new ComputeBuffer(instanceCount, 12);

		// binding positions buffer to the material
		material.SetBuffer ("buf_Positions", outBufferPos);

		updateCS.SetBuffer (0, "_origPos", bufferPos);
		updateCS.SetBuffer (0, "_pos", outBufferPos);
	}
	
	private void ReleaseBuffers() 
	{
		if (cbDrawArgs != null)
			cbDrawArgs.Release ();
		
		if (bufferPoints != null) 
			bufferPoints.Release();

		if (bufferPos != null) 
			bufferPos.Release();

		if (outBufferPos != null)
			outBufferPos.Release ();
	
		bufferPoints = null;
		bufferPos = null;
		outBufferPos = null;
	}
	
	void OnDisable() {
		ReleaseBuffers ();
	}

	
	// called if script attached to the camera, after all regular rendering is done
	void OnPostRender () {

		// updating current positions buffer
		updateCS.SetFloat ("_Time", Time.timeSinceLevelLoad);
		updateCS.Dispatch(0, 100, 100, 100);

		material.SetPass (0);

		Graphics.DrawProcedural (MeshTopology.LineStrip, vertexCount, instanceCount);
		//Graphics.DrawProceduralIndirect (MeshTopology.LineStrip, cbDrawArgs);
	}
}
