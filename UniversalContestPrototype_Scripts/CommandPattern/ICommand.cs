using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand<Vessel>
{
	void Enter(Vessel obj);
	void Execute (Vessel obj);
	void Exit (Vessel obj);
}
