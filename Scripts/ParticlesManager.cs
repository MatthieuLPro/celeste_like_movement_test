using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesManager : MonoBehaviour
{
    public ParticleSystem jumpParticle;
    public ParticleSystem slideParticle;

    [Space]
    [Header("Data")]
    [SerializeField]
    private CharacterData data;

    
    [HideInInspector]
    private AnimationScript anim;
    private Collision coll;

    // Start is called before the first frame update
    void Start()
    {
        anim = transform.parent.GetComponentInChildren<AnimationScript>();
        coll = GetComponentInParent<Collision>();
    }

    public void GroundTouch() {
        data.hasDashed = false;
        data.isDashing = false;

        data.side = anim.sr.flipX ? -1 : 1;

        jumpParticle.Play();
    }

    public void WallParticle(float vertical)
    {
        var main = slideParticle.main;

        if (data.wallSlide || (data.wallGrab && vertical < 0))
        {
            slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
            main.startColor = Color.white;
        }
        else
        {
            main.startColor = Color.clear;
        }
    }

    public int ParticleSide()
    {
        int particleSide = coll.onRightWall ? 1 : -1;
        return particleSide;
    }

    public void UpdateSlideParticleLocalScale() {
        slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
    }
}
