using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerScript : MonoBehaviour
{
    static Vessel vesselToSelect;
    static Vessel selectedVessel;
    static bool abilityMenuOpen;
    public SteamVR_TrackedController device;

    // Use this for initialization
    void Start()
    {
        device.TriggerClicked += TriggerClicked;
        device.PadTouched += TrackpadTouch;
        device.Gripped += GripButtonClicked;
    }

    // Update is called once per frame
    void Update()
    {
		if ((device.controllerState.rAxis0.x + device.controllerState.rAxis0.y) != 0) {
			if (device.gripped) {
				GameManager.instance.Zoom (device.controllerState.rAxis0.x + device.controllerState.rAxis0.y);
			} 
			else if (Mathf.Abs(device.controllerState.rAxis0.x) > 0.5f)
			{
				GameManager.instance.Rotate (device.controllerState.rAxis0.x);
			}
		}
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Vessel")
        {
            vesselToSelect = other.gameObject.GetComponent<Vessel>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Vessel")
        {
            vesselToSelect = null;
        }
    }

    void TriggerClicked(object sender, ClickedEventArgs e)
    {
		if (selectedVessel == null && vesselToSelect != null) 
		{
			selectedVessel = vesselToSelect;
			vesselToSelect = null;
		} 
		else if (selectedVessel != null) 
		{
			selectedVessel.FSM.IssueCommand (selectedVessel, new MoveCommand (gameObject.transform.position));
		} 
		else 
		{
			RaycastHit hitInfo;
			if (Physics.Raycast(transform.position, transform.forward, out hitInfo))
			{
				selectedVessel = hitInfo.collider.gameObject.GetComponent<Vessel>();
			}
		}
		if (selectedVessel != null) 
		{
			selectedVessel.GetComponent<MeshRenderer>().material.color = Color.green;
		}
    }

    void TrackpadTouch(object sender, ClickedEventArgs e)
    {
		if (device.controllerState.rAxis0.y > 0.7f && selectedVessel != null) 
		{
			Debug.Log ("Open Ability Menu");
		}
    }

    void GripButtonClicked(object sender, ClickedEventArgs e)
    {
		if (selectedVessel != null) 
		{
			selectedVessel.GetComponent<MeshRenderer>().material.color = Color.white;
			selectedVessel = null;
		}
    }
}
