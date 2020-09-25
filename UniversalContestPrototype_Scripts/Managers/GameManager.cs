using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
	public static GameManager instance;
	[SerializeField]
	GameObject battlefieldAnchor;
	float battlefieldScale = 1;

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
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void Zoom(float zoomFactor)
	{
		battlefieldScale += zoomFactor * Time.deltaTime;
		battlefieldScale = Mathf.Clamp (battlefieldScale, 0.7f, 2.3f);
		battlefieldAnchor.transform.localScale = new Vector3 (battlefieldScale, battlefieldScale, battlefieldScale);
	}

	public void Rotate(float direction)
	{
		battlefieldAnchor.transform.Rotate (new Vector3(0, (direction * 48) * Time.deltaTime, 0));
	}
}
