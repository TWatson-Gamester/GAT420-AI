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
        wanderAngle = wanderAngle + Random.Range(-wanderDisplacement, wanderDisplacement);
        Quaternion rotation = Quaternion.AngleAxis(wanderAngle, Vector3.up);
        Vector3 point = rotation * (Vector3.forward * wanderRadius);
        Vector3 forward = agent.transform.forward * wanderDistance;
        Vector3 Force = CalculateSteering(agent, (forward + point));
        return Force;
    }

    public Vector3 Cohesion(AutonomousAgent agent, GameObject[] neighbors)
    {
        Vector3 centerOfTargets = Vector3.zero;
        foreach (GameObject target in neighbors)
        {
            centerOfTargets += target.transform.position;
        }

        centerOfTargets /= neighbors.Length;
        Vector3 force = CalculateSteering(agent, centerOfTargets - agent.transform.position);
        return force;
    }

    public Vector3 Seperation(AutonomousAgent agent, GameObject[] targets, float radius)
    {
        Vector3 separation = Vector3.zero;
        foreach (GameObject target in targets)
        {
            Vector3 direction = (agent.transform.position - target.transform.position);
            if (direction.magnitude < radius)
            {
                separation += direction / direction.sqrMagnitude;
            }
        }

        Vector3 force = CalculateSteering(agent, separation);
        return force;
    }

    public Vector3 Alignment(AutonomousAgent agent, GameObject[] targets)
    {
        Vector3 averageVelocity = Vector3.zero;
        foreach (GameObject target in targets)
        {
            averageVelocity += target.GetComponent<AutonomousAgent>().velocity;
        }
        averageVelocity /= targets.Length;
        Vector3 force = CalculateSteering(agent, averageVelocity);
        return force;
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
