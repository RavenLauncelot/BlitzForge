using UnityEngine;

public class BasicTank : Unit
{
    [SerializeField] ParticleSystem explosionParticles;

    protected override void DestroyUnitAnim()
    {
        base.DestroyUnitAnim();

        Instantiate(explosionParticles, transform.position, Quaternion.identity);

        GameObject.Destroy(gameObject);
    }
}


