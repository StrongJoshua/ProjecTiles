using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snipe : SpecialFire {

    public int cost;
    // Use this for initialization

    private LineRenderer line;
    private Vector3[] linePositions;
	public Material laserMat;

    public override void startAim()
    {
        LineRenderer line = unit.gameObject.AddComponent(typeof(LineRenderer)) as LineRenderer;
		line.material = laserMat;
        Vector3[] positions = new Vector3[2];
        positions[0] = gameObject.transform.position;
		positions[1] = unit.aimRing.transform.forward * 10 + new Vector3(0,1,0);
        linePositions = positions;
        line.startColor = Color.red;
        line.endColor = Color.red;
        line.receiveShadows = false;
        line.SetPositions(positions);

        this.line = line;
    }

    public void Update()
    {
        if (line != null)
        {
			linePositions[1] = unit.aimRing.transform.forward * 10 + new Vector3(0,1,0);
            line.SetPositions(linePositions);
        }
    }

    public override void stopAim()
    {
        Destroy(line);
        line = null;
    }
    public override void fire()
    {
        if (unit.canShoot())
        {
            Debug.Log("pew");
            // do something
        }
    }
}
