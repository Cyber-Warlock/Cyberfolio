using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrackerMovement
{
    Up,
    Down,
    Still
}

public class ControllerTracker : MonoBehaviour
{
    #region Fields
    // Singleton
    static ControllerTracker instance;
    /// <summary>
    /// Position of VIVE Tracker
    /// </summary>
    Vector3 controllerPosition;
    /// <summary>
    /// The tracker position last frame
    /// </summary>
    [SerializeField]
    Vector3 trackerPosLastFrame;
    /// <summary>
    /// Direction the VIVE Tracker is travelling in real space for compressions
    /// </summary>
    TrackerMovement direction;
    /// <summary>
    /// The controller velocity.
    /// </summary>
    float verticalVelocity;
    [SerializeField]
    GameObject optimalZoneFeedback;
    [SerializeField]
    GameObject[] viveControllers;
    GameObject globalAnchor;
    #endregion

    #region Properties
    public static ControllerTracker Instance
    {
        get { return instance; }
    }
    /// <summary>
    /// Position of VIVE Tracker
    /// </summary>
    public Vector3 ControllerPosition
    {
        get { return controllerPosition; }
    }
    /// <summary>
    /// Get: The tracker position last frame
    /// </summary>
    public Vector3 TrackerPosLastFrame
    {
        get { return trackerPosLastFrame; }
    }
    /// <summary>
    /// Get: Direction the VIVE Tracker is travelling in real space for compressions
    /// </summary>
    public TrackerMovement Direction
    {
        get { return direction; }
    }
    public float VerticalVelocity
    {
        get { return verticalVelocity; }
    }
    public GameObject[] ViveControllers
    {
        get { return viveControllers; }
    }
    #endregion

    void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
        instance = this;
        trackerPosLastFrame = Vector3.zero;
        optimalZoneFeedback.GetComponent<MeshRenderer>().material.color = Color.red;
        globalAnchor = GameObject.FindGameObjectWithTag("GlobalAnchor");
		StartCoroutine (CheckFloorCalibration ());
		viveControllers = new GameObject[1];
    }

    // Update is called once per frame
    void Update()
    {
        if (viveControllers.Length < 2 && !DummyScript.DummyCalibrated)
        {
            viveControllers = GameObject.FindGameObjectsWithTag("ViveController");
        }
        controllerPosition = transform.position;
        if ((trackerPosLastFrame.y - controllerPosition.y) < -0.005)
        {
            direction = TrackerMovement.Up;
        }
        else if ((trackerPosLastFrame.y - controllerPosition.y) > 0.005)
        {
            direction = TrackerMovement.Down;
        }
        else
        {
            direction = TrackerMovement.Still;
        }
        /*if (direction != TrackerMovement.Still)
        {
            verticalVelocity = Vector3.Distance(controllerPosition, trackerPosLastFrame) / Time.deltaTime;
        }
        else
        {
            verticalVelocity = 0;
        }*/
    }
    private void LateUpdate()
    {
        trackerPosLastFrame = transform.position;
    }

	IEnumerator CheckFloorCalibration()
	{
		yield return new WaitForSeconds (5f);
		while (true) 
		{
			//If you get an error here, you probably removed the object the GameManager script was attached to
			if (GameManager.Instance.GameState != GameState.Calibration)
			{
				Debug.Log ("[ControllerTracker]: viveControllers size = " + viveControllers.Length);
				foreach (GameObject controller in viveControllers)
				{
					if (controller.transform.position.y < 0.3f &&
						(controller.transform.position.y < globalAnchor.transform.position.y - 0.05 ||
							controller.transform.position.y > globalAnchor.transform.position.y + 0.05))
					{
						globalAnchor.transform.position = new Vector3(0, controller.transform.position.y, 0);
					}
				}
			}
			yield return new WaitForSecondsRealtime (2f);
		}
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "OptimalCompressionZone")
        {
            optimalZoneFeedback.GetComponent<MeshRenderer>().material.color = UIColor.GetColorFromHex(UIColor.UIGreen);
			other.GetComponentInParent<DummyScript> ().HandsInOptimalZone = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "OptimalCompressionZone")
        {
            optimalZoneFeedback.GetComponent<MeshRenderer>().material.color = UIColor.GetColorFromHex(UIColor.UIRed);
			other.GetComponentInParent<DummyScript> ().HandsInOptimalZone = false;
        }
    }
}