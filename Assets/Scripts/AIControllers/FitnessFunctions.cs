using System;
using UnityEngine;
using System.Collections;
using Voxel2D;

public static class FitnessFunctions
{

 
    /// <summary>
    /// Stay at point
    /// </summary>
    /// <param name="S"></param>
    /// <returns></returns>
    public static float GetFitnessStayOnTarget(ShipBuilderBrain S)
    {
        VoxelSystem VS = S.voxelSystem;
        Vector3 shipPos = VS.transform.TransformPoint(VS.GetCenter());

        float fitness = 100;
        fitness -= (shipPos - GotoTarget.Position).magnitude;
        fitness += S._targetScore;

        /*
        fitness -= S._wallHits * 0.01f;
        fitness -= Mathf.Clamp(VS.rigidbody2D.velocity.magnitude / 100, 0, 10);
        fitness -= Mathf.Clamp(VS.rigidbody2D.angularVelocity / 10, 0, 10);
        fitness -= Mathf.Clamp(S._totalRotation / 100, 0, 25);
        */
         
        fitness = Mathf.Clamp(fitness, 0, 999999);

        return fitness;
    }
}
