using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxColl;

    [Header("移动参数")] 
    public float speed = 8f;
    public float crouchSpeedDivisor = 3f;

    [Header("跳跃参数")] 
    public float jumpForce = 6.3f;
    public float jumpHoldForce = 1.9f;
    public float jumpHoldDuration = 0.1f;
    public float crouchJumpBoost = 2.5f;

    private float jumpTime;
    
    [Header("角色状态")] 
    public bool isCrouch;
    public bool isOnGround;
    public bool isJump;
    
    [Header("环境检测")] 
    public LayerMask groundLayer;
    
    private float xVelocity;

    //按键设置
    private bool jumpPressed;
    private bool jumpHeld;
    private bool crouchHeld;

    //站立碰撞体尺寸
    private Vector2 StandSize;
    private Vector2 StandOffest;
    //下蹲碰撞体尺寸
    private Vector2 CrouchSize;
    private Vector2 CrouchOffest;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxColl = GetComponent<BoxCollider2D>();

        //赋值碰撞体大小
        StandOffest = boxColl.offset;
        StandSize = boxColl.size;
        CrouchSize = new Vector2(boxColl.size.x,boxColl.size.y / 2f);
        CrouchOffest = new Vector2(boxColl.offset.x, boxColl.offset.y / 2f);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
            jumpPressed = true;
        jumpHeld = Input.GetButton("Jump");
        crouchHeld = Input.GetButton("Crouch"); //"Crouch"在ProjectSettings创建
    }

    private void FixedUpdate()
    {
        PhysicsCheck();
        GroundMovement();
        MidAirMovement();
    }

    /// <summary>
    /// 物理状态检测
    /// </summary>
    void PhysicsCheck()
    {
        if (boxColl.IsTouchingLayers(groundLayer))
            isOnGround = true;
        else 
            isOnGround = false;
    }
    
    /// <summary>
    /// 角色移动
    /// <para>朝向、下蹲站立、更新移速</para>
    /// </summary>
    void GroundMovement()
    {
        //下蹲站立
        if(crouchHeld && !isCrouch && isOnGround)
            Crouch();
        else if (!crouchHeld && isCrouch)
            StandUp();
        else if (!isOnGround && isCrouch)
            StandUp();
        //判断朝向
        xVelocity = Input.GetAxis("Horizontal"); // -1f ~ 1f 当键盘无输入时为 0
        FlipDirction(); 
        
        //更新速度
        if (isCrouch)
            xVelocity /= crouchSpeedDivisor;
        
        rb.velocity = new Vector2(xVelocity * speed,rb.velocity.y);
        
    }

    /// <summary>
    /// 跳跃
    /// </summary>
    void MidAirMovement()
    {
        if (jumpPressed && isOnGround && !isJump)
        {
            //下蹲跳跃判断
            if (isCrouch && isOnGround)
            {
                StandUp();
                rb.AddForce(new Vector2(0f,crouchJumpBoost),ForceMode2D.Impulse);
            }
            
            //正常跳跃
            isOnGround = false;
            isJump = true;
            jumpPressed = false;

            jumpTime = Time.time + jumpHoldDuration;
            
            rb.AddForce(new Vector2(0f,jumpForce),ForceMode2D.Impulse);
        }
        else if (isJump)
        {
            //长按跳跃
            if(jumpHeld)
                rb.AddForce(new Vector2(0f,jumpHoldForce),ForceMode2D.Impulse);
            if (jumpTime < Time.time)
                isJump = false;
        }
    }
    
    /// <summary>
    /// 转向
    /// </summary>
    void FlipDirction()
    {
        if(xVelocity < 0) 
            transform.localScale = new Vector2(-1,1); //向左翻转
        if (xVelocity > 0)
            transform.localScale = new Vector2(1, 1); //向右翻转
    }
    
    /// <summary>
    /// 下蹲
    /// </summary>
    void Crouch()
    {
        isCrouch = true;
        //缩小碰撞体
        boxColl.size = CrouchSize;
        boxColl.offset = CrouchOffest;
    }

    /// <summary>
    /// 站立
    /// </summary>
    void StandUp()
    {
        isCrouch = false;
        //复原碰撞体
        boxColl.size = StandSize;
        boxColl.offset = StandOffest;
    }
}
