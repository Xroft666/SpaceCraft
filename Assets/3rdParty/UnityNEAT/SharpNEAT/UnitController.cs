using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;

public abstract class UnitController : MonoBehaviour
{

	public abstract void Activate(IBlackBox box, params object[] blackBoxExtraData);

    public abstract void Stop();

    public abstract float GetFitness();
    public abstract void SetOptimizer(Optimizer o);
}
