using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;

   // public void Init(bool isOffset)
   // {

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        
    }
 
    public void Init(bool isOffset)
    {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }
    


    void OnMouseEnter() //old input system. needed to be set as both in project settings
    {
        
        _highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        _highlight.SetActive(false);
    }
}
