using UnityEngine;
using System.Collections;

public class Player_GunBaseClass : MonoBehaviour {

	[System.Serializable]
	public class GunStats
	{
		float _fireRate;
		public float curFireRate { get {return _fireRate;} set { _fireRate = Mathf.Clamp(value, 0.5f, 2f);}}
		public float startingFireRate;

		public void Init()
		{
			curFireRate = startingFireRate;
		}
	}

	public GunStats gunStats = new GunStats();


	public Transform sightEnd; // where the gun's range ends
	
	public LayerMask mask;

	public GameObject target;

	public float countDownToFire = 0; // counts up in seconds until it reaches fire rate

	public bool canFire = false;

	public ObjectPool objPool;




	public void CanFire()
	{
		if (countDownToFire >= gunStats.curFireRate) {
			canFire = true;
		} else {
			countDownToFire += Time.deltaTime;
		}
	}


	
	public void ShootRay()
	{
		Debug.DrawLine (transform.position, sightEnd.position, Color.cyan);
		Debug.Log ("Shooting!");
		RaycastHit2D hit = Physics2D.Linecast (transform.position, sightEnd.position, mask.value);
		if (hit.collider != null) {

			if (hit.collider.CompareTag ("Enemy")) {

				// Linecast HIT an enemy, so store the enemy unit as the target
				target = hit.collider.gameObject;

				Debug.Log("PLAYER GUN: Hit an enemy!");

			}
		}
		
	}
}
