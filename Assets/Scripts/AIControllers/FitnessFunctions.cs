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
    public static float GetFitnessStayOnTarget(ShipBuilderBrain S, float currentGoal)
    {
        VoxelSystem VS = S.voxelSystem;
        Vector3 shipPos = VS.transform.TransformPoint(VS.GetCenter());

        float fitness = 100;
        fitness -= (shipPos - GotoTarget.Position).magnitude;
        fitness += S.StayOnTargetScore;
        fitness -= S.Stats.ObsticleHits*10;
        //fitness -= S.Stats.usedFuel/25;
         
        fitness = Mathf.Clamp(fitness, 0, 999999);

        return fitness / currentGoal;
    }

	public static float GetFitnessHitTarget(ShipBuilderBrain S, float currentGoal)
	{
	    float fitness = S.Stats.EnemyDamage.Hits;
	    fitness -= S.Stats.ObsticleHits/4;
        fitness = Mathf.Clamp(fitness, 0, 999999);
        return fitness / currentGoal;
	}
}
