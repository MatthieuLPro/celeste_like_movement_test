using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions {
    public class Walk : MonoBehaviour
    {
        [HideInInspector]
        public Rigidbody2D rb;
        private Animator anim;

        [Space]
        [Header("Data")]
        [SerializeField]
        private CharacterData data;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponentInParent<Rigidbody2D>();
            anim = transform.parent.GetComponentInChildren<Animator>();
        }

        public void Move(Vector2 dir)
        {
            if (!data.canMove)
                return;

            if (data.wallGrab)
                return;

            if (!data.wallJumped)
            {
                rb.velocity = new Vector2(dir.x * data.speed, rb.velocity.y);
            }
            else
            {
                rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * data.speed, rb.velocity.y)), data.wallJumpLerp * Time.deltaTime);
            }
        }

        public void SetAnimation(float x,float y, float yVel)
        {
            anim.SetFloat("HorizontalAxis", x);
            anim.SetFloat("VerticalAxis", y);
            anim.SetFloat("VerticalVelocity", yVel);
        }
    }
}
