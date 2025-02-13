using UnityEngine;
using UnityEngine.AI;

public class DebugInfantry : Unit
{
    public float setSpeed;
    public float setHealth;

    public Unit.teamName setTeam;

    public bool setCanAttack;
    public bool setCanMove;

    public UnitManager setManager;

    public Vector3 setPos;

    public Vector3 previousPos; //this is just so it can check if the command changed

    public NavMeshAgent agent;

    private void Awake()
    {
        manager = setManager;


        activeCMD = new CommandTypes(CommandTypes.command.move, Vector3.zero);
    }

    private void Start()
    {
        speed = setSpeed;
        health = setHealth;
        team = setTeam;

        canAttack = setCanAttack;
        canMove = setCanMove;

    }

    private void Update()
    {

    }
}
