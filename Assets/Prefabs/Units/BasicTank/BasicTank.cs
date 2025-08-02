using UnityEngine;

public class BasicTank : Unit
{
    [SerializeField] ParticleSystem explosionParticles;

    protected override void DestroyUnitAnim()
    {
        base.DestroyUnitAnim();

        explosionParticles.Play();

        GameObject.Destroy(gameObject);
    }
}
