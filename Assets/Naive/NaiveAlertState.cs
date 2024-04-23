using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class NaiveAlertState : NaiveFSMState
{
    private enum AlertSubState
    {
        Stopped = 1,
        GoingToCheck = 2,
        ReturningToPosition = 3,
        MAX = 4
    }

    private AlertSubState _currentSubState = AlertSubState.Stopped;

    private PatrolAgentFSM PatrolFSMRef = null;
    public bool Moviendose = false; //Ayuda a controlar los estados de Alerta 

    public float VisionDistance;
    public float VisionAngle;

    private float TimeDetectingPlayerBeforeEnteringAttack = 2f;
    private float AccumulatedTimeDetectingPlayerBeforeEnteringAttack;

    //  cierto tiempo de la �ltima vez que detect� al jugador, es decir, en qu� momento en el tiempo se detect�.
    private float LastTimePlayerSeen = 0;
    // Cu�nto tiempo debe pasar entre la �ltima vez que vio al jugador, para decir: ya toca ir a la �ltima posici�n conocida.
    private float TimeSinceLastSeenTreshold = 2;
    // Espec�ficamente para este caso, en vez de un AccumulatedTime, vamos a usar :
    // la diferencia entre LastTimePlayerSeen y Time.realtimeSinceStartup.

    // la necesitamos porque el NavMesh no nos avisa directamente cuando ya acab�.
    private float DistanceToGoalTolerance = 1.0f;

    // NOTA, ESTA VARIABLE LA MOV� A LA FSM, YA QUE LA PODR�AN  USAR OTROS ESTADOS, COMO EL DE ATAQUE.
    // SE ACTUALIZA A TRAV�S DE LA FUNCI�N CheckFieldOfView DIRECTAMENTE.
    // la �ltima posici�n conocida del sospechoso
    // Si nos ponemos elaborados, hasta podr�amos darle un margen de error
    //private Vector3 LastKnownPlayerLocation = Vector3.zero;

    // A una cierta velocidad de movimiento
    private float SpeedWhileCheckingLastKnownLocation = 5.0f;

    // el agente se regresar� hacia el punto de patrullaje
    // Este ya lo tenemos, a trav�s de la FSM.

    public NaiveAlertState(NaiveFSM FSM)
    {
        Name = "Alert"; // el nombre por el cual conocemos a este estado.
        _FSM = FSM;  // la m�quina de estados que es due�a de este estado
        PatrolFSMRef = ((PatrolAgentFSM)_FSM);
    }

    public void Init(float in_VisionDistance, float in_VisionAngle, float in_TimeSinceLastSeenTreshold,
        float in_SpeedWhileCheckingLastKnownLocation)
    {
        VisionAngle = in_VisionAngle;
        VisionDistance = in_VisionDistance;
        TimeSinceLastSeenTreshold = in_TimeSinceLastSeenTreshold;
        SpeedWhileCheckingLastKnownLocation = in_SpeedWhileCheckingLastKnownLocation;
    }


    public override void Enter()
    {
        base.Enter();
        LastTimePlayerSeen = Time.realtimeSinceStartup;
        // Tal vez Time.time es una mejor opci�n que: Time.realtimeSinceStartup
        AccumulatedTimeDetectingPlayerBeforeEnteringAttack = 0.0f;
        _currentSubState = AlertSubState.Stopped; // Iniciamos en el subestado Stopped siempre.
    }

    // Update is called once per frame
    public override void Update()
    {
        // base.Update();
        // Time.realtimeSinceStartup

        if (_currentSubState == AlertSubState.Stopped)
        {
            // Aqu� tendr�amos la parte que ya conocemos sobre pasar al estado de Ataque
            // Si s� estamos viendo al jugador, hacemos lo siguiente:
            // tenemos que estar en el subestado Stopped para seguir seguir acumulando este tiempo.
            AccumulatedTimeDetectingPlayerBeforeEnteringAttack += Time.deltaTime;
            if (AccumulatedTimeDetectingPlayerBeforeEnteringAttack > TimeDetectingPlayerBeforeEnteringAttack)
            {
                //Quitamos este codigo de comentarios jejeje
                PatrolAgentFSM SpecificFSM = (PatrolAgentFSM)_FSM;
                NaiveAttackState AttackStateInstance = SpecificFSM.AttackStateRef;
                _FSM.ChangeState(AttackStateInstance);
                return;
            }

            if (LastTimePlayerSeen == -1)
            {
                // entonces lo acabamos de ver, (si no no estar�amos entrando a este subestado).
                LastTimePlayerSeen = Time.realtimeSinceStartup;
            }
            else
            {
                // Si no estamos haciendo cosas para pasar al estado de ataque, 
                // entonces, por nuestro dise�o, estar�amos haciendo cosas que nos devuelvan al estado de Patrullaje.
                float TranscurredTime = Time.realtimeSinceStartup - LastTimePlayerSeen;

                // Si despu�s de un cierto tiempo de la �ltima vez que detect� al jugador ya no lo ha visto.
                if (TimeSinceLastSeenTreshold < TranscurredTime)
                {
                    // seteamos LastTimePlayerSeen en -1, para que sepamos que no es v�lido ahorita.
                    LastTimePlayerSeen = -1;
                    _currentSubState = AlertSubState.GoingToCheck;
                    // Entonces, vamos a la �ltima posici�n conocida del sospechoso. (A una cierta velocidad de movimiento
                    // Le ponemos al NavMesh que su destination es la �ltima posici�n  conocida del sopechoso
                    /* Recordatorio, tenemos que castearla al tipo espec�fico de FSM que es, para acceder
                    acceder a las variables de dicho tipo espec�fico. Pero eso ya lo hicimos y lo guardamos en 
                    PatrolFSMRef. */
                    PatrolFSMRef._NavMeshAgent.SetDestination(PatrolFSMRef.LastKnownPlayerPosition);
                    // A lo que tendr�an que estar atentos es al mensaje/trigger que el NavMesh manda al llegar a su
                    // destionation
                    // Si no hay tal mensaje, se podr�a hacer como se muestra aqu�:
                    // https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-destination.html
                    // Sino, checar aqu�:
                    // https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent.html
                    return;
                }
            }
        }
        if (_currentSubState == AlertSubState.GoingToCheck)
        {
            // Si vemos otra vez al jugador, inmediatamente pasamos al estado de Stopped.
            // if( detected)
            // Then: _currentSubState = AlertSubState.Stopped;
            // Reinicializar las variables o valores necesarios.
            // por consistencia, si cambiamos de subestados, tambi�n vamos a llamar return, como si 
            // fueran estados grandes y no subestados.

            // Tenemos que checar si ya llegamos a la posici�n deseada (que es la �ltima posic��n conocida)
            // Le damos rango de tolerancia a esta distancia entre nuestra posici�n y la �ltima posici�n conocida
            float dist = Vector3.Distance(_FSM.transform.position, PatrolFSMRef.LastKnownPlayerPosition);
            if (dist < DistanceToGoalTolerance)  // DE HECHO, no ser�a necesario este Tolerance, porque el NavMesh ya tiene un valor para esto.
            {
                // Entonces ya estamos lo suficientemente cerca.
                // Entonces ya nos podemos empezar a regresar a la InitialPatrolPosition
                // Le pondr�amos al NavMesh que su "destination" es esa initial Patrol position.
                PatrolFSMRef._NavMeshAgent.SetDestination(PatrolFSMRef.InitialPatrolPosition);
                _currentSubState = AlertSubState.ReturningToPosition;
                return;
            }
        }
        if (_currentSubState == AlertSubState.ReturningToPosition)
        {
            // Seguir checando a ver si de camino a la posici�n inicial vemos al infiltrador.
            // Si s� lo vemos, nos vamos al estado de stopped
            // y actualizamos el valor de LastKnownLocation

            
            if (Vector3.Distance(_FSM.transform.position, PatrolFSMRef.InitialPatrolPosition) < DistanceToGoalTolerance)
            {
                // Si no, nada m�s tenemos que checar que lleguemos a la posici�n inicial de patrullaje
                // y cuando lo hagamos, pasamos al estado de patrullaje.
                NaivePatrolState PatrolStateInstance = PatrolFSMRef.PatrolStateRef;
                _FSM.ChangeState(PatrolStateInstance);
                return;
            }
        }

        // Esto de ir a la �ltima posici�n conocida es como un "sub-estado" del estado de alerta.
        // Digamos: Subestado de IrAChecar.
        // Porque si no pasa nada, te vas al estado de patrullaje, 
        // Pero si s� ves al jugador mientras est�s en IrAChecar, te quedas quieto y esperas a ver
        // si tienes que volver a IrAChecar o si pasas al estado de Ataque.


        //
        // Si al llegar a esa �ltima posici�n conocida seguimos sin detectar al jugador,
        // entonces el agente se regresar� hacia el punto de patrullaje.
        // Si llega al punto de patrullaje y sigue sin detectar al jugador, 
        // entonces pasar� al estado de patrullaje.

    }

    public override void Exit()
    {
        base.Exit();
    }

    private bool CheckFOV()
    {
        // �nicamente mandamos a llamar la funci�n de la FSM pero con nuestros par�metros espec�ficos de este estado.
        return PatrolFSMRef.CheckFieldOfVision(VisionDistance, VisionAngle);
    }
}