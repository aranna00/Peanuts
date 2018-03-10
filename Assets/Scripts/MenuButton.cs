using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		AudioSource audio = gameObject.AddComponent<AudioSource>();
		AudioClip clip = Resources.Load<AudioClip>("Ultimate GUI Kit/Sounds/Menu Selection Click");
		audio.clip = clip;
	}
	
	// Update is called once per frame
	void Update ()
	{

	}
	private void OnMouseEnter()
	{
		GetComponent<AudioSource>().Play();
		GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1.1f,1.1f,1.1f);
	}

	private void OnMouseExit()
	{
		GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f,1f,1f);
	}
}
