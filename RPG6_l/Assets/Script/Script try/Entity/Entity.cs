using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public event Action OnFlipped;

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Entity_Stats stats { get; private set; }
    protected StateMachine stateMachine;

    private bool facingRight = true;
    public int facingDir { get; private set; } = 1;
    // When true the entity should not take damage (used e.g. for player dash invulnerability)
    public bool IsInvulnerable { get; private set; } = false;

    public void SetInvulnerable(bool state)
    {
        IsInvulnerable = state;
    }
  

    [Header("检测参数")]
    [SerializeField] private float groundCheckDistance;//检测地面距离
    [SerializeField] private float wallCheckDistance;//检测墙壁距离
    [SerializeField] private Transform groundCheck;//检测地面点
    [SerializeField] private Transform primaryWallCheck;//检测墙壁点1
    [SerializeField] private Transform secondaryWallCheck;//检测墙壁点2
    [SerializeField] protected LayerMask whatIsGround;//地面图层
    public bool groundDetected { get; private set; }//是否检测到地面
    public bool wallDetected { get; private set; }//是否检测到墙壁

    private bool isKnocked;//是否处于击退状态
    private Coroutine knockbackCo;//击退协程
    private Coroutine slowDownCo;//减速协程

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<Entity_Stats>();

        stateMachine = new StateMachine();
        
    }

    

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }

    public virtual void EntityDeath()
    {

    }

    public virtual  void SlowDownEntity(float slowMultiplier, float duration)//减速实体
    {
        if(slowDownCo != null)
            StopCoroutine(slowDownCo);

        slowDownCo = StartCoroutine(SlowDownEntityCo(slowMultiplier, duration));
    }


    protected virtual IEnumerator SlowDownEntityCo(float slowMultiplier, float duration)//减速实体协程(减速实体的子程序)
    {
        yield return null;
    }

    public void ReciveKnockback(Vector2 knockback, float duration)//接收击退
    {
        if(knockbackCo != null)
            StopCoroutine(knockbackCo);

        knockbackCo = StartCoroutine(KnockbackCo(knockback, duration));
    }


    private IEnumerator KnockbackCo(Vector2 knockback,float duration)//击退协程(击退的子程序)
    {
        isKnocked = true;
        rb.linearVelocity = knockback;

        yield return new WaitForSeconds(duration);

        rb.linearVelocity = Vector2.zero;
        isKnocked = false;
    }


    public void CurrentStateAnimationTrigger()//当前状态动画触发器
    {
        stateMachine.currentState.AnimationTrigger();
    }

    public void SetVelocity(float xVelocity, float yVelocity)//设置速度
    {
        if (isKnocked)
            return;

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    public void HandleFlip(float xVelocity)//处理翻转
    {
        if (xVelocity > 0 && !facingRight)
        {
            Flip();
        }
        if (xVelocity < 0 && facingRight)
        {
            Flip();
        }
    }


    public void Flip()//翻转实体
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDir *= -1;

        OnFlipped?.Invoke();
    }

    private void HandleCollisionDetection()//处理碰撞检测
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        if (secondaryWallCheck != null)
        {
            wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround)
                && Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        }
        else
            wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    protected virtual void OnDrawGizmos()//绘制地面，墙壁检测线
    {
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawLine(primaryWallCheck.position, primaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));
        if(secondaryWallCheck != null)
            Gizmos.DrawLine(secondaryWallCheck.position, secondaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));
    }
}
