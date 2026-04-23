using UnityEngine;
using UnityEngine.InputSystem;

public class PickUp : MonoBehaviour
{
    
    public AudioClip equipSFX;
    public float equipSFXVol = 0.2f;

    public Material highlightMaterial;
    private Material[] originalMaterials;
    private MeshRenderer[] meshRenderers;

    public GameObject weaponPrefab;
    public float lookRange = 3f;

    private bool isLookedAt = false;
    private Camera playerCam;
    private PlayerShooting player;


    void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        originalMaterials = new Material[meshRenderers.Length];
        for(int i = 0; i < meshRenderers.Length; i++)
        {
            originalMaterials[i] = meshRenderers[i].material;
        }

        player = FindAnyObjectByType<PlayerShooting>();
        playerCam = player.GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        if(Physics.Raycast(ray, out RaycastHit hit, lookRange))
        {
            if(hit.collider.GetComponentInParent<PickUp>() == this)
            {
                if (!isLookedAt)
                {
                    SetLookedAt(true);

                    
                }
                return;
            }
        }


        if (isLookedAt)
        {
            SetLookedAt(false);
        }
    }

    void SetLookedAt(bool lookedAt)
    {
        isLookedAt = lookedAt;

        if (lookedAt)
        {
            foreach(MeshRenderer mr in meshRenderers)
            {
                mr.material = highlightMaterial;
            }
        }
        else
        {
            for(int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material = originalMaterials[i];
            }
        }
    }


    public void OnPickUp()
    {
        Debug.Log("pressed E");
        if(!isLookedAt) return;

        player.OnDrop();

        AudioManager.Instance.PlaySFX(equipSFX,equipSFXVol);
        GameObject newWeapon = Instantiate(weaponPrefab, player.gunHolder);
        //newWeapon.transform.localPosition = Vector3.zero;
        //newWeapon.transform.localRotation = Quaternion.identity;

        player.gun = newWeapon.GetComponent<Gun>();

        Destroy(gameObject);

    }


}
