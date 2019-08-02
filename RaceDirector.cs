using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RaceDirector : MonoBehaviour
{
    [Header("Public Reference")]
    public CameraFollow cameraFollow;
    public CinemachineDollyCart dollyCart;
    public CinemachineSmoothPath smoothPath;

    [Space]
    [Header("Track Mapping")]
    public Vector2 defaultLimit = new Vector2(11.65f, 4.15f);
    public Vector2 testLimit = new Vector2(60,30);

    [Space]
    [Header("Parameters")]
    public float duration;

    private float velocity = 0;
    float startTime;

    void Start()
    {

        startTime = Time.time;
    }

    void Update()
    {

        //Between Waypoints 1 & 4, Camera limit is increased
        if (dollyCart.m_Position >= smoothPath.m_Waypoints[1].position.z && dollyCart.m_Position <= smoothPath.m_Waypoints[4].position.z)
            CameraTransition(testLimit);
        else
            CameraTransition(defaultLimit);

    }

    void CameraTransition(Vector2 newVector)
    {
        float t = (Time.time - startTime) / duration;
        cameraFollow.limits.x = Mathf.SmoothStep(cameraFollow.limits.x, newVector.x,  t);
        cameraFollow.limits.y = Mathf.SmoothStep(cameraFollow.limits.y, newVector.y,  t);

    }

}
