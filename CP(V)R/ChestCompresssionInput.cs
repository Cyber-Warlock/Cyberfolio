using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public enum CompressionState
{
    Ended,
    Started
}

public class ChestCompresssionInput : MonoBehaviour
{
 


	MetricsManager metric;

    #region Fields
    //Store the total score from evaluation here while no database
    int totalScore;
    #region Compression Tracking
    /// <summary>
    /// Collection of delta times for each compression performed to track frequency
    /// </summary>
    [SerializeField]
    Queue<float> compressionDeltaTimes;
    /// <summary>
    /// Depth of the actual compression
    /// </summary>
	[SerializeField]
    float compressionDepth;
    /// <summary>
    /// Is the depth of actual compression deep enough
    /// </summary>
	[SerializeField]
    bool compressionMinDepth;

	float currentTimeWindow;
	int compressionCountWindow;

    /// <summary>
    /// State of actual compression
    /// </summary>
	[SerializeField]
    CompressionState compressionState;
    /// <summary>
    /// Time in current compression sequence
    /// </summary>
    float currentTimeForCompressions;
    /// <summary>
    /// Stores the instances of the triggerPlane objects
    /// </summary>
    [SerializeField]
    static GameObject[] triggerPlanes;
    /// <summary>
    /// Time since actual compression started
    /// </summary>
    float deltaTimeForActualCompression;
    /// <summary>
    /// For tracking how many compressions were not deep enough or too deep
    /// </summary>
	[SerializeField]

    static bool sequenceStarted;
    /// <summary>
    /// Currently unused
    /// </summary>
    static bool practice;
    /// <summary>
    /// Is the compression returning to start?
    /// </summary>
    bool returning;


    #region DisplayedFields
    /// <summary>
    /// For tracking how many compressions were not deep enough or too deep
    /// </summary>
    [SerializeField]
    int failedCompressions;
    /// <summary>
    /// Current rate of compressions
    /// </summary>
    float compressionRate;
    /// <summary>
    /// For tracking number of total compressions for sequence
    /// </summary>
    [SerializeField]
    int compressions;
	Coroutine frequencyRoutine;
	#endregion


    #region Prefabs
    /// <summary>
    /// The dummy model prefab.
    /// </summary>
    [SerializeField]
    GameObject dummyModel;
    #endregion
    List<Compression> compressionData;
    #endregion

    #region Properties
    /// <summary>
    /// Get: Field for tracking compressions in current sequence
    /// </summary>
    public int Compressions
    {
        get { return compressions; }
    }
    /// <summary>
    /// Get: Field for tracking number of failed compressions in current sequence
    /// </summary>
    public int FailedCompressions
    {
        get { return failedCompressions; }
    }
    /// <summary>
    /// Get: Field tracking depth of actual compression
    /// </summary>
    public float CompressionDepth
    {
        get { return compressionDepth; }
    }
    /// <summary>
    /// Get: Field for tracking time in current compression sequence
    /// </summary>
    public float CurrentTimeForCompressions
    {
        get { return currentTimeForCompressions; }
    }
    /// <summary>
    /// Get: Field for tracking rate of compressions
    /// </summary>
    public float CompressionRate
    {
        get { return compressionRate; }
    }
    public int TotalScore
    {
        get { return totalScore; }
        set { totalScore = value; }
    }

	public Vector2Int compressionInfo {
		get { 
			return new Vector2Int (compressions, failedCompressions);
		}
	}
    #endregion
	#endregion
    #region Methods
    // Use this for initialization
    void Start()
    {
		
        compressions = 0;
        failedCompressions = 0;
		metric = GameObject.FindObjectOfType<MetricsManager> ();
        compressionState = CompressionState.Ended;
        compressionMinDepth = false;
        returning = false;
        compressionDeltaTimes = new Queue<float>();
        triggerPlanes = new GameObject[2];
        compressionData = new List<Compression>();
    }

    // Update is called once per frame
    void Update()
    {
        //Are you sure this is necessary? Wasted writes to memory
		currentTimeWindow += Time.deltaTime;
		if (currentTimeWindow > 5f){
			//compressionRate = (compressions / currentTimeWindow) * 12; 
			currentTimeWindow = 0;
			compressionCountWindow = 0;
		}
        string feedbackMessage = "";
        //Ensures seqeuence is started before tracking time and rate
        if (sequenceStarted)
        {
			/*if (frequencyRoutine == null)
				frequencyRoutine = StartCoroutine (FrequencyCalculation ());*/

            currentTimeForCompressions += Time.deltaTime;
			/*
			if (compressionRate < 90) {
				metric.SetFeedbackText ("Speed Up", UIColor.UIOrange);
			} else if (compressionRate > 110) {
				metric.SetFeedbackText ("Slow Down", UIColor.UIOrange);
			} else {
				metric.SetFeedbackText ("Good", UIColor.UIGreen);
			}*/
        }
        //For doing stuff that should only be done while a compression is in progress
        if (compressionState == CompressionState.Started)
        {
            deltaTimeForActualCompression += Time.deltaTime;
            if (ControllerTracker.Instance.Direction == TrackerMovement.Down && compressionMinDepth && returning)
            {
                string.Format(feedbackMessage,
                                ((feedbackMessage.Length > 0) ? "\n" : ""),
                                "Allow Full Recoil");
            }
            compressionDepth = (triggerPlanes[0].transform.position.y - 0.01f) - gameObject.transform.position.y;
            //Calls event to update depth display
			metric.SetSliderValue(
                (triggerPlanes[0].transform.position.y - gameObject.transform.position.y) /
                (triggerPlanes[0].transform.position.y - triggerPlanes[1].transform.position.y)
            );
        }
		if (compressions >= 200 && sequenceStarted)
        {
            StartCompressions(false, false);
            SetPlanesActive(false);
            EvaluateCompression();
        }
    }

    /// <summary>
    /// Sets the TriggerPlanes as active or inactive
    /// </summary>
    public static void SetPlanesActive(bool active)
    {
        try
        {
            //Tries to iterate through plane collection to set active
            foreach (GameObject plane in triggerPlanes)
            {
                plane.SetActive(active);
            }
        }
        //Catches NullReferenceException if objects were lost and reacquires them
        catch (System.NullReferenceException e)
        {
            Debug.LogError(e.Message + "\nReacquiring TriggerPlanes");
            triggerPlanes[0] = GameObject.FindGameObjectWithTag("EnterCompression");
            triggerPlanes[1] = GameObject.FindGameObjectWithTag("CompressionMinDepth");
        }
        //Ensures planes are in the correct active state, in case NullReferenceException occurred
        finally
        {
            if (triggerPlanes[0].activeSelf != active)
            {
                foreach (GameObject plane in triggerPlanes)
                {
                    plane.SetActive(active);
                }
            }
        }
    }
    //Called when gameObject enters a triggerzone. Used for compression tracking
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "EnterCompression")
        {
            if (!sequenceStarted)
            {
                sequenceStarted = true;
            }
            if (compressionState == CompressionState.Ended)
            {
                compressionState = CompressionState.Started;
            }
            else
            {
                compressionState = CompressionState.Ended;
				if (compressionDeltaTimes.Count >= 20) 
				{
					compressionDeltaTimes.Dequeue ();
				}
				compressionDeltaTimes.Enqueue(deltaTimeForActualCompression);
                if (compressionMinDepth == false)
                {
                    failedCompressions++;
					metric.SetFeedbackText("Too Shallow", UIColor.UIOrange);
                }
				compressions++;
				compressionCountWindow++;
				compressionData.Add(new Compression(metric.MaxSliderValue * 5, compressionRate));
				metric.SetCompressionCount (new Vector2Int(compressions, failedCompressions));
                compressionMinDepth = false;
                returning = false;
                deltaTimeForActualCompression = 0f;
				metric.SpawnBar ();
            }
        }
        else if (other.gameObject.tag == "CompressionMinDepth" && !returning)
        {
            compressionMinDepth = true;
			metric.SetFeedbackText("Good!", UIColor.UIGreen);
        }
        else if (other.gameObject.tag == "CompressionMinDepth")
        {
            returning = true;
        }
    }

    /// <summary>
    /// Start as coroutine to make countdown
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public IEnumerator Countdown(int seconds)
    {
        do
        {
            seconds--;
            yield return new WaitForSeconds(1f);
        } while (seconds > 0);
        SetPlanesActive(true);
        sequenceStarted = true;
        yield return null;
    }

	IEnumerator FrequencyCalculation(){
		float totalDeltaTimes;
		while (true) {
			totalDeltaTimes = 0;
			foreach (float deltaTime in compressionDeltaTimes) {
				totalDeltaTimes += deltaTime;
			}
			compressionRate = compressionDeltaTimes.Count / (totalDeltaTimes / 60);
			yield return new WaitForSeconds (2f);
		}
	}

    /// <summary>
    /// Enables compression tracking
    /// </summary>
    /// <param name="state">Starts/Stops compression tracking</param>
    /// <param name="isPractice">unused</param>
    public static void StartCompressions(bool state, bool isPractice)
    {
        sequenceStarted = state;
        //practice = isPractice;
    }

    /// <summary>
    /// Completely resets compression tracking
    /// </summary>
    public void ResetCompressions()
    {
        sequenceStarted = false;
        compressions = 0;
        failedCompressions = 0;
        //OnCompressionCountChange(new Vector2Int(compressions, failedCompressions));
        compressionState = CompressionState.Ended;
        compressionDepth = 0;

		currentTimeWindow = 21f;
		compressionCountWindow = 0;
        ///OnDisplayCompressionDepth(compressionDepth);
        //OnFeedbackNeeded("");
		metric.SetFeedbackText ();
    }

    /// <summary>
    /// Evaluates compression sequence quality
    /// </summary>
    public void EvaluateCompression()
    {
        GameObject.FindGameObjectWithTag("Dummy").GetComponent<AudioSource>().Stop();
		totalScore = System.Convert.ToInt32((compressions - failedCompressions) / 2);
        new DataPrinter().PrintData(compressionData, totalScore, failedCompressions, compressions);
		GameManager.Instance.ProgressGame (GameState.AEDSequence);
    }

    struct Compression
    {
        public float depth, frequency;

        public Compression(float depth, float frequency)
        {
            this.depth = depth;
            this.frequency = frequency;
        }
    }

    class DataPrinter
    {
        string fileName;

        public DataPrinter()
        {
			fileName = System.DateTime.Now.ToOADate().ToString();
			fileName += ".txt";
        }

        public void PrintData(List<Compression> data, int total, int fails, int score)
        {
            using (StreamWriter sWriter = new StreamWriter("Assets\\Data\\" + fileName, false, System.Text.Encoding.UTF8))
            {
                sWriter.WriteLine("DateTime: " + fileName);
                float avgDepth = 0;
                float avgFrequency = 0;
                foreach (Compression compression in data)
                {
                    avgDepth += compression.depth;
                    avgFrequency += compression.frequency;
                }
                sWriter.WriteLine("Avg. Depth: " + (avgDepth / data.Count).ToString("N3"));
                sWriter.WriteLine("Avg. Frequency: " + (avgFrequency / data.Count).ToString("N3"));
                sWriter.WriteLine("Score: " + total.ToString() + " out of 100");
                sWriter.WriteLine("Total: " + score.ToString());
                sWriter.WriteLine("Fails: " + fails.ToString());
                sWriter.WriteLine("\n_________________________________________\n");
                sWriter.WriteLine("\tDepth (CM)\tFrequency (/MIN)");
                for (int i = 0; i < data.Count; i++)
                {
                    sWriter.WriteLine("#"+ i.ToString() + "\t" + data[i].depth.ToString("N2") + "\t\t" + data[i].frequency.ToString("N2"));
                }
            }
        }
    }

	#endregion
}
