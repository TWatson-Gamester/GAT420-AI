using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UtilityAgent : Agent
{
    Need[] needs;
    [SerializeField] Perception perception;
    [SerializeField] MeterUI meter;

    const float MIN_SCORE = .2f;
    public bool isUsingUtilityObject { get { return activeUtilityObject != null; } }

    UtilityObject activeUtilityObject;
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

        if (activeUtilityObject == null)
        {
            var gameObjects = perception.GetGameObjects();
            List<UtilityObject> utilityObjects = new List<UtilityObject>();
            foreach (var go in gameObjects)
            {
                if (go.TryGetComponent<UtilityObject>(out UtilityObject utilityObject))
                {
                    utilityObject.visible = true;
                    utilityObject.score = GetUtilityObjectScore(utilityObject);
                    if (utilityObject.score > .2f) utilityObjects.Add(utilityObject);
                }
            }

            activeUtilityObject = (utilityObjects.Count == 0) ? null : utilityObjects[0];
            if (activeUtilityObject != null)
            {
                StartCoroutine(ExecuteUtilityObject(activeUtilityObject));
            }

        }
    }

    private void LateUpdate()
    {
        meter.slider.value = happiness;
        meter.worldPosition = transform.position + (Vector3.up * 4);
        
    }

    float GetUtilityObjectScore(UtilityObject utilityObject)
    {
        float score = 0;

        foreach (var effector in utilityObject.effectors)
        {
            Need need = GetNeedByType(effector.type);
            if (need != null)
            {
                float futureNeed = need.getMotive(need.input + effector.change);
                score += need.motive - futureNeed;
            }
        }

        return score;
    }

    Need GetNeedByType(Need.Type type)
    {
        return needs.First(need => need.type == type);
    }

    void ApplyUtilityObject(UtilityObject utilityObject)
    {
        foreach (var effector in utilityObject.effectors)
        {
            Need need = GetNeedByType(effector.type);
            if (need != null)
            {
                need.input += effector.change;
                need.input = Mathf.Clamp(need.input, -1, 1);
            }
        }
    }

    IEnumerator ExecuteUtilityObject(UtilityObject utilityObject)
    {
        movement.MoveTowards(utilityObject.location.position);
        while (Vector3.Distance(transform.position, utilityObject.location.position) > 0.25f)
        {
            Debug.DrawLine(transform.position, utilityObject.location.position);
            yield return null;
        }

        print("start effect");
        if (utilityObject.effect != null) utilityObject.effect.Play();

        yield return new WaitForSeconds(utilityObject.duration);

        print("stop effect");
        if (utilityObject.effect != null) utilityObject.effect.Stop();

        ApplyUtilityObject(utilityObject);

        yield return null;
    }
}
