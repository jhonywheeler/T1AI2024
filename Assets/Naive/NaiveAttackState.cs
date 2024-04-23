using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class NaiveAttackState : NaiveFSMState
{
    private PatrolAgentFSM PatrolFSMRef = null;
    // Variable para controlar la posici�n del jugador
    public GameObject agent;

    // Variables para ajustar la distancia y el �ngulo de visi�n
    public float AttackVisionAngleMultiplier;
    public float AttackVisionDistanceMultiplier;
    // Variables para controlar la transici�n a otro estado
    public float TimeToChangeState;
    public float TimeBeforeChangeState = 0f;

    // Constructor que recibe la m�quina de estados y asigna el nombre del estado

    public NaiveAttackState(NaiveFSM FSM)
    {
        Name = "Attack";
        _FSM = FSM;
        PatrolFSMRef = ((PatrolAgentFSM)_FSM);

    }

    // M�todo para inicializar los par�metros de visi�n del estado
    public void Init(float in_AttackVisionAngleMultiplier, float in_AttackVisionDistanceMultiplier)
    {
        AttackVisionAngleMultiplier = in_AttackVisionAngleMultiplier;
        AttackVisionDistanceMultiplier = in_AttackVisionDistanceMultiplier;
    }

    // M�todo que se ejecuta al entrar al estado

    public override void Enter()
    {
        base.Enter();
        // Inicializar el tiempo de transici�n de estado
        TimeToChangeState = 5f;
        // Inicializar la animaci�n al entrar a este estado

        // Buscar el objeto del jugador en la escena
        agent = GameObject.Find("SpaceRacer");
        // Cambiar el color de la luz a rojo
        PatrolFSMRef._light.color = Color.red;
    }

    public override void Update()
    {
        base.Update();
        // Obtener la direcci�n hacia el jugador desde la posici�n del agente
        Vector3 directionToPlayer = agent.transform.position - _FSM.transform.position;
        // Establecer el color de la luz en rojo
        PatrolFSMRef._light.color = Color.red;
        // Establecer el destino del NavMesh hacia la posici�n del jugador
        PatrolFSMRef._NavMeshAgent.SetDestination(agent.transform.position);

        // Contar el tiempo restante para cambiar de estado
        TimeToChangeState -= Time.deltaTime;

        // Si se agota el tiempo, cambiar al estado de alerta
        if (TimeToChangeState <= TimeBeforeChangeState)
        {
            NaivePatrolState AlertStateInstance = PatrolFSMRef.PatrolStateRef;
            _FSM.ChangeState(AlertStateInstance);
            return;
        }

        // Si la distancia al jugador es menor que 1 unidad, cambiar al estado de patrulla y destruir al jugador
        if (directionToPlayer.magnitude < 1.0f)
        {
            NaivePatrolState PatrolStateInstance = PatrolFSMRef.PatrolStateRef;
            _FSM.ChangeState(PatrolStateInstance);
            DestroyPlayer();
            return;
        }

    }

    public override void Exit()
    {
        base.Exit();

    }

    private void DestroyPlayer()
    {
        agent.SetActive(false);
        Debug.Log("El jugador ha sido destruido.");
    }
}