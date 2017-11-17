using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medic : MonoBehaviour
{
    public List<Collider> triggerList;
    float currTime;
    float delay;
    Unit parent;
    MeshRenderer meshRenderer;
    // Use this for initialization
    void Start()
    {
        currTime = Time.timeSinceLevelLoad;
        delay = 2f;
        parent = GetComponentInParent<Unit>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        triggerList.RemoveAll((collider) => collider == null);
        if (Time.timeSinceLevelLoad - currTime > delay)
        {
            currTime = Time.timeSinceLevelLoad;
            foreach (Collider c in triggerList)
            {
                Unit unit = c.gameObject.GetComponent<Unit>();
                if (unit != null)
                {
                    unit.heal(1);
                }
            }
        }

        meshRenderer.enabled = parent.highlighted;
    }

    void OnTriggerEnter(Collider col)
    {
        if (!triggerList.Contains(col))
            triggerList.Add(col);
    }
    void OnTriggerExit(Collider col)
    {
        if (triggerList.Contains(col))
            triggerList.Remove(col);
    }
}
