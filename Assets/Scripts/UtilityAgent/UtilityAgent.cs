using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityAgent : Agent
{
    Need[] needs;
    [SerializeField] MeterUI meter;
    public float happiness
    {
        get
        {
            float totalMotive = 0;
            foreach(Need need in needs)
            {
                totalMotive += need.motive;
            }
            return 1 - (totalMotive / needs.Length);
        }
    }

    private void Start()
    {
        needs = GetComponentsInChildren<Need>();
        meter.text.text = "";
    }


    private void Update()
    {
            animator.SetFloat("speed", movement.velocity.magnitude);
        meter.slider.value = happiness;
        meter.worldPosition = transform.position + (Vector3.up * 4);
    }
}
