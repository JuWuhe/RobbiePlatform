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

    [Header("角色状态")] 
    public bool isCrouch;
    
    private float xVelocity;

    /// <summary>
    /// 碰撞体尺寸
    /// </summary>
    /// <returns></returns>
    private Vector2 StandSize;
    private Vector2 StandOffest;
    private Vector2 CrouchSize;
    private Vector2 CrouchOffest;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxColl = GetComponent<BoxCollider2D>();

        StandOffest = boxColl.offset;
        StandSize = boxColl.size;
        CrouchSize = new Vector2(boxColl.size.x,boxColl.size.y / 2f);
        CrouchOffest = new Vector2(boxColl.offset.x, boxColl.offset.y / 2f);
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        GroundMovement();
    }

    void GroundMovement()
    {
        if(Input.GetButton("Crouch")) 
            Crouch();
        else if (!Input.GetButton("Crouch") && isCrouch)
            StanUp();
        
        xVelocity = Input.GetAxis("Horizontal"); // -1f ~ 1f 当键盘无输入时为0

        if (isCrouch)
            xVelocity /= crouchSpeedDivisor;
        
        rb.velocity = new Vector2(xVelocity * speed,rb.velocity.y);
        
        FlipDirction();
    }

    void FlipDirction()
    {
        if(xVelocity < 0) 
            transform.localScale = new Vector2(-1,1); //向左翻转
        if (xVelocity > 0)
            transform.localScale = new Vector2(1, 1); //向右翻转
    }

    void Crouch()
    {
        isCrouch = true;
        boxColl.size = CrouchSize;
        boxColl.offset = CrouchOffest;
    }

    void StanUp()
    {
        isCrouch = false;
        boxColl.size = StandSize;
        boxColl.offset = StandOffest;
    }
}
