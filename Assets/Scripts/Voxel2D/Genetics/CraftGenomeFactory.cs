//using UnityEngine;
//using System.Collections;
using System.Collections.Generic;

using System.Diagnostics;

using SharpNeat;
using SharpNeat.Core;
using SharpNeat.Utility;

public class CraftGenomeFactory : IGenomeFactory<CraftGenome>
{
	readonly UInt32IdGenerator _genomeIdGenerator;
	readonly UInt32IdGenerator _innovationIdGenerator;
	int _searchMode;

	protected readonly FastRandom _rng = new FastRandom();


	public UInt32IdGenerator GenomeIdGenerator
	{
		get { return _genomeIdGenerator; }
	}

	public int SearchMode 
	{ 
		get { return _searchMode; }
		set { _searchMode = value; }
	}

	public List<CraftGenome> CreateGenomeList(int length, uint birthGeneration)
	{   
		List<CraftGenome> genomeList = new List<CraftGenome>(length);
		for(int i=0; i<length; i++)
		{
			_innovationIdGenerator.Reset();
			genomeList.Add(CreateGenome(birthGeneration));
		}
		return genomeList;
	}

	public List<CraftGenome> CreateGenomeList(int length, uint birthGeneration, CraftGenome seedGenome)
	{   
		Debug.Assert(this == seedGenome.GenomeFactory, "seedGenome is from a different genome factory.");
		
		List<CraftGenome> genomeList = new List<CraftGenome>(length);
		
		// Add an exact copy of the seed to the list.
		CraftGenome newGenome = CreateGenomeCopy(seedGenome, _genomeIdGenerator.NextId, birthGeneration);
		genomeList.Add(newGenome);
		
		// For the remainder we create mutated offspring from the seed.
		for(int i=1; i<length; i++) {
			genomeList.Add(seedGenome.CreateOffspring(birthGeneration));
		}
		return genomeList;
	}

	public List<CraftGenome> CreateGenomeList(int length, uint birthGeneration, List<CraftGenome> seedGenomeList)
	{   
		if(seedGenomeList.Count == 0) {
			throw new SharpNeatException("CreateGenomeList() requires at least on seed genome in seedGenomeList.");
		}

		seedGenomeList = new List<CraftGenome>(seedGenomeList);
		Utilities.Shuffle(seedGenomeList, _rng);

		List<CraftGenome> genomeList = new List<CraftGenome>(length);
		int idx=0;
		int seedCount = seedGenomeList.Count;
		for(int seedIdx=0; idx<length && seedIdx<seedCount; idx++, seedIdx++)
		{
			CraftGenome newGenome = CreateGenomeCopy(seedGenomeList[seedIdx], _genomeIdGenerator.NextId, birthGeneration);
			genomeList.Add(newGenome);
		}

		for(; idx<length;) {
			for(int seedIdx=0; idx<length && seedIdx<seedCount; idx++, seedIdx++) {
				genomeList.Add(seedGenomeList[seedIdx].CreateOffspring(birthGeneration));
			}
		}
		return genomeList;
	}

	public CraftGenome CreateGenome(uint birthGeneration)
	{   		
		// Create and return the completed genome object.
		return CreateGenome(_genomeIdGenerator.NextId, birthGeneration);
	}

	public virtual bool CheckGenomeType(CraftGenome genome)
	{
		return genome.GetType() == typeof(CraftGenome);
	}

	public virtual CraftGenome CreateGenome(uint id, 
	                                       uint birthGeneration)
	{
		return new CraftGenome(this, id, birthGeneration);
	}

	public virtual CraftGenome CreateGenomeCopy(CraftGenome copyFrom, uint id, uint birthGeneration)
	{
		return new CraftGenome(copyFrom, id, birthGeneration);
	}

}
