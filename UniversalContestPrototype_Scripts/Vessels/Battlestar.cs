using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battlestar : Vessel
{
	public const int hardpoints = 12;
	public string temp;

	void Awake()
	{
		vesselName = temp;
		vesselSpeed = 0.2f;
	}

	// Use this for initialization
	protected override void Start () 
	{
		base.Start ();
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		base.Update ();
	}
}
