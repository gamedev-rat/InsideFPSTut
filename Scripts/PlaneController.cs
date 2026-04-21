using UnityEngine;
using UnityEngine.InputSystem;
public class PlaneController : MonoBehaviour
{
    [Header("Plane Stats")]
    [Tooltip("How much the throttle ramps up or down.")]
    [SerializeField] private float throttleIncrement = 0.1f;
    public float maxThrust = 200f;
    public float responsiveness = 10f; //factors in the mass (see below)


    private float throttle;
    private float yawInput;
    private float pitchInput;
    private float rollInput;
    private float throttleInput;

    [SerializeField] private Transform propeller;

    private float responseModifier
    {
        get
        {
            return (rb.mass / 10f) * responsiveness;
        }
    }

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

   
    void OnYaw(InputValue value)
    {
        yawInput = value.Get<float>();
    }
    void OnPitch(InputValue value)
    {
        pitchInput = value.Get<float>();
    }
    void OnRoll(InputValue value)
    {
        rollInput = value.Get<float>();
    }
    void OnThrottle(InputValue value)
    {
        throttleInput = value.Get<float>();
    }

    private void HandleInputs()
    {
        //roll = Input.GetAxis("Roll");
        //pitch = Input.GetAxis("Pitch");
        //yaw = Input.GetAxis("Yaw");

        //if (Input.GetKey(KeyCode.Space)) throttle += throttleIncrement;
        //else if (Input.GetKey(KeyCode.LeftControl)) throttle -= throttleIncrement;
        throttle += (throttleInput * throttleIncrement);
        throttle = Mathf.Clamp(throttle, 0f, 100f);
    }

    private void Update()
    {
        HandleInputs();
        propeller.Rotate(Vector3.up *throttle);
    }

    private void FixedUpdate()
    {
        
        //apply forces to our plane
        rb.AddForce(transform.forward * maxThrust * throttle);
        rb.AddTorque(transform.up * yawInput * responseModifier);
        rb.AddTorque(transform.right * pitchInput * responseModifier);
        rb.AddTorque(transform.forward * rollInput * responseModifier);
    }

}
