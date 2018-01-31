using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public int currentOkurky;
	public Text okurkyText;
	public Transform Spawnpoint;
	public GameObject Prefab;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (currentOkurky == 40)
		{
			Instantiate(Prefab, Spawnpoint.position, Spawnpoint. rotation);
			Destroy(gameObject);
		}	
	}
	
	
	public void AddOkurky(int okurkyToAdd)
	{
		currentOkurky += okurkyToAdd;
		okurkyText.text = "Okurky: " + currentOkurky;
	}

}
