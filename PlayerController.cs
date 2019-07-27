using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	static public PlayerController S;

	[Header ("Set in Inspector")]
	public float movement = 30;
	public float rollMult = -45;
	public float pitchMult = 30;
	public float gameRestartDelay = 2f;

	public GameObject projectilePrefab;
	public float projectileSpeed = 40;
	public Weapon[] weapons;

	[Header("Set Dynamically")]
	[SerializeField]
	private float _shieldLevel = 1;

	private GameObject lastTriggerGo = null;

	//Defining a new delegate type and field
	public delegate void WeaponFireDelegate ();
	public WeaponFireDelegate fireDelegate;

	void Start(){
		if (S == null) {
			S = this;
		} else {
			Debug.Log ("Player.Awake() tried to be a sneaky little fuck");
		}

		//fireDelegate += TempFire;

		ClearWeapons ();
		weapons [0].SetType (WeaponType.blaster);
	}

	void FixedUpdate() {
		
		//Input
		float xAxis = Input.GetAxis("Horizontal");
		float yAxis = Input.GetAxis ("Vertical");

		// Change transform.position based on the axes
		Vector3 pos = transform.position;
		pos.x += xAxis * movement * Time.deltaTime;
		pos.y += yAxis * movement * Time.deltaTime;
		transform.position = pos;

		//Rotate ship to make it feel dynamic
		transform.rotation = Quaternion.Euler(-yAxis*pitchMult,0,xAxis*rollMult);

		//Allow the ship to fire
		if (Input.GetKeyDown (KeyCode.Z)) {
			TempFire ();
		}

		if (Input.GetKeyDown (KeyCode.Z) && fireDelegate != null) {
			fireDelegate ();
		}
	}

	void OnTriggerEnter (Collider other){
		Transform rootT = other.gameObject.transform.root;
		GameObject go = rootT.gameObject;
		Debug.Log ("Triggered: " + go.name);

		if (go == lastTriggerGo) {
			return;
		}

		lastTriggerGo = go;

		if (go.tag == "Enemy") {
			Debug.Log (shieldLevel);
			shieldLevel--;
			Destroy (go);
		} else if(go.tag == "PowerUp"){
			AbsorbPowerUp (go);
		}else {
			print ("Triggered by non-Enemy: " + go.name);
		}
	}

	public void AbsorbPowerUp(GameObject go){
		PowerUp pu = go.GetComponent<PowerUp> ();
		switch (pu.type) {
		case WeaponType.shield:
			shieldLevel++;
			break;

			default:
			if(pu.type == weapons[0].type){
				Weapon w = GetEmptyWeaponSlot();
				if (w != null){
					w.SetType(pu.type);
				}
			} else {
				ClearWeapons();
				weapons[0].SetType(pu.type);
			}
			break;
		}
		pu.AbsorbedBy (this.gameObject);
	}

	//Shield Level Property
	public float shieldLevel{
		get {
			return (_shieldLevel);
		} 
		set {
			_shieldLevel = Mathf.Min (value, 4);
			if (value < 0) {
				Debug.Log ("Destroy");
				Destroy (gameObject);
				//Resart game
				Main.S.DelayedRestart(gameRestartDelay);
			}
		}
	}

	Weapon GetEmptyWeaponSlot(){
		for (int i = 0; i < weapons.Length; i++) {
			if (weapons [i].type == WeaponType.none) {
				return(weapons [i]);
			}
		}
		return null;
	}

	void ClearWeapons(){
		foreach (Weapon w in weapons) {
			w.SetType (WeaponType.none);
		}
	}

	void TempFire(){
		Vector3 shootPos = new Vector3 (transform.position.x, transform.position.y, transform.position.z + 2);

		GameObject projGO = Instantiate<GameObject> (projectilePrefab);
		projGO.transform.position = shootPos;
		Rigidbody rigidB = projGO.GetComponent<Rigidbody> ();
		//rigidB.velocity = Vector3.forward * projectileSpeed;

		Projectile proj = projGO.GetComponent<Projectile> ();
		proj.type = WeaponType.blaster;
		float tSpeed = Main.GetWeaponDefinition (proj.type).velocity;
		rigidB.velocity = Vector3.forward * tSpeed;

	}
}
