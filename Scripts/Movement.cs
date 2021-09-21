using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Movement : MonoBehaviour
{
    private Collision coll;
    [HideInInspector]
    public Rigidbody2D rb;
    private AnimationScript anim;
    private Actions.Walk walkAction;
    private Actions.Jump jumpAction;
    private Actions.Dash dashAction;

    [Space]
    [Header("Data")]
    [SerializeField]
    private CharacterData data;

    [Space]
    private bool groundTouch;

    private ParticlesManager particles;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<AnimationScript>();

        walkAction = GetComponentInChildren<Actions.Walk>();
        jumpAction = GetComponentInChildren<Actions.Jump>();
        dashAction = GetComponentInChildren<Actions.Dash>();

        particles = GetComponentInChildren<ParticlesManager>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        CallWalkAction(dir, x, y, rb.velocity.y);

        if (coll.onWall && Input.GetButton("Fire3") && data.canMove)
        {
            if(data.side != coll.wallSide)
                anim.Flip(data.side*-1);
            data.wallGrab = true;
            data.wallSlide = false;
        }

        if (Input.GetButtonUp("Fire3") || !coll.onWall || !data.canMove)
        {
            data.wallGrab = false;
            data.wallSlide = false;
        }

        if (coll.onGround && !data.isDashing)
        {
            jumpAction.EnableBetterJumping();
        }
        
        if (data.wallGrab && !data.isDashing)
        {
            rb.gravityScale = 0;
            if(x > .2f || x < -.2f)
            rb.velocity = new Vector2(rb.velocity.x, 0);

            float speedModifier = y > 0 ? .5f : 1;

            rb.velocity = new Vector2(rb.velocity.x, y * (data.speed * speedModifier));
        }
        else
        {
            rb.gravityScale = 3;
        }

        if(coll.onWall && !coll.onGround)
        {
            if (x != 0 && !data.wallGrab)
            {
                data.wallSlide = true;
                wallSlide();
            }
        }

        if (!coll.onWall || coll.onGround)
            data.wallSlide = false;

        if (Input.GetButtonDown("Jump"))
        {
            CallJumpAction();
        }

        if (Input.GetButtonDown("Fire1") && !data.hasDashed)
        {
            if(xRaw != 0 || yRaw != 0) {
                CallDashAction(xRaw, yRaw);
            }
        }

        UpdateGroundTouch();

        particles.WallParticle(y);

        if (data.wallGrab || data.wallSlide || !data.canMove)
            return;

        UpdateDirection(x);
    }

    // Check and update the character direction
    private void UpdateDirection(float x) {
        if(x > 0)
        {
            data.side = 1;
            anim.Flip(data.side);
        }
        if (x < 0)
        {
            data.side = -1;
            anim.Flip(data.side);
        }
    }

    // Detect and update if the character touche the ground
    private void UpdateGroundTouch() {
        if (coll.onGround && !groundTouch)
        {
            particles.GroundTouch();
            groundTouch = true;
        }

        if(!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }
    }

    private void CallWalkAction(Vector2 dir, float x, float y, float yVelocity) {
        walkAction.Move(dir);
        walkAction.SetAnimation(x, y, yVelocity);
    }
    
    private void CallDashAction(float x, float y) {
        dashAction.DashEffect();
        dashAction.SetAnimation();
        dashAction.Call(x, y);
    }

    private void CallJumpAction() {
        jumpAction.SetAnimation();
        if (OnTheGround())
            particles.UpdateSlideParticleLocalScale();
            jumpAction.Call(Vector2.up, false);
        if (OnAWall()) {
            if ((data.side == 1 && coll.onRightWall) || data.side == -1 && !coll.onRightWall)
            {
                data.side *= -1;
                anim.Flip(data.side);
            }

            StopCoroutine(DisableMovement(0));
            StartCoroutine(DisableMovement(.1f));

            jumpAction.CallWall();
        }
    }

    private bool OnTheGround() {
        return coll.onGround;
    }

    private bool OnAWall() {
        return coll.onWall && !coll.onGround;
    }


    private void wallSlide()
    {
        if(coll.wallSide != data.side)
         anim.Flip(data.side * -1);

        if (!data.canMove)
            return;

        bool pushingWall = false;
        if((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -data.slideSpeed);
    }

    IEnumerator DisableMovement(float time)
    {
        data.canMove = false;
        yield return new WaitForSeconds(time);
        data.canMove = true;
    }
}
