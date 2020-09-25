using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyScript : MonoBehaviour
{
    static bool dummyCalibrated;
    [SerializeField]
    float timeInCalibration;
    [SerializeField]
    GameObject dummy;
    [SerializeField]
    float dummyCalibrationHeight = 0.5f;
    [SerializeField]
    GameObject optimalCompressionZone;
    bool handsInOptimalZone;
    /// <summary>
    /// The controller actively doing calibration decided by y axis
    /// </summary>
    GameObject calibratingController;
    /// <summary>
    /// The global anchor object all position-sensitive objects are children of to synchronize with floor
    /// </summary>
    GameObject globalAnchor;

    /// <summary>
    /// Gets whether or not the dummy has been calibrated this session
    /// </summary>
    public static bool DummyCalibrated
    {
        get { return dummyCalibrated; }
    }
    public bool HandsInOptimalZone
    {
        get { return handsInOptimalZone; }
        set { handsInOptimalZone = value; }
    }

    // Use this for initialization
    void Start()
    {
        dummyCalibrated = false;
        timeInCalibration = 0;
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (!dummyCalibrated && ControllerTracker.Instance.ViveControllers.Length > 0)
            {
                Calibration();
            }
        }
        catch (System.NullReferenceException e)
        {
            Debug.LogError("[DummyScript] Controllers couldn't be found");
        }
    }

    void Calibration()
    {
        foreach (GameObject controller in ControllerTracker.Instance.ViveControllers)
        {
            if (calibratingController == null || controller.transform.position.y < calibratingController.transform.position.y)
            {
                calibratingController = controller;
            }
            if (calibratingController == controller && controller.transform.position.y < dummyCalibrationHeight)
            {
                timeInCalibration += Time.deltaTime;
                if (timeInCalibration > 5)
                {
                    GameObject.FindGameObjectWithTag("GlobalAnchor").transform.position = new Vector3(0, controller.transform.position.y - 0.145f, 0);
                    GameObject.FindGameObjectWithTag("GlobalAnchor").transform.Rotate(0, controller.transform.rotation.eulerAngles.y, 0);
                    Vector3 pos = new Vector3(  controller.transform.position.x,
                                                controller.transform.position.y,
                                                controller.transform.position.z);
                    //Vector3 rot = new Vector3(0, controller.transform.rotation.eulerAngles.y, 0);
                    //gameObject.transform.Rotate(rot);
                    gameObject.transform.position = pos;
                    dummyCalibrated = true;
                    ChestCompresssionInput.SetPlanesActive(false);
                    timeInCalibration = 0;
                    StartCoroutine(AdjustCompressionZone());
                }
            }
            else if (calibratingController == controller)
            {
                timeInCalibration = 0;
            }
        }
    }

    IEnumerator AdjustCompressionZone()
    {
        Transform tracker = GameObject.FindGameObjectWithTag("ViveTracker").transform;
        while (true)
        {
            if (!handsInOptimalZone)
            {
                if (tracker.position.x < optimalCompressionZone.transform.position.x ||
                    tracker.position.z < optimalCompressionZone.transform.position.z)
                {
                    optimalCompressionZone.transform.localPosition = new Vector3(0.05f, 0.67f, 0.05f);
                }
                else if (tracker.position.x > optimalCompressionZone.transform.position.x ||
                           tracker.position.z > optimalCompressionZone.transform.position.z)
                {
                    optimalCompressionZone.transform.localPosition = new Vector3(-0.05f, 0.67f, -0.05f);
                }
            }
            yield return new WaitForSecondsRealtime(2f);
        }
    }
}
