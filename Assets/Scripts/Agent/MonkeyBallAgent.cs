﻿using UnityEngine;
using MLAgents;
using System.Collections;

public class MonkeyBallAgent : Agent
{
    [Header("Specific to MonkeyBall Unity")]

    public GameObject goal;
    public GameObject deathPlane;
    public GameObject sphere;
    public GameObject floor;

    FollowCamera followCamera;
    Rigidbody m_BallRb;

    float reset_x, reset_y, reset_z;
    IFloatProperties m_resetParams;

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        m_BallRb = sphere.GetComponent<Rigidbody>();
        followCamera = Camera.main.GetComponent<FollowCamera>();

        // Floor rot: (0, 0, 0)

        // TODO: Generalize to map choice
        // floor rotation decided upon map loading
        reset_x = 0;
        reset_y = 1;
        reset_z = -12;

        SetResetParameters();
    }

    public override void CollectObservations()
    {
        base.CollectObservations();

        /*****   OBSERVATIONS   *****/
        
        // Current speed (?)
        AddVectorObs(m_BallRb.velocity);

        // Rotation [x, z] of the floor/ground
        AddVectorObs(floor.transform.rotation.x);
        AddVectorObs(floor.transform.rotation.z);

        // Relative distance between ball and floor / goal
        AddVectorObs(m_BallRb.position - floor.transform.position);
        AddVectorObs(m_BallRb.position - goal.transform.position);

        // _____________ Added with the Raycast 3D Component _______________
        // - Perception of the floor underneath (am I in contact with the floor?)
        // - Perception of the floor ahead (can I move forward?)
        // - Perception of obstacles ahead (can I move without finding obstacles?)
        
    }

    public void GoalReached()
    {
        AddReward(5f);

        Done();

        // -- Do a little cutesy something to celebrate success and feel accomplished as a monkey
        // StartCoroutine(GoalReachedAnimation(0.5f))
    }

    IEnumerator GoalReachedAnimation(float time)
    {
        yield return new WaitForSeconds(time);
    }

    public override void AgentAction(float[] vectorAction)
    {
        // move agent 

        floor.transform.Rotate(Camera.main.transform.forward, vectorAction[0]);
        floor.transform.Rotate(Camera.main.transform.right, vectorAction[1]);
        
        followCamera.MoveCamera(vectorAction[2]);

        // Penalty given each step to encourage agent to finish task quickly
        AddReward(-1f / maxStep);
    }

    public override float[] Heuristic()
    {
        var action = new float[3];

        action[0] = -Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        action[2] = Input.GetAxis("Mouse X");

        return action;
    }

    public override void AgentReset()
    {
        // TODO: 
        // - Place Ball at starting position
        // - turn Floor to starting position
        // - reset flags

        // ball to original position
        m_BallRb.position = new Vector3(reset_x, reset_y, reset_z);

        // floor to no rotation
        floor.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));

        SetResetParameters();
    }


    public void SetResetParameters()
    {
        // TODO: Changes exec parameters (mass, floor turn speed, etc)
        
    }

    private void FixedUpdate()
    {
        transform.rotation = floor.transform.rotation;
    }
}


