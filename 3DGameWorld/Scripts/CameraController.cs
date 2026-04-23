using UnityEngine;
using UnityEngine.InputSystem;
public class CameraController : MonoBehaviour
{
    [Tooltip("An array of transforms representing camera positions")]
    [SerializeField] Transform[] povs;

    [SerializeField] private float speed; //speed at which the camera follows the plane. some delay to see the movement in the plane

    private int index = 0;
    private Vector3 target;

    private void Update()
    {
        if(Keyboard.current.gKey.wasPressedThisFrame) index = 0;
        else if (Keyboard.current.hKey.wasPressedThisFrame)  index = 1;
        else if (Keyboard.current.jKey.wasPressedThisFrame)  index = 2;
        else if (Keyboard.current.kKey.wasPressedThisFrame)  index = 3;

        target = povs[index].position;
    }

    private void FixedUpdate()
    {
        //move camera to desired position/orientation. must be here in FixedUpdate to avoid camera jitters (ie if Update() and FixedUpdate have some lag between)
        transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime *speed);
        transform.forward = povs[index].forward;

    }

}
