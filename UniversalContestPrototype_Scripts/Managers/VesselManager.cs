using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselManager : MonoBehaviour 
{
	public static VesselManager instance = null;
	Dictionary<string, Vessel> vessels;

	void Awake()
	{
		if (instance == null) 
		{
			instance = this;
		} 
		else if (instance != this) 
		{
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start() 
	{
		vessels = new Dictionary<string, Vessel>();
		Vessel[] intermediary = GetComponents<Vessel>();
		foreach (Vessel vessel in intermediary) 
		{
			vessels.Add(vessel.VesselName, vessel);	
		}
	}

	public Vessel GetVessel(string name)
	{
		if (vessels.ContainsKey(name)) 
		{
			return vessels[name];
		}
		return null;
	}
}
