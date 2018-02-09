using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {

	public int maxHealth;
	public int currentHealth;
	
	PlayerController thePlayer;
	GameObject playerModel;
	SimpleHealthBar healthBar;
	
	public float InvincibilityLength;
	private float InvincibilityCounter;
	
	public Renderer playerRenderer;
	public Animator anim;
	private float flashCounter;
	public float flashLength = 0.1f;
	
	private bool isRespawning;
	private Vector3 respawnPoint;
	public float respawnLength;
	

	// Use this for initialization
	void Start () {
		currentHealth = maxHealth;
		thePlayer = FindObjectOfType<PlayerController>();
		playerModel = GameObject.FindGameObjectsWithTag("PlayerModel")[0];
		healthBar = SimpleHealthBar.GetSimpleHealthBar("HealthIndicator");
		Debug.Log(healthBar.gameObject.name);
		
		respawnPoint = thePlayer.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (InvincibilityCounter > 0)
		{
			InvincibilityCounter -= Time.deltaTime;
			
			flashCounter -= Time.deltaTime;
			if (flashCounter <= 0)
			{
				//playerRenderer.enabled = !playerRenderer.enabled;
				playerModel.SetActive(!playerModel.activeSelf);
				flashCounter = flashLength;
			}
			if (InvincibilityCounter <= 0)
			{
				//playerRenderer.enabled = true;
				playerModel.SetActive(true);
			}
				
		}
		
	}
	
	public void HurtPlayer(int damage, Vector3 direction)
	{
		if (InvincibilityCounter <= 0)
		{
			currentHealth = (currentHealth - damage);
			
			if (currentHealth <= 0)
			{
				healthBar.UpdateBar(0, maxHealth);
				anim.SetBool("Dead", true);
				Respawn();
			} else
			
			{
				healthBar.UpdateBar(currentHealth, maxHealth);
				thePlayer.Knockback(direction);
			
				InvincibilityCounter = InvincibilityLength;
				anim.SetTrigger("Hurt");
				//playerRenderer.enabled = false;
				//flashCounter = flashLength;
			}
		}
	}	
	
	
	public void Respawn()
	{
		//thePlayer.transform.position = respawnPoint;
		//currentHealth = maxHealth;
		
		if (!isRespawning)
		{
			StartCoroutine("RespawnCo");
		}
		
	}
	
	public IEnumerator RespawnCo()
	{
			isRespawning = true;
			thePlayer.gameObject.SetActive(false);
			
			yield return new WaitForSeconds(respawnLength);
			
			isRespawning = false;
			thePlayer.gameObject.SetActive(true);
			thePlayer.transform.position = respawnPoint;
			currentHealth = maxHealth;
			healthBar.UpdateBar(currentHealth, maxHealth);
			
			InvincibilityCounter = InvincibilityLength;
			playerRenderer.enabled = false;
			flashCounter = flashLength;
	}	
	
	public void HealPlayer(int healAmount)
	{
		currentHealth += healAmount;
		
		if (currentHealth > maxHealth)
		{
			currentHealth = maxHealth;
		}
	}
	
}
