using UnityEngine;
using System.Collections;

public class Player_FreezeGun : Player_GunBaseClass {

	public float frozenTime = 2f; // time in seconds target units stay frozen when hit

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

	// Check for a HIT and do EFFECT:

	void Update()
	{
		if (target) {
			// If gun linecast has hit a target, then freeze the enemy here
			FreezeEnemy();
		}

		CanFire ();
	}

	// Shoot with LEFT CLICK:

	void FixedUpdate () {

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

	void FreezeEnemy()
	{
		if (target.GetComponent<Enemy_MoveHandler> ()) {
			Enemy_MoveHandler enemy = target.GetComponent<Enemy_MoveHandler> ();

			// freeze the enemy by changing its Move Handler's state to FROZEN
			enemy.state = Enemy_MoveHandler.State.FROZEN;

			// tell the enemy for how long it will be frozen
			enemy.frozenTime = frozenTime;

			// instantiate a visual FX from the pool
			GameObject fx = objPool.GetObjectForType("Frozen Particles", true);
			if (fx){
				fx.transform.position = enemy.transform.position;
			}

			// After freezing this enemy make target null so we stop calling this method
			target = null;

		} else {
			// If it couldn't find the Enemy Move Handler component it's probably because the unit is already dead
			target = null;
		}
	}



}
