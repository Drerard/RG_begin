using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private enum CharState
    {
        Idle,
        Run,
        Attack,
        Die
    }

    #region FIELDS
    [SerializeField] private float moveSpeed = 10;
    [Space(5)]
    [SerializeField] private float DashForce = 15;
    [SerializeField] private float DashCD = 1;
    [SerializeField] private float DashDuration = 1;
    [Space(5)]
    [SerializeField] public PlayerLayerData LayerData;
    [Header("Sound Effects")]
    [SerializeField] public AudioSource attackSound;
    [SerializeField] public AudioSource recieveDmgSound;
    [SerializeField] public AudioSource upCoinSound;
    [SerializeField] public AudioSource upKeySound;



    private float timerNormalAttackCD = 0;
    private float timerDashCD = 0;
    private bool isDashing = false;
    private Vector2 lastDirection = Vector2.right;

    [HideInInspector] public PlayerInventory inventory;
    private PlayerCombatSystem playerCombatSystem;
    private Animator animator;
    private Rigidbody2D rb;


    private CharState State
    {
        get { return (CharState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }
    #endregion

    #region MONO
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        inventory = GetComponent<PlayerInventory>();
        playerCombatSystem = GetComponent<PlayerCombatSystem>();
    }

    private void Update()
    {
        if (Input.GetButtonDown(InputButtonData.Cancel))
        {
            KeyInputEventManager.StartEscapePressedEvent();
        }

        if (playerCombatSystem.IsDead())
        {
            State = CharState.Die;
            StopMove();
            return;
        }

        if (timerNormalAttackCD <= 0)
        {
            if (Input.GetButtonDown(InputButtonData.Fire1))
            {
                State = CharState.Attack;
                attackSound.Play();
                timerNormalAttackCD = playerCombatSystem.GetNormalAttackCD();
                playerCombatSystem.NormalAttack();
            }
        }
        else
        {
            timerNormalAttackCD -= Time.deltaTime;
        }

        if (timerDashCD <= 0)
        {
            if (Input.GetButtonDown(InputButtonData.Fire2))
            {
                isDashing = true;
                playerCombatSystem.Dash(DashDuration);
                timerDashCD = DashCD;
                StartCoroutine(StopDash(DashDuration));
            }
        }
        else
        {
            timerDashCD -= Time.deltaTime;
        }
    }
    IEnumerator StopDash(float duration)
    {
        yield return new WaitForSeconds(duration);
        isDashing = false;
        StopMove();
    }
    private void FixedUpdate()
    {
        if (playerCombatSystem.IsDead())
        {
            State = CharState.Die;
            StopMove();
            return;
        }

        if (isDashing)
        {
            Dash();
        }
        else
        {
            if (Input.GetButton(InputButtonData.Horizontal) || Input.GetButton(InputButtonData.Vertical))
            {
                State = CharState.Run;
                Move();
            }
            else
            {
                State = CharState.Idle;
                StopMove();
            }
        }

        if (Input.GetButton(InputButtonData.PickUp))
        {
            PickUp();
        }
    }
    #endregion

    #region MAIN
    private void PickUp()
    {
        KeyInputEventManager.StartPickUpEvent(GetComponent<Player>());
        KeyInputEventManager.StartInteractionEvent();
    }

    private void Move()
    {
        float horizontal = Input.GetAxis(InputButtonData.Horizontal);
        float vertical = Input.GetAxis(InputButtonData.Vertical);
        Vector2 direction = new Vector2(horizontal, vertical);
        transform.eulerAngles = horizontal >= 0 ? Vector3.zero : new Vector3(0, 180, 0);

        rb.velocity = moveSpeed * direction.normalized;

        lastDirection = SoftAroundToInt(horizontal, vertical);
    }

    private void Dash()
    {
        rb.velocity = DashForce * lastDirection.normalized;
    }

    private void StopMove()
    {
        rb.velocity = Vector2.zero;
    }
    #endregion

    private Vector2 SoftAroundToInt(float horizontal, float vertical)
    {
        Vector2 vector = new Vector2();
        if (horizontal != 0)
            vector.x = horizontal > 0 ? 1 : -1;
        if (vertical != 0)
            vector.y = vertical > 0 ? 1 : -1;

        return vector;
    }
}
