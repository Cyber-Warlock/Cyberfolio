using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM<Vessel> 
{
	ICommand<Vessel> currentCommand;

	public void Execute(Vessel obj)
	{
		currentCommand.Execute (obj);
	}

	public void IssueCommand(Vessel obj, ICommand<Vessel> newCommand)
	{
		if (currentCommand != null) {
			currentCommand.Exit (obj);
		}
		currentCommand = newCommand;
		newCommand.Enter (obj);
	}
}
