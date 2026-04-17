using UnityEngine;
using System.Collections;
using System.Numerics; //for coroutines
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;



public class Gun : MonoBehaviour
{
    [SerializeField] private AudioClip reloadSFX;
    [SerializeField] private float reloadSFXVol = 0.1f;
    [SerializeField] private AudioClip shootingSFX;
    [SerializeField] private float shootingVol = 0.25f;

    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private float fireRate = 0.15f;
    [SerializeField] private int magSize = 20;

    [SerializeField] private Transform hipPos;
    [SerializeField] private Transform zoomPos;


    [SerializeField] private bool Zoomed = false;

    public GameObject bullet;
    public Transform bulletSpawnPoint;



    public GameObject weaponFlash;
    public GameObject droppedWeapon;

    public float recoilDistance = 0.1f;
    public float recoilSpeed = 15f;


    private int currentAmmo;
    private bool isReloading = false;
    private float nextTimeToFire = 0f;

    private Quaternion initialRotation;
    private Vector3 initialPosition;
    private Vector3 reloadRotationOffset = new Vector3(66, 50, 50);




    void Start()
    {
        currentAmmo = magSize;
        initialRotation = transform.localRotation;
        initialPosition = transform.localPosition;

    }

    public void Shoot()
    {
        if (isReloading) return;
        if (Time.time < nextTimeToFire) return;

        Debug.Log("shoot");
        if(currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        
        nextTimeToFire = Time.time + fireRate;
        currentAmmo--;

        AudioManager.Instance.PlaySFX(shootingSFX, shootingVol);

        Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Instantiate(weaponFlash, bulletSpawnPoint.position, bulletSpawnPoint.rotation);



        StopCoroutine(nameof(Recoil)); //so no two coroutines at the same time. should this be reload?
        StartCoroutine(nameof(Recoil));
    }


    IEnumerator Reload()
    {
        AudioManager.Instance.PlaySFX(reloadSFX,reloadSFXVol);
        isReloading = true;

        Quaternion targetRotation = Quaternion.Euler(initialRotation.eulerAngles + reloadRotationOffset);
        float halfReload = reloadTime / 2f;
        float t = 0f;

        while(t < halfReload)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(initialRotation, targetRotation, t/ halfReload);
            yield return null;
        }

        t = 0f;

        while(t < halfReload)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(targetRotation, initialRotation, t/ halfReload);
            yield return null;
        }

        currentAmmo = magSize;
        isReloading = false;
    }

    public void TryReload()
    {
        if (isReloading) return;
        if (currentAmmo == magSize) return;

        StartCoroutine(Reload());
    }


    private IEnumerator Recoil()
    {
        Vector3 testpos;
        if(Zoomed)
        {
            testpos = zoomPos.localPosition;
        }
        else
        {
            testpos = hipPos.localPosition;
        }
        
        Vector3 recoilTarget = testpos + new Vector3(recoilDistance, 0, 0);
        float t = 0f;

        while(t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            transform.localPosition = Vector3.Lerp(testpos, recoilTarget, t);
            yield return null;
        }

        t = 0f;

        while(t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            transform.localPosition = Vector3.Lerp(recoilTarget, testpos, t);
            yield return null;
        }

        //transform.localPosition = initialPosition;
    }


    public void ToggleZoom()
    {
        
        Zoomed = !Zoomed;
        StartCoroutine(ZoomLerp());
       // MoveZoomLoc();
    }

    [SerializeField] private float zoomSpeed = 4f;
    private IEnumerator ZoomLerp()
    {
        Vector3 initial;
        Vector3 final;
        
        if(Zoomed)
        {
            initial = hipPos.localPosition;
            final = zoomPos.localPosition;
        }
        else
        {
            initial = zoomPos.localPosition;
            final = hipPos.localPosition;
        }
        
        
        float t = 0f;

        while(t < 1f)
        {
            t += Time.deltaTime * zoomSpeed;
            transform.localPosition = Vector3.Lerp(initial, final, t);
            yield return null;
        }

        //transform.localPosition = final;
    }
    private void MoveZoomLoc()
    {
        if(Zoomed == true)
        {

            gameObject.transform.localPosition = zoomPos.localPosition;
           
        } 
        else
        {
            gameObject.transform.localPosition = hipPos.localPosition;
           
        }
    }



    public void Drop()
    {
        Instantiate(droppedWeapon, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
