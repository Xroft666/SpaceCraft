using SharpNeat.Domains;
using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using System.Collections.Generic;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using System;
using System.Xml;
using System.IO;
using Voxel2D;

public class Optimizer : MonoBehaviour {

    const int NUM_INPUTS = 20;
    const int NUM_OUTPUTS = 7;

    public static int VoxelSize;

    public int Trials;
    public float TrialDuration;
    public float StoppingFitness;
    bool EARunning;
    string popFileSavePath, champFileSavePath;

    SimpleExperiment experiment;
    public static NeatEvolutionAlgorithm<NeatGenome> _ea { get; private set; }

    public GameObject Unit;

    static Dictionary<IBlackBox, UnitController> ControllerMap = new Dictionary<IBlackBox, UnitController>();
	static public List<UnitController> Units
	{
		get
		{
			return new List<UnitController>(ControllerMap.Values);
		}
	}

    private DateTime startTime;
    private float timeLeft;
    private float accum;
    private int frames;
    private float updateInterval = 12;

    private uint Generation;
    private double Fitness;

    public ObjectiveHandler objective;
	
	static public string fileName = "";

	// Use this for initialization
	void Start ()
	{

        
	    VoxelImpacter.GenerateFragments = false;

        Utility.DebugLog = true;
        experiment = new SimpleExperiment();
        XmlDocument xmlConfig = new XmlDocument();
        TextAsset textAsset = (TextAsset)Resources.Load("experiment.config");
        xmlConfig.LoadXml(textAsset.text);
        experiment.SetOptimizer(this);
        VoxelSize = XmlUtils.GetValueAsInt(xmlConfig.DocumentElement, "VoxelSize");
        experiment.Initialize("Car Experiment", xmlConfig.DocumentElement, NUM_INPUTS, NUM_OUTPUTS);

        champFileSavePath = Application.persistentDataPath + string.Format("/{0}.champ.xml", fileName);
        popFileSavePath = Application.persistentDataPath + string.Format("/{0}.pop.xml", fileName);
	}

    // Update is called once per frame
    void Update()
    {
      //  evaluationStartTime += Time.deltaTime;

        timeLeft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if (timeLeft <= 0.0)
        {
            var fps = accum / frames;
            timeLeft = updateInterval;
            accum = 0.0f;
            frames = 0;
            //   print("FPS: " + fps);
            if (fps < 10)
            {
               // Time.timeScale = Time.timeScale - 1;
                print("Lowering time scale to " + Time.timeScale);
            }
        }
    }

    public void StartEA()
    {        
        Utility.DebugLog = true;
        Utility.Log("Starting PhotoTaxis experiment");
        // print("Loading: " + popFileLoadPath);
        
        startTime = DateTime.Now;

        _ea.UpdateEvent += new EventHandler(ea_UpdateEvent);
        _ea.PausedEvent += new EventHandler(ea_PauseEvent);

        var evoSpeed = 1;

     //   Time.fixedDeltaTime = 0.045f;
        Time.timeScale = evoSpeed;       
        _ea.StartContinue();
        EARunning = true;
    }



    void ea_UpdateEvent(object sender, EventArgs e)
    {
        Utility.Log(string.Format("gen={0:N0} bestFitness={1:N6}",
            _ea.CurrentGeneration, _ea.Statistics._maxFitness));

        Fitness = _ea.Statistics._meanFitness;
        Generation = _ea.CurrentGeneration;
      
        objective.NextGen();
    //    Utility.Log(string.Format("Moving average: {0}, N: {1}", _ea.Statistics._bestFitnessMA.Mean, _ea.Statistics._bestFitnessMA.Length));

		XmlWriterSettings _xwSettings = new XmlWriterSettings();
		_xwSettings.Indent = true;

		// Autosave every 50 generation 
    	if( Generation % 50 == 0 )
		{
			using (XmlWriter xw = XmlWriter.Create(popFileSavePath, _xwSettings))
			{
				experiment.SavePopulation(xw, _ea.GenomeList);
			}
			// Also save the best genome
			
			using (XmlWriter xw = XmlWriter.Create(champFileSavePath, _xwSettings))
			{
				experiment.SavePopulation(xw, new NeatGenome[] { _ea.CurrentChampGenome });
			}
		}


		// destroyong all the bullets prefabs
		BulletController[] bullets = FindObjectsOfType(typeof(BulletController)) as BulletController[];
		foreach( BulletController bullet in bullets )
			Destroy(bullet.gameObject);
    }

    void ea_PauseEvent(object sender, EventArgs e)
    {
        Time.timeScale = 1;
        Utility.Log("Done ea'ing (and neat'ing)");

        XmlWriterSettings _xwSettings = new XmlWriterSettings();
        _xwSettings.Indent = true;
        // Save genomes to xml file.        
        DirectoryInfo dirInf = new DirectoryInfo(Application.persistentDataPath);
        if (!dirInf.Exists)
        {
            Debug.Log("Creating subdirectory");
            dirInf.Create();
        }
        using (XmlWriter xw = XmlWriter.Create(popFileSavePath, _xwSettings))
        {
            experiment.SavePopulation(xw, _ea.GenomeList);
        }
        // Also save the best genome

        using (XmlWriter xw = XmlWriter.Create(champFileSavePath, _xwSettings))
        {
            experiment.SavePopulation(xw, new NeatGenome[] { _ea.CurrentChampGenome });
        }
        DateTime endTime = DateTime.Now;
        Utility.Log("Total time elapsed: " + (endTime - startTime));

        System.IO.StreamReader stream = new System.IO.StreamReader(popFileSavePath);
       

      
        EARunning = false;        
        
    }

    public void StopEA()
    {

        if (_ea != null && _ea.RunState == SharpNeat.Core.RunState.Running)
        {
            _ea.Stop();
        }
    }

    public void Evaluate(IBlackBox box, params object[] blackBoxExtraData)
    {
        GameObject obj = Instantiate(Unit, Unit.transform.position, Unit.transform.rotation) as GameObject;
        UnitController controller = obj.GetComponent<UnitController>();

        ControllerMap.Add(box, controller);


		// some how here, i need to get the original genome which contains all the data on voxels
        controller.SetOptimizer(this);
		controller.Activate(box, blackBoxExtraData);
    }

    public void StopEvaluation(IBlackBox box)
    {
        UnitController ct = ControllerMap[box];

		ct.Stop();
        Destroy(ct.gameObject);
    }

    public void RunBest()
    {
        Time.timeScale = 1;

        NeatGenome genome = null;


        // Try to load the genome from the XML document.
        try
        {
            using (XmlReader xr = XmlReader.Create(champFileSavePath))
                genome = NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, (NeatGenomeFactory)experiment.CreateGenomeFactory())[0];


        }
        catch (Exception e1)
        {
			print(champFileSavePath + " Error loading genome from file!\nLoading aborted.\n"
			      + e1.Message + "\nJoe: " + champFileSavePath);
            return;
        }

		// Here we can read the genome's info on the ships construction
		// as long it is directly represented everything can just be converted into voxels


        // Get a genome decoder that can convert genomes to phenomes.
        var genomeDecoder = experiment.CreateGenomeDecoder();

        // Decode the genome into a phenome (neural network).
        var phenome = genomeDecoder.Decode(genome);

        GameObject obj = Instantiate(Unit, Unit.transform.position, Unit.transform.rotation) as GameObject;
        UnitController controller = obj.GetComponent<UnitController>();

        ControllerMap.Add(phenome, controller);

        controller.Activate(phenome, genome.VoxelData);
    }

    public float GetFitness(IBlackBox box)
    {
        if (ControllerMap.ContainsKey(box))
        {
            return ControllerMap[box].GetFitness();
        }
        return 0;
    }
	
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 40), "Start EA"))
        {
			RefreshFileName();

			_ea = experiment.CreateEvolutionAlgorithm();
            StartEA();
        }
        if (GUI.Button(new Rect(10, 60, 100, 40), "Stop EA"))
        {
			RefreshFileName();

            StopEA();
        }
        if (GUI.Button(new Rect(10, 110, 100, 40), "Run best"))
        {
			RefreshFileName();

            RunBest();
        }

		fileName = GUI.TextField(new Rect(10, 160, 100, 20), fileName);

		if( GUI.Button(new Rect(10, 190, 100, 40), "Load Brain"))
		{
			RefreshFileName();

			_ea = experiment.CreateEvolutionAlgorithm(popFileSavePath);
			StartEA();
		}

        GUI.Label(new Rect(10, Screen.height - 70, 60, 150), string.Format("Generation: {0}\nFitness: {1:0.00}", Generation, Fitness));
    }

	// so we dont do it per update
	private void RefreshFileName()
	{
		champFileSavePath = Application.persistentDataPath + string.Format("/{0}.champ.xml", fileName);
		popFileSavePath = Application.persistentDataPath + string.Format("/{0}.pop.xml", fileName);
	}
}
