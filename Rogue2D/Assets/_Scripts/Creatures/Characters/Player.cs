using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private enum CharacterState
    {
        Idle,
        Run,
        Attack,
        Die
    }

    #region FIELDS
    [SerializeField] private float moveSpeed = 10;
    [Space(5)]
    [SerializeField] private float dashForce = 15;
    [SerializeField] private float dashCD = 1;
    [SerializeField] private float dashDuration = 1;
    [Space(5)]
    [SerializeField] public PlayerLayerData LayerData;
    [Header("Sound Effects")]
    [SerializeField] public AudioSource attackSound;
    [SerializeField] public AudioSource recieveDmgSound;
    [SerializeField] public AudioSource upCoinSound;
    [SerializeField] public AudioSource upKeySound;



    private float timerNormalAttackCD = 0;
    [SerializeField] private float timerDashCD = 0;
    private bool isDashing = false;
    private Vector2 lastDirection = Vector2.right;

    [HideInInspector] public PlayerInventory inventory;
    private PlayerCombatSystem playerCombatSystem;
    private Animator animator;
    private Rigidbody2D rb;


    private CharacterState State
    {
        get { return (CharacterState)animator.GetInteger("State"); }
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
            State = CharacterState.Die;
            StopMove();
            return;
        }

        if (timerNormalAttackCD <= 0)
        {
            if (Input.GetButtonDown(InputButtonData.Fire1))
            {
                State = CharacterState.Attack;
                attackSound.Play();
                timerNormalAttackCD = playerCombatSystem.GetNormalAttackCD();
                playerCombatSystem.NormalAttack();
            }
        }
        else
        {
            timerNormalAttackCD -= Time.deltaTime;
        }

        if (timerDashCD <= 0 && !isDashing)
        {
            if (Input.GetButtonDown(InputButtonData.Fire2))
            {
                isDashing = true;
                playerCombatSystem.Dash(dashDuration);
                timerDashCD = dashCD;
                StartCoroutine(StopDash(dashDuration));
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
            State = CharacterState.Die;
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
                State = CharacterState.Run;
                Move();
            }
            else
            {
                State = CharacterState.Idle;
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
        rb.velocity = dashForce * lastDirection.normalized;
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
