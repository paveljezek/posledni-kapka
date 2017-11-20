using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOverTime : MonoBehaviour {

	public float lifetime;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	//Destroy(gameObject, lifetime);
	
	if (lifetime < 0)
	{
		Destroy(gameObject);
	}
	
	}
}
