using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Actions {
    public class Dash : MonoBehaviour
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
        public ParticleSystem dashParticle;
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponentInParent<Rigidbody2D>();
            anim = transform.parent.GetComponentInChildren<Animator>();
            coll = GetComponentInParent<Collision>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void Call(float x, float y) {
            data.hasDashed = true;
            rb.velocity = Vector2.zero;
            Vector2 dir = new Vector2(x, y);

            rb.velocity += dir.normalized * data.dashSpeed;
            StartCoroutine(DashWait());
        }

        public void DashEffect() {
            Camera.main.transform.DOComplete();
            Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
            FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
        }

        public void SetAnimation()
        {
            anim.SetTrigger("dash");
        }

        IEnumerator DashWait()
        {
            FindObjectOfType<GhostTrail>().ShowGhost();
            StartCoroutine(GroundDash());
            DOVirtual.Float(14, 0, .8f, RigidbodyDrag);

            dashParticle.Play();
            rb.gravityScale = 0;
            transform.parent.GetComponentInChildren<BetterJumping>().enabled = false;
            data.wallJumped = true;
            data.isDashing = true;

            yield return new WaitForSeconds(.3f);

            dashParticle.Stop();
            rb.gravityScale = 3;
            transform.parent.GetComponentInChildren<BetterJumping>().enabled = true;
            data.wallJumped = false;
            data.isDashing = false;
        }

        IEnumerator GroundDash()
        {
            yield return new WaitForSeconds(.15f);
            if (coll.onGround)
                data.hasDashed = false;
        }

        void RigidbodyDrag(float x)
        {
            rb.drag = x;
        }
    }
}
