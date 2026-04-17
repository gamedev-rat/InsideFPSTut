using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; //lets you call this easier as a global instance, combined with the awake() function below

    public GameObject hitUI;

    private void Awake()
    {
        Instance = this;
    }

    public void InstantiateHitUI()
    {
        Instantiate(hitUI, transform);
    }
}
