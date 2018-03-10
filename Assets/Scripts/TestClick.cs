using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class TestClick : MonoBehaviour
{
	private Random rnd = new Random();
	private List<String> _images = new List<string>();
	private Sprite _sprite;
	
	// Use this for initialization
	void Start () {
		_images.Add("Almond");
		_images.Add("Brazil");
		_images.Add("Cashew");
		_images.Add("Coconut");
		_images.Add("Hazelnut");
		_images.Add("Peanut");
		_images.Add("Pecan");
		_images.Add("Pine");
		_images.Add("Pistachio");
		_images.Add("Walnut");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnMouseDown()
	{
		GameObject go = new GameObject();
		
		string imageToLoad = "Sprites/Nuts/" + _images[rnd.Next(_images.Count)];
		_sprite = Resources.Load(imageToLoad,typeof(Sprite)) as Sprite;

		Debug.Log(imageToLoad);
		
	 	CircleCollider2D circleCollider2D = go.AddComponent<CircleCollider2D>();
		circleCollider2D.radius = _sprite.bounds.size.y / 2;
		
		go.transform.position = new Vector2(rnd.Next(10)-5,rnd.Next(10)-5);
		go.AddComponent<TestClick>();
		
		SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = _sprite;
	}

}
