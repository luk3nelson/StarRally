using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class PlayerController : MonoBehaviour {
	static public PlayerController S;

    private Transform playerModel;
    private PlayerShield playerShield;

    [Header("Settings")]
    public bool joystick = true;
    
    [Space]

    [Header("Public Parameters")]
    public float movement = 30;
    public float forwardSpeed = 6;
    public float shieldLoss = 1;
    public float shieldGain = 2;

    [Space]

	[Header ("Rotation Parameters")]
	public float rollMult = -45;
	public float pitchMult = 30;
	public float gameRestartDelay = 2f;

    [Header("Public Reference")]
    public CinemachineDollyCart dolly;
    public Transform cameraParent;
	public GameObject projectilePrefab;
	public float projectileSpeed = 40;

	[Header("Set Dynamically")]
	[SerializeField]
	private float _shieldLevel = 1;

	void Start()
    {
		if (S == null) {
			S = this;
		} else {
			Debug.Log ("Player.Awake() tried to be a sneaky little fuck");
		}

        playerModel = transform.GetChild(0);
        playerShield = GetComponent<PlayerShield>();

        SetSpeed(forwardSpeed);
	}

	void Update() {

        //Input
        float xAxis = joystick ? Input.GetAxis("Horizontal") : Input.GetAxis("Mouse X") ;
		float yAxis = joystick ? Input.GetAxis ("Vertical") : Input.GetAxis("Mouse Y");

        //Get Button Inputs need to be called from the Update function since its called every frame
        if (Input.GetButtonDown("Action"))
            Boost(true);

        if (Input.GetButtonUp("Action"))
            Boost(false);

        if (Input.GetButtonDown("Brake"))
            Brake(true);

        if (Input.GetButtonUp("Brake"))
            Brake(false);

        /*
		// Change transform.position based on the axes
		Vector3 pos = transform.position;
		pos.x += xAxis * movement * Time.deltaTime;
		pos.y += yAxis * movement * Time.deltaTime;
		transform.position = pos;
        */

        LocalMove(xAxis, yAxis, movement);

		//Rotate ship to make it feel dynamic
		transform.localRotation = Quaternion.Euler(-yAxis*pitchMult,0,xAxis*rollMult);
        //ClampPosition();

	}

    void LocalMove(float x, float y, float speed)
    {
        transform.localPosition += new Vector3(x, y, 0) * speed * Time.deltaTime;
        ClampPosition();
    }

    void ClampPosition()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    void SetSpeed(float x)
    {
        dolly.m_Speed = x;
    }

    //Camera Settings
    void SetCameraZoom(float zoom, float duration)
    {
        cameraParent.DOLocalMove(new Vector3(0, 0, zoom), duration);
    }

    void FieldOfView(float fov)
    {
        cameraParent.GetComponentInChildren<CinemachineVirtualCamera>().m_Lens.FieldOfView = fov;
    }
    //End of Camera Settings

    void Boost(bool state)
    {
        //Debug.Log("FAST");
        float origFOV = state ? 40 : 55;
        float endFOV = state ? 55 : 40;
        float speed = state ? forwardSpeed * 2 : forwardSpeed;
        float zoom = state ? -7 : 0;

        DOVirtual.Float(origFOV, endFOV, .5f, FieldOfView);
        DOVirtual.Float(dolly.m_Speed, speed, .15f, SetSpeed);
        SetCameraZoom(zoom, .4f);

        //Decrease Player Shield
        playerShield.ReduceShield(shieldLoss);
    }

    void Brake(bool state)
    {
        float speed = state ? forwardSpeed / 3 : forwardSpeed;
        float zoom = state ? 3 : 0;

        DOVirtual.Float(dolly.m_Speed, speed, .15f, SetSpeed);
        SetCameraZoom(zoom, .4f);

        //Regen Player Shield
        playerShield.RegenShield(shieldGain);
    }
}
