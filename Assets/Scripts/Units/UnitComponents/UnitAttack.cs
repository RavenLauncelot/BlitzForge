using System.Collections;
using UnityEngine;

public class UnitAttack : UnitComponent
{
    public Unit commandTarget;
    public Unit autofireTarget;
    public Unit activeTarget;
    public Unit Unit;

    public float range;
    public float firerate;
    public float damage;

    public bool fireAtWill;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        componentType = UnitComponents.UnitAttack;
        Unit = GetComponent<Unit>();

        StartCoroutine(FireCoroutine());
    }

    public void SetAttackTarget(Unit target)
    {
        commandTarget = target;
    }

    public void Update()
    {
        //no command has been assigned auto
        if (commandTarget == null)
        {
            activeTarget = autofireTarget;
            AutofireTargetFinder();
        }

        else
        {
            activeTarget = commandTarget;
        }
    }

    public void AutofireTargetFinder()
    {
        //find new target autofireTarget is null
        if (autofireTarget == null)
        {
            //Unit found 
            //This may be bad for performannce checking every frame, especially when no untis have been discovered yet.
            if (Unit.unitManager.FindClosestEnemy(transform.position, out Unit foundTarget))
            {
                //within range
                if (Vector3.Distance(foundTarget.transform.position, transform.position) < range)
                {
                    autofireTarget = foundTarget;
                }

                //out of range
                else
                {
                    autofireTarget = null;
                }
            }
        }

        //check auto fire target is still valid (in range)
        else
        {
            //checks if within range
            if(Vector3.Distance(autofireTarget.transform.position, transform.position) > range)
            {
                autofireTarget = null;
            }
        }
    }

    public void Fire(Unit target)
    {
        Debug.Log("Fired");
    }

    public IEnumerator FireCoroutine()
    {
        while (true)
        {
            if (activeTarget != null)
            {
                if (fireAtWill & checkTarget(activeTarget))
                {
                    Fire(activeTarget);
                }
            }

            yield return new WaitForSeconds(firerate);
        }
    }

    public bool checkTarget(Unit target)
    {
        //check range

        if (Vector3.Distance(target.transform.position, transform.position) < range)
        {
            //check line of sight
            RaycastHit hit;
            Ray ray = new Ray(transform.position, target.transform.position - transform.position);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.TryGetComponent<Unit>(out Unit hitUnit))
                {
                    Debug.Log("Within line of sight");
                    return true;
                }

                Debug.Log("TargetBlocked collider name: " + hit.collider.name);
            }

            else
            {
                Debug.Log("Ray Didnt hit");
            }

            Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.magenta, 1);
        }

        else
        {
            Debug.Log("Out of range");
        }

        return false;
    }
}
