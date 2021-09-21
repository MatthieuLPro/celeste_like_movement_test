using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions {
    public class Jump : MonoBehaviour
    {
        
        [HideInInspector]
        public Rigidbody2D rb;
        private Animator anim;
        private Collision coll;
        
        [Space]
        [Header("Data")]
        [SerializeField]
        private CharacterData data;
        
        [Space]
        [Header("Polish")]
        public ParticleSystem jumpParticle;
        public ParticleSystem wallJumpParticle;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponentInParent<Rigidbody2D>();
            anim = transform.parent.GetComponentInChildren<Animator>();
            coll = GetComponentInParent<Collision>();
        }

        public void EnableBetterJumping() {
            data.wallJumped = false;
            GetComponent<BetterJumping>().enabled = true;
        }

        public void SetAnimation()
        {
            anim.SetTrigger("jump");
        }

        public void Call(Vector2 dir, bool wall)
        {
            ParticleSystem particle = wall ? wallJumpParticle : jumpParticle;

            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity += dir * data.jumpForce;

            particle.Play();
        }

        public void CallWall()
        {
            Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

            Call((Vector2.up / 1.5f + wallDir / 1.5f), true);

            data.wallJumped = true;
        }
    }
}