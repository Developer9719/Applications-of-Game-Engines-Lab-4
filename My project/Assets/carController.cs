using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carController : MonoBehaviour
{
    public GameObject LeftTailLight, RightTailLight;
    public GameObject HeadlightLeft, HeadlightRight;
    private Rigidbody rb;

    // Headlights
    private bool areHeadlightsOn = false;

    // Steering
    private float currentSteeringAngle;
    public float maxSteeringAngle;

    //Braking
    private float currentBrakeForce;
    private bool isBreaking;
    public float brakeForce;

    // Motor
    public float motorForce;

    // Wheels 
    public WheelCollider frontLeftCollider, frontRightCollider, rearLeftCollider, rearRightCollider;
    public Transform frontLeftTransform, frontRightTransform, rearLeftTransform, rearRightTransform;

    // Input
    private float h;
    private float v;

    //Speed
    public float currentSpeed;
    public float topSpeed;

    // _drivetrain is the variable name

    public drivetrain _drivetrain = drivetrain.RWD;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize RB
        rb = GetComponent <Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetKeyDown(KeyCode.H))
        {
            areHeadlightsOn = !areHeadlightsOn;
            HeadlightLeft.SetActive(areHeadlightsOn);
            HeadlightRight.SetActive(areHeadlightsOn);
        }
    }

    // Runs once per frame at a fixed interval
    private void FixedUpdate()
    { 
        // Set the taillights
        LeftTailLight.SetActive(isBreaking);
        RightTailLight.SetActive(isBreaking);

        PlayerInput();
        Motor();
        Steering();
        UpdateWheels();
    }

    
    void PlayerInput()
    {
        // Set the input
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        isBreaking = Input.GetKey(KeyCode.S);
    }

    void Motor()
    {
        // If enum is RWD, move rear wheels 
        if (_drivetrain == drivetrain.RWD)
        {
            rearLeftCollider.motorTorque = v * motorForce;
            rearRightCollider.motorTorque = v * motorForce;
        }
        else if (_drivetrain == drivetrain.FWD) 
        { 
            frontLeftCollider.motorTorque = v * motorForce;
            frontRightCollider.motorTorque = v * motorForce;
        } else
        {
            rearLeftCollider.motorTorque = v * motorForce;
            rearRightCollider.motorTorque = v * motorForce;
            frontLeftCollider.motorTorque = v * motorForce;
            frontRightCollider.motorTorque = v * motorForce;
        }

        // Speed is velocity in a direction. defaults to m/s
        currentSpeed = rb.velocity.magnitude * 3.6f; // 3.6f converts to kph

        if (currentSpeed >= topSpeed) 
        { 
            currentSpeed = topSpeed;
        }

        // Stopping distance formula, offcial formula
        float stoppingDistance = (0.278f * Time.deltaTime * rb.velocity.magnitude + (Mathf.Pow(rb.velocity.magnitude, 2)) / (254 * (0.7f +1)));

        float brakingForce = rb.mass * (rb.velocity.magnitude/stoppingDistance);

        // If player is braking currentBrakingForce is the formula above, if not its 0
        currentBrakeForce = isBreaking ? brakingForce : 0f;

        ApplyBrakes(currentBrakeForce);

    }

    void ApplyBrakes(float brakez)
    {
        frontLeftCollider.brakeTorque = brakez;
        frontRightCollider.brakeTorque = brakez;
        rearLeftCollider.brakeTorque = brakez;
        rearRightCollider.brakeTorque = brakez;
    }

    void Steering()
    {
        currentSteeringAngle = maxSteeringAngle * h;
        frontLeftCollider.steerAngle = currentSteeringAngle;
        frontRightCollider.steerAngle = currentSteeringAngle;
    }

    void UpdateWheels()
    {
        UpdateOneWheel(frontLeftTransform, frontLeftCollider);
        UpdateOneWheel(rearLeftTransform, rearLeftCollider);
        UpdateOneWheel(frontRightTransform, frontRightCollider);
        UpdateOneWheel(rearRightTransform, rearRightCollider);
    }

    void UpdateOneWheel(Transform t, WheelCollider c)
    {
        Vector3 pos;
        Quaternion rot; // Rotation x,y,z

        // WheelColliders builtin function GetWorldPose to get position and rotation in world space
        c.GetWorldPose(out pos, out rot);
        // out sends the value somewhere else in this case it sends it out to the variables

        // Set transform yo be what the wheelcolliders position and rotation are
        t.position = pos;
        t.rotation = rot;
    }
}

// Enum is just a list. Its an array without a type needed
public enum drivetrain {
    RWD, FWD, AWD
}