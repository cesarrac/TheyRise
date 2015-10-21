using UnityEngine;
using System.Collections;

public class Player_AssaultRifle : Player_GunBaseClass {

	public float explosiveDamage = 2f; 
	
	// To find weapon when selecting available weapons
	public int wpnIndex = 1;

	
	void Awake () 
	{
		// Initialize gun stats
		gunStats.Init (wpnIndex);
		
		
	}
	
	void Start()
	{
		objPool = GetComponentInParent<Player_HeroAttackHandler> ().objPool;
	}
	
	// Check for a HIT:
	
	void Update()
	{
		if (target) {
		

			// Do Gun Effect
			DamageEnemy();

		}
		
		CanFire ();
	}

	
	// Shoot with LEFT CLICK:
	
	void FixedUpdate ()
	{
		FollowMouse ();


		if (Input.GetMouseButtonDown (0)) {
			if (canFire){
				ShootRay();
				// Reset countdown to fire
				countDownToFire = 0;
				canFire = false;
			}
		}
	}
	
	// The GUN EFFECT:
	void DamageEnemy()
	{
		if (target.GetComponent<Unit_Base> ()) {
			Unit_Base enemy = target.GetComponent<Unit_Base> ();

			if (enemy.stats.curHP > 0){
	
				// instantiate a visual FX from the pool
				GameObject fx = objPool.GetObjectForType("MachineGun_ShootFX", true);
				if (fx){
					fx.transform.position = enemy.transform.position;
				}

				/* This guns explodes a chunk or part off of the enemy unit IF the unit only has a 4th of their HP left */

				// Get the ammount that is a quarter of this enemy's HP
				float quarterHP = enemy.stats.maxHP * 0.25f;

				if (enemy.stats.curHP <= quarterHP){
					// Blow up a chunk
				}else{
					// just do damage
					enemy.TakeDamage(explosiveDamage);
				}


				target = null;
			}else{
				target = null;
			}

		} else {
			// If it couldn't find the Enemy Move Handler component it's probably because the unit is already dead
			target = null;

		}
	}
}
