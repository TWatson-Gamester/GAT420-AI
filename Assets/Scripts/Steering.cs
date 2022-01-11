using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering : MonoBehaviour
{
    [SerializeField] float wanderDistance = 1;
    [SerializeField] float wanderRadius = 3;
    [SerializeField] float wanderDisplacement = 5;

    float wanderAngle = 0;

    public Vector3 Seek(AutonomousAgent agent, GameObject target)
    {
        Vector3 Force = CalculateSteering(agent, target.transform.position - agent.transform.position);

        return Force;
    }

    public Vector3 Flee(AutonomousAgent agent, GameObject target)
    {
        Vector3 Force = CalculateSteering(agent, agent.transform.position - target.transform.position);

        return Force;
    }

    public Vector3 Wander(AutonomousAgent agent)
    {
        Vector3 Force = Vector3.zero;
        wanderAngle = wanderAngle + Random.Range(-wanderDisplacement, wanderDisplacement);
        Quaternion rotation = Quaternion.AngleAxis(wanderAngle, Vector3.up);
        Vector3 point = rotation * (Vector3.forward * wanderRadius);
        Vector3 forward = agent.transform.forward * wanderDistance;
        Force = CalculateSteering(agent, (forward + point));
        return Force;
    }

    Vector3 CalculateSteering(AutonomousAgent agent, Vector3 vector)
    {
        Vector3 direction = vector.normalized;
        Vector3 desired = direction * agent.maxSpeed;
        Vector3 steer = desired - agent.velocity;
        Vector3 force = Vector3.ClampMagnitude(steer, agent.maxSpeed);
        return force;
    }
}
