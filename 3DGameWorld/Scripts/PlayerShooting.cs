using System.IO.Compression;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{


    public Gun gun;
    public Transform gunHolder;
    private bool isHoldingShoot = false;
    //private bool isZoomedIn = false;

    void OnZoomButton()
    {
        Debug.Log("pressed zoom");
        if(gun != null)
        {
            gun.ToggleZoom();
        }
    }
    void OnShoot()
    {
        Debug.Log("tried shoot! command");
        isHoldingShoot = true;
    }



    void OnShootRelease()
    {
        isHoldingShoot = false;
    }

    void OnReload()
    {
        if(gun != null)
        {
            gun.TryReload();
        }
    }




 
    void Update()
    {
        if(isHoldingShoot && gun != null)
        {
            gun.Shoot();
        }
    }



    public void OnDrop()
    {
        if(gun != null)
        {
            gun.Drop();
            gun = null;
        }
    }

}
