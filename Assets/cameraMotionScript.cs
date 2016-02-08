using UnityEngine;
using System.Collections;

public class cameraMotionScript : MonoBehaviour {

	public Transform car;
    public float distance = 6.4f;
    public float height = 1.4f;
    public float rotationDamping = 3.0f;
    public float heightDamping = 2.0f;
    public float zoomRacio = 0.5f;
    public float DefaultFOV = 60;
    public int mode = 0;

    private Vector3 rotationVector;

    void LateUpdate () {
        switch(mode)
        {
            case 0:
                var wantedAngel = rotationVector.y;
                var wantedHeight = car.position.y + height;
                var myAngel = transform.eulerAngles.y;
                var myHeight = transform.position.y;
                myAngel = Mathf.LerpAngle(myAngel, wantedAngel, rotationDamping * Time.deltaTime);
                myHeight = Mathf.Lerp(myHeight, wantedHeight, heightDamping * Time.deltaTime);
                var currentRotation = Quaternion.Euler(0, myAngel, 0);
                transform.position = new Vector3(car.position.x, myHeight, car.position.z);
                transform.position -= currentRotation * Vector3.forward * distance;
                transform.LookAt(car);
                break;
            case 1:
                transform.position = new Vector3(car.position.x, car.position.y+2, car.position.z+1);
                transform.localRotation = car.localRotation;
                break;
            case 2:
                float extrax = 10;
                float extray = 0;
                transform.position = new Vector3(Mathf.Clamp(car.position.x, 125 - extrax, 125 + extrax), 250, Mathf.Clamp(car.position.z, 175 - extray, 175 + extray));
                transform.localRotation = Quaternion.Euler(90,0,0);
                break;
            case 3:
                break;
        }
    }
    void FixedUpdate (){
        var carrigidbody = car.GetComponent<Rigidbody>();
        var localVilocity = car.InverseTransformDirection(carrigidbody.velocity);
        //if (localVilocity.z<-0.5){
        //rotationVector.y = car.eulerAngles.y + 180;
        //}
        //else {
        rotationVector.y = car.eulerAngles.y;
        //}
        float acc = 0;
        if (mode == 0)
        {
            acc = carrigidbody.velocity.magnitude;
        }
        GetComponent<Camera>().fieldOfView = DefaultFOV + acc*zoomRacio;
    }
}
