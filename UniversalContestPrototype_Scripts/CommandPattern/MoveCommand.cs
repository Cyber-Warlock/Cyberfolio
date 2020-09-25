using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : ICommand<Vessel> 
{
	Vector3 targetCoordinates;

	public MoveCommand(Vector3 targetCoordinates)
	{
		this.targetCoordinates = targetCoordinates;
	}
	public void Enter(Vessel obj)
	{
		
	}
	public void Execute(Vessel obj)
	{
		if (Vector3.Distance(obj.transform.position, targetCoordinates) > 0.05f) 
		{
			Quaternion targetRotation = Quaternion.LookRotation (targetCoordinates - obj.transform.position, Vector3.up);
			obj.transform.rotation = Quaternion.Slerp (obj.transform.rotation, targetRotation, (obj.VesselSpeed * 5f) * Time.deltaTime);
			obj.transform.position = Vector3.MoveTowards (obj.transform.position, obj.transform.position + obj.transform.forward, obj.VesselSpeed * Time.deltaTime);
		} 
		else 
		{
			obj.FSM.IssueCommand(obj, new IdleCommand ());
		}
	}
	public void Exit(Vessel obj)
	{
		
	}
}
