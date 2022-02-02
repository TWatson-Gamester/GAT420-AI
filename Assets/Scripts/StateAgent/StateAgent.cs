using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAgent : Agent
{
    public StateMachine stateMachine = new StateMachine();

    void Start()
    {
        stateMachine.AddState(new IdleState(this, "idle"));
        stateMachine.AddState(new PatrolState(this, "patrol"));
        stateMachine.SetState(stateMachine.StateFromName("idle"));
    }


    void Update()
    {
        stateMachine.Update();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.SetState(stateMachine.StateFromName("patrol"));
        }

        if (movement.velocity.magnitude > .5f)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
}
