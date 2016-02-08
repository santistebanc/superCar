using UnityEngine;
using System.Collections;

public class sendhitmes : MonoBehaviour {
    Transform nextBrotherNode;
	void OnTriggerEnter(Collider other) {
        int index = transform.parent.GetSiblingIndex();
        if (index < transform.parent.parent.childCount-1)
        {
            nextBrotherNode = transform.parent.parent.GetChild(index + 1);
        }
        else
        {
            nextBrotherNode = transform.parent.parent.GetChild(0);
        }
        other.gameObject.SendMessageUpwards("hitMark", nextBrotherNode);
    }
        
}
