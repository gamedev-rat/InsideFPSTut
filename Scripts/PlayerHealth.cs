using UnityEngine;

public class PlayerHealth : MonoBehaviour
{


    public int health = 100;
    public AudioClip hitSFX;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Damage")
        {
            DecreaseHealth(10);
        }
    }


    private void DecreaseHealth(int decreaseAmount)
    {
        health -= decreaseAmount;
        //add screen shake here TBD
        UIManager.Instance.InstantiateHitUI();
        AudioManager.Instance.PlaySFX(hitSFX);



        if(health <=0)
        {
            Die();
        }
    }

    private void Die()
    {
        Time.timeScale = 0f; //pausing the game
    }

}
