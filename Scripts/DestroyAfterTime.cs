using UnityEngine;

public class DestroyFlash : MonoBehaviour
{
    
    public float time;

    void Start()
    {
        Destroy(gameObject, time);
    }


}
