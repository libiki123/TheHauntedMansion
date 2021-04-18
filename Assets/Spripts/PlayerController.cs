using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private Flashlight fl;
    private SpriteRenderer sr;

    private Vector2 movement;
    private Vector2 mousePos;

    private int numOfHeart = 3;
    private int curNumOfHeart;
    private float movementSpeed;

    private bool isInVulnerable = false;
    private bool isFalling = false;

    private int curGem = 0;
    private bool obtainedKey = false;

    public Camera cam;

    [Space]
    [Header("Timers")]
    float immnedTimer;
    public float immnedTimerSet = 2f;

    float blinkTimer;
    public float blinkTimerSet = 0.1f;

    [Space]
    [Header("Movements")]
    public float normalSpeed = 5f;
    public float sprintSpeed = 8f;
    public float rotationSpeed = 5f;

    [Space]
    [Header("Others")]
    public float invulnerabilityDuration = 2f;


    public event Action OnDamaged;
    public event Action OnObtainItem;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        fl = GetComponent<Flashlight>();
        sr = GetComponent<SpriteRenderer>();

        curNumOfHeart = numOfHeart;
        immnedTimer = immnedTimerSet;
        blinkTimer = blinkTimerSet;

        fl.OnBatteryDeplete += GameManager.Instance.OnPLayerBatteryDeplete;
        OnDamaged += GameManager.Instance.OnPlayerDamaged;
        OnObtainItem += GameManager.Instance.OnPLayerObtainItem;

    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckStatus();
        //ApplyAnimation();
    }

	private void FixedUpdate()
	{
        ApplyMovement();
        FaceMousePos();
	}

    private void CheckInput()
    {
        if (isFalling)
        {
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");        // Get player's x,y input
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();

        Vector3 pos = Input.mousePosition;
        pos.z = 10;                                 // CAN NOT pass 0 as z distance to ScreenToWorldPoint
        mousePos = cam.ScreenToWorldPoint(pos);


        if (Input.GetMouseButtonDown(1))
        {
            fl.StunGhost();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = sprintSpeed;
        }
        else
            movementSpeed = normalSpeed;
    }

    private void CheckStatus()
    {
        if (isInVulnerable)      // when Player take damage
        {
            immnedTimer -= Time.deltaTime;
            blinkTimer -= Time.deltaTime;

            if (blinkTimer <= 0)     // when blinkTimer <= 0 blink once
            {
                sr.enabled = !sr.enabled;
                blinkTimer = blinkTimerSet;
            }

            if (immnedTimer <= 0)
            {
                sr.enabled = true;
                isInVulnerable = false;
                immnedTimer = immnedTimerSet;
            }
        }

    }

    private void FaceMousePos()
	{
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

    }

    private void ApplyAnimation()
	{
        if(movement.x < 0)
            anim.SetInteger("Direction", 3);
        else if(movement.x > 0)
            anim.SetInteger("Direction", 2);

        if (movement.y < 0)
            anim.SetInteger("Direction", 0);
        else if (movement.y > 0)
            anim.SetInteger("Direction", 1);

        anim.SetBool("IsMoving", movement.magnitude > 0);
    }

    private void ApplyMovement()
	{
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);
	}

    public void TakeDamage(Transform fromObject, bool isFall = false)
	{
		//if (isVulnerable && curNumOfHeart > 0)
        if (!isInVulnerable)                               // FOR TESTING FOR NOW
        {
            Vector2 dir = transform.position - fromObject.position;
            dir = dir.normalized;

            if (isFall)
			{
                isFalling = true;
                rb.AddForce(-dir * 2000);       // suck
                StartCoroutine(Fall());
            }
			else
			{
                rb.AddForce(dir * 5000);        // push
                isInVulnerable = true;          // flickering right away if not fallingS
            }

            
            curNumOfHeart -= 1;
            OnDamaged?.Invoke();
        }

        if(curNumOfHeart <= 0)
		{
            // GAME OVER
		}

	}

    public IEnumerator Fall()
    {
        float duration = 2f;
        float elapsed = 0f;

        movement = Vector2.zero;

        while (elapsed <= duration)
        {
            var idk = Mathf.Lerp(1f, 0f, elapsed / duration);   // shrink the object
            transform.localScale = new Vector3(idk, idk, idk);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(1, 1, 1);

		Respawn();

        isInVulnerable = true;
        isFalling = false;
	}

    private void Respawn()
	{
        transform.position = GameManager.Instance.lastCheckpoint;
	}


    #region INCREASER

    public void GiveHeart()
	{
        curNumOfHeart += 1;
        OnObtainItem?.Invoke();

    }

    public void RefillBattery()
	{
        fl.RefillBattery();
        OnObtainItem?.Invoke();
    }

    public void GiveGem()
	{
        curGem += 1;
        OnObtainItem?.Invoke();
    }

    public void GiveKey()
	{
        obtainedKey = true;
        OnObtainItem?.Invoke();
    }

	#endregion

	#region GET

	public int GetCurNumOfHeart()
	{
        return curNumOfHeart;
	}

    public int GetNumOfHeart()
	{
        return numOfHeart;
	}

    public float GetFlCurrentBattery()
	{
        return fl.GetCurrentBattery();
	}

    public float GetFlFullBattery()
	{
        return fl.GetFullBattery();
	}

    public int GetCurNumOfGem()
	{
        return curGem;
	}

    public bool IsKeyObtained()
	{
        return obtainedKey;
	}

	#endregion
}
