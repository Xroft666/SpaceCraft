//using UnityEngine;
//using System.Collections;

using SharpNeat;
using SharpNeat.Core;

using Voxel2D;

public class CraftGenome : IGenome<CraftGenome>
{
	CraftGenomeFactory _genomeFactory;
	readonly uint _id;
	int _specieIdx;
	readonly uint _birthGeneration;
	EvaluationInfo _evalInfo;
	CoordinateVector _position;
	object _cachedPhenome;

	int voxelsCount;
//	VoxelData

	public CraftGenome () {}
	public CraftGenome( CraftGenome copyFrom, uint id, uint birthGeneration )
	{
		_id = id;
		_birthGeneration = birthGeneration;
	}

	public CraftGenome( CraftGenomeFactory genomeFactory, uint id, uint birthGeneration )
	{
		_id = id;
		_birthGeneration = birthGeneration;
		_genomeFactory = genomeFactory;
	}

	public uint Id
	{
		get { return _id; }
	}

	public int SpecieIdx 
	{ 
		get { return _specieIdx; }
		set { _specieIdx = value; }
	}

	public uint BirthGeneration
	{
		get { return _birthGeneration; }
	}

	public EvaluationInfo EvaluationInfo
	{
		get { return _evalInfo; }
	}

	public double Complexity 
	{ 
		get { return voxelsCount; }
	}

	public CoordinateVector Position 
	{ 
		get { return new CoordinateVector(null); }
	}

	public object CachedPhenome 
	{ 
		get { return _cachedPhenome; }
		set { _cachedPhenome = value; }
	}
	
	public CraftGenome CreateOffspring(uint birthGeneration)
	{
//		// Make a new genome that is a copy of this one but with a new genome ID.
//		NeatGenome offspring = _genomeFactory.CreateGenomeCopy(this, _genomeFactory.NextGenomeId(), birthGeneration);
//		
//		// Mutate the new genome.
//		offspring.Mutate();
//		return offspring;

		return new CraftGenome();
	}

	public CraftGenome CreateOffspring(CraftGenome parent, uint birthGeneration)
	{
		return new CraftGenome();
	}

	public CraftGenomeFactory GenomeFactory
	{
		get { return _genomeFactory; }
		set 
		{
			if(null != _genomeFactory) {
				throw new SharpNeatException("NeatGenome already has an assigned GenomeFactory.");
			}
			_genomeFactory = value;
//			_evalInfo = new EvaluationInfo(_genomeFactory.NeatGenomeParameters.FitnessHistoryLength);
//			_auxStateNeuronCount = CountAuxStateNodes();
		}
	}
}
