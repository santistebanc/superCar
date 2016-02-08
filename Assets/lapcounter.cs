using UnityEngine;
using System.Collections;

public class lapcounter : MonoBehaviour {

    public int lap = 1;
    public int countcars;
    public bool crossedgoal = true;

    public timer timer;

    void Start() {
	    countcars = transform.parent.Find("Cars").childCount;
	}
}
