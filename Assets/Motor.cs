using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

[RequireComponent(typeof(Rigidbody))]
public class Motor : MonoBehaviour {

    public timer tim;

    private bool respawned = false;
    private bool waitframe = true;

    private Vector3 startpos;
    private Quaternion startrot;

    public Color color;

    public bool crossmiddle = false;
    public int lap = 1;

    public float enginepower = 200f;
    public float turnpower = 20f;
    public float brake = 1000f;

    public bool hittingFloor = false;

    public Rigidbody m_Rigidbody;
    public Wheel FRWheel;
    public Wheel FLWheel;
    public Wheel BRWheel;
    public Wheel BLWheel;

    [Range(0, 1)] public float m_SteerHelper;

    public WheelCollider[] m_WheelColliders = new WheelCollider[4];
    public WheelEffects[] m_WheelEffects = new WheelEffects[4];

    public float MaxSpeed = 200;
    public float Revs = 0;
    public float m_SlipLimit = 0.3f;

    public float CurrentSpeed { get { return m_Rigidbody.velocity.magnitude * 2.23693629f; } }
    public float AccelInput { get; private set; }

    private float m_OldRotation;


    private void Start()
    {
        tim = transform.root.GetComponentInChildren<timer>();
        m_Rigidbody = GetComponent<Rigidbody>();
        startpos = transform.position;
        startrot = transform.rotation;
    }

    public void resetIt()
    {
        transform.rotation = startrot;
        transform.position = startpos;
        respawned = true;
        crossmiddle = false;
        lap = 1;

        for (int i = 0; i < 4; i++)
        {
            m_WheelColliders[i].motorTorque = 0;
            m_WheelColliders[i].steerAngle = 0;
            m_WheelColliders[i].steerAngle = 0;
            m_WheelColliders[i].brakeTorque = Mathf.Infinity;
        }
        m_Rigidbody.isKinematic = true;
        respawned = true;
        waitframe = false;
    }

    void LateUpdate()
    {
        // (if the vehicle has been respanwed this frame, 
        //        then a variable respawned is set to true)

        if (waitframe && respawned)
        {
            waitframe = false;
            for (int i = 0; i < 4; i++)
            {
                m_WheelColliders[i].brakeTorque = 0;
            }
            m_Rigidbody.isKinematic = false;
            respawned = false;
        }
        else if (!waitframe && respawned)
        {
            waitframe = true;
        }
    }

    void FixedUpdate()
    {

        

        hittingFloor = false;
        for (int i = 0; i < 4; i++)
        {
            if (m_WheelColliders[i].isGrounded)
            {
                hittingFloor = true;
                break;
            }
        }

        Revs = Mathf.Lerp(Revs, Mathf.Abs(CurrentSpeed / 200), Time.deltaTime * 5f);

        CheckForWheelSpin();
        SteerHelper();

        float posx = Mathf.Clamp(transform.position.x,10,240);
        float posz = Mathf.Clamp(transform.position.z,10,340);

        transform.position = new Vector3(posx, transform.position.y,posz);


    }

    public void Move(float turn, float accel, float thebrake, float other)
    {
        float torque = accel * enginepower;

        AccelInput = Mathf.Clamp(torque, 0, 1);

        FRWheel.Move(torque);
        FLWheel.Move(torque);
        BRWheel.Move(torque);
        BLWheel.Move(torque);

        float steer = turn * turnpower;

        FRWheel.Turn(steer);
        FLWheel.Turn(steer);

        for (int i = 0; i < 4; i++)
        {
            if (CurrentSpeed > 5)
            {
                m_WheelColliders[i].brakeTorque = brake * thebrake;
            }
            //else if (footbrake > 0)
            //{
            //    m_WheelColliders[i].brakeTorque = 0f;
            //    m_WheelColliders[i].motorTorque = -enginepower * footbrake;
            //}
        }

    }


    public void HandBrake(float val)
    {

        for (int i = 0; i < 4; i++)
        {
            WheelFrictionCurve fc = m_WheelColliders[i].sidewaysFriction;
            
            fc.extremumSlip = Mathf.Lerp(0.1f, 0.4f, val);
            fc.asymptoteSlip = Mathf.Lerp(0.2f, 0.5f, val);
            m_WheelColliders[i].sidewaysFriction = fc;
        }

    }

    private void CheckForWheelSpin()
    {
        // loop through all wheels

        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelHit;
            m_WheelColliders[i].GetGroundHit(out wheelHit);

            // is the tire slipping above the given threshhold
            if (Mathf.Abs(wheelHit.sidewaysSlip) >= m_SlipLimit)
            {
                // avoiding all four tires screeching at the same time
                // if they do it can lead to some strange audio artefacts
                if (!AnySkidSoundPlaying())
                {
                    m_WheelEffects[i].PlayAudio();
                }
                continue;
            }

            // if it wasnt slipping stop all the audio
            if (m_WheelEffects[i].PlayingAudio)
            {
                m_WheelEffects[i].StopAudio();
            }
            // end the trail generation
            m_WheelEffects[i].EndSkidTrail();
        }
    }

    private bool AnySkidSoundPlaying()
    {
        for (int i = 0; i < 4; i++)
        {
            if (m_WheelEffects[i].PlayingAudio)
            {
                return true;
            }
        }
        return false;
    }

    private void SteerHelper()
    {
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelhit;
            m_WheelColliders[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return; // wheels arent on the ground so dont realign the rigidbody velocity
        }

        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
        if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
        }
        m_OldRotation = transform.eulerAngles.y;
    }

    public void hitMark(Transform mark)
    {
        if (mark.name == "middle")
        {
            crossmiddle = true;
        }
        if (crossmiddle && mark.name == "begin")
        {
            crossmiddle = false;
            lap++;
            if (lap == tim.laps + 1)
            {
                SendMessage("finished");
            }
            tim.addlap(transform,lap,color);
        }
    }

}
