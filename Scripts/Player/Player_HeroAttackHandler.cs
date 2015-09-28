using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player_HeroAttackHandler : Unit_Base {
	Animator anim;

	[SerializeField]
	private SpriteRenderer weaponSprite;


	void Awake(){

		anim = GetComponent<Animator> ();
	}

	void Start () {
		// Initialize Unit stats
		stats.Init ();
	}
	

	void Update () {
		
		if (Input.GetMouseButton (0)) {
			anim.SetTrigger ("attack");
			if (anim.GetFloat("input_y") > 0){
				weaponSprite.sortingLayerName = "Units";
				weaponSprite.sortingOrder = -10;
			}else{
				weaponSprite.sortingLayerName = "Units Above";
				weaponSprite.sortingOrder = 10;
			}
		} else {
			anim.ResetTrigger("attack");
		}

		if (stats.curHP <= 0)
			Suicide ();
	}

	void Suicide()
	{
		// get a Dead sprite to mark my death spot
		GameObject deadE = objPool.GetObjectForType("dead", false); // Get the dead unit object
		if (deadE != null) {
			deadE.GetComponent<EasyPool> ().objPool = objPool;
			deadE.transform.position = transform.position;
		}
		
		// make sure we Pool any Damage Text that might be on this gameObject
		if (GetComponentInChildren<Text>() != null){
			Text dmgTxt = GetComponentInChildren<Text>();
			objPool.PoolObject(dmgTxt.gameObject);
		}
		
		// and Pool myself
		objPool.PoolObject (this.gameObject);
	}

}
