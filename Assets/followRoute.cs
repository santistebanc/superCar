using UnityEngine;
using System.Collections;

public class followRoute : MonoBehaviour {

    public Transform route;
    public Transform curentTarget;
    private int num = 0;
    private int count;
    public CarAI caraicontrol;
	void Start () {
        curentTarget = route.GetChild(0);
        count = route.childCount;
        caraicontrol = GetComponent<CarAI>();
        caraicontrol.SetTarget(curentTarget);
	}

    public void hitMark(Transform mark)
    {
        num++;
        if (num == count)
        {
            num = 0;
        }
        curentTarget = mark;
        caraicontrol.SetTarget(curentTarget);
    }

    public void resetIt()
    {
       num = 0;
       curentTarget = route.GetChild(0);
       caraicontrol.SetTarget(curentTarget);
    }

}
