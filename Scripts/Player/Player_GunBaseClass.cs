using UnityEngine;
using System.Collections;

public class Player_GunBaseClass : MonoBehaviour {

	[System.Serializable]
	public class GunStats
	{
		float _fireRate;
		public float curFireRate { get {return _fireRate;} set { _fireRate = Mathf.Clamp(value, 0.5f, 2f);}}
		public float startingFireRate;

		public int weaponIndex;

		public void Init(int index)
		{
			curFireRate = startingFireRate;
			weaponIndex = index;
		}
	}

	public GunStats gunStats = new GunStats();


	public Transform sightStart, sightEnd; // where the gun's range starts and ends
	
	public LayerMask mask;

	public GameObject target;

	public float countDownToFire = 0; // counts up in seconds until it reaches fire rate

	public bool canFire = false;

	public ObjectPool objPool;


	public void FollowMouse()
	{
		Vector3 targetMouse = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		float z = Mathf.Atan2 ((targetMouse.y - sightStart.position.y), (targetMouse.x - sightStart.position.x)) * Mathf.Rad2Deg - 90;		
		sightStart.rotation = Quaternion.AngleAxis (-z, Vector3.forward);

	}

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
