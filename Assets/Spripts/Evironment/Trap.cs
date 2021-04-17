using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    Collider2D trapCollider;
    Animator anim;

    public float animationStartDelay;

    void Start()
    {
        trapCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        
        StartCoroutine(DelayAnimation());


    }

    IEnumerator DelayAnimation()
	{
        anim.enabled = false;
        yield return new WaitForSeconds(animationStartDelay);
        anim.enabled = true;
    }
    
    public void ToggleColider()
	{
        trapCollider.enabled = !trapCollider.enabled;
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(transform);
        }
    }
}
