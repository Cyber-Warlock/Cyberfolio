using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Vessel : MonoBehaviour 
{
	protected FSM<Vessel> fsm;
	protected string vesselName;
    protected int vesselPoints;
	protected int vesselHealth;
	protected float vesselMass;
	protected float vesselSpeed;
	protected List<GameObject> vesselWeapons;

	public FSM<Vessel> FSM
	{
		get { return fsm; }
	}
	public string VesselName
	{
		get { return vesselName; }
	}
	public float VesselSpeed
	{
		get { return vesselSpeed; }
	}

	// Use this for initialization
	protected virtual void Start () 
	{
		fsm = new FSM<Vessel>();
		fsm.IssueCommand (this, new IdleCommand ());
	}
	
	// Update is called once per frame
	protected virtual void Update () 
	{
		fsm.Execute (this);
	}
}
