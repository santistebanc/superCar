using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Motor))]
public class PlayerControls : MonoBehaviour {

    public int selectedcamera = 0;
    public cameraMotionScript cam;

    private bool free = true;

    private bool enab = true;

    void FixedUpdate()
    {

        Motor motor = GetComponent<Motor>();

        if (enab && motor.tim.startrace)
        {

            float accel = Input.GetAxis("Vertical");
            float turn = Input.GetAxis("Horizontal");
            float brake = Input.GetAxis("Jump");

            motor.Move(turn, accel, 0f, 0f);
            motor.HandBrake(Input.GetAxis("Fire1"));

        }
        else
        {
            motor.Move(0, 0, 1f, 0f);
        }

        if (Input.GetAxis("Camera") == 0)
        {
            free = true;
        }
        if (free && Input.GetAxis("Camera") == 1)
        {
            free = false;
            selectedcamera++;
        }
        if (free && Input.GetAxis("Camera") == -1)
        {
            free = false;
            selectedcamera--;
        }
        if (selectedcamera == 4)
        {
            selectedcamera = 0;
        }
        if (selectedcamera == -1)
        {
            selectedcamera = 3;
        }

        switch (selectedcamera)
        {
            case 0:
                cam.distance = 15;
                cam.height = 10;
                cam.mode = 0;
                break;
            case 1:
                cam.mode = 1;
                break;
            case 2:
                cam.distance = 5;
                cam.height = 50;
                cam.mode = 0;
                break;
            case 3:
                cam.mode = 2;
                break;
        }

    }

    public void finished()
    {
        enab = false;
        cam.mode = 3;
    }

}

