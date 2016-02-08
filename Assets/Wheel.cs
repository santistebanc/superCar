using UnityEngine;
using System.Collections;

[RequireComponent(typeof(WheelCollider))]
public class Wheel : MonoBehaviour {

    WheelCollider wc;

	void Awake () {
        wc = GetComponent<WheelCollider>();
	}

    public void Move(float val)
    {
        wc.motorTorque = val;
        wc.motorTorque *= 0.9f;
	}

    public void Turn(float val)
    {
        wc.steerAngle = val;
    }

}
