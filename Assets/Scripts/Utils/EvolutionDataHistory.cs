using System.Collections.Generic;
using System.IO;
using SharpNeat.Core;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using UnityEngine;
using System.Collections;
using System.Linq;

public class EvolutionDataHistory
{
    private readonly NeatEvolutionAlgorithm<NeatGenome> _ea;
    private readonly string _filename;
    private readonly Optimizer _optimizer;
    private string path;

    public EvolutionDataHistory(NeatEvolutionAlgorithm<NeatGenome> ea, string filename, Optimizer optimizer)
    {
        _ea = ea;
        _filename = filename;
        _optimizer = optimizer;

        path = Application.persistentDataPath + "/EvolutionData/";
        Debug.Log(path);


        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        if (!File.Exists(path + _filename + ".cvs"))
        {

            CreateTitleLine();
        }
    }

    void CreateTitleLine()
    {
        WriteData("Objective Number");
        WriteData(",");
        WriteData("Fitness");
        WriteData(",");
        WriteData("Fuel usage");
        WriteData(",");
        WriteData("Wall Hit");
        WriteData(",");
        WriteData("Astroid Damage");
        WriteData(",");
        WriteData("Astroid Hits");
        WriteData(",");
        WriteData("Enemy Damage");
        WriteData(",");
        WriteData("Enemy Hits");
        WriteData(",");
        WriteData("Required Fitness");

        WriteData("\n");
    }

    private void WriteData(string Data)
    {
        File.AppendAllText(path + _filename + ".csv", Data);
    }

    public void NextGeneration(float meanFit, float Reqfit)
    {
        

        //List<float> meanFit = new List<float>();
        List<float> fuel = new List<float>();
        List<float> wallHit = new List<float>();
        List<float> aD = new List<float>();
        List<float> aH = new List<float>();
        List<float> eD = new List<float>();
        List<float> eH = new List<float>();

        List<ShipBuilderBrain.SimulationStats> statList = Optimizer.Units.Select(controller => (controller as ShipBuilderBrain).Stats).ToList();
        //Debug.Log(test.Count);
        statList.Sort((stats, simulationStats) => stats.Fitness < simulationStats.Fitness ? 1 : -1);
        statList.RemoveRange(statList.Count / 2, statList.Count - statList.Count / 2);

 //      int l = statList.Count;
 //      for (int i = l / 2 - 1; i >= 0; i--)
 //      {
 //          statList.RemoveAt(i);
 //      }

        foreach (ShipBuilderBrain.SimulationStats s in statList)
        {
           
            fuel.Add(s.UsedFuel);
            wallHit.Add(s.ObsticleHits);
            aD.Add(s.AstroidDamage.Damage);
            aH.Add(s.AstroidDamage.Hits);
            eD.Add(s.EnemyDamage.Damage);
            eH.Add(s.EnemyDamage.Hits);
        }
        
        WriteData(_optimizer.objective.ObjectiveNum.ToString());
        WriteData(",");
        WriteData(meanFit.ToString());
        WriteData(",");
        WriteData(fuel.Average().ToString());
        WriteData(",");
        WriteData(wallHit.Average().ToString());
        WriteData(",");
        WriteData(aD.Average().ToString());
        WriteData(",");
        WriteData(aH.Average().ToString());
        WriteData(",");
        WriteData(eD.Average().ToString());
        WriteData(",");
        WriteData(eH.Average().ToString());
        WriteData(",");
        WriteData(Reqfit.ToString());
        WriteData("\n");


    }


    
}
