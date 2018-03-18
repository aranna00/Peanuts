using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectOnInput : MonoBehaviour
{
	public EventSystem EventSystem;
	public GameObject SelectedObject;
	private bool _buttonSelected;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxisRaw("vertical") != 0&&!_buttonSelected)
		{
			EventSystem.SetSelectedGameObject(SelectedObject);
			_buttonSelected = true;
		}
	}

	private void OnDisable()
	{
		_buttonSelected = false;
	}
}
