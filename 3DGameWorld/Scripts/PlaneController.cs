using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;



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

    public float lift = 135f; //based on how many mph thhis plane need to get off the ground
    [SerializeField] private Transform propeller;

    private float responseModifier
    {
        get
        {
            return (rb.mass / 10f) * responsiveness;
        }
    }

    Rigidbody rb;
    AudioSource engineSound;
    [SerializeField] private TextMeshProUGUI hud;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        engineSound = GetComponent<AudioSource>();
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
        UpdateHUD();
        propeller.Rotate(Vector3.up *throttle);
        engineSound.volume = throttle * 0.01f; // if throttle is 100 it is set at volume of 1 (goes from 0 to 1)
    }

    private void FixedUpdate()
    {
        
        //apply forces to our plane
        rb.AddForce(transform.forward * maxThrust * throttle);
        rb.AddTorque(transform.up * yawInput * responseModifier);
        rb.AddTorque(transform.right * pitchInput * responseModifier);
        rb.AddTorque(transform.forward * rollInput * responseModifier);

        //forward speed = Vector3.Dot(rb.velocity, transform.forward)
        //rb.AddForce(rb.transform.up * rb.linearVelocity.magnitude * lift);
        rb.AddForce(rb.transform.up * Vector3.Dot(rb.linearVelocity, transform.forward) * lift);

        
    }

    private void UpdateHUD()
    {
        hud.text = "Throttle " + throttle.ToString("F0")+"%\n";// 0 decimal values of this float
        hud.text += "Airspeed: " + (rb.linearVelocity.magnitude * 3.6f * 0.62f).ToString("F0") + "mph\n"; //unity units into kilometer (if you are treating unity units as a meter ) then into miles
        hud.text += "Altitude: " + transform.position.y.ToString("F0") + " m";
    }

}
