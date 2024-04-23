using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Como NaiveFSM ya hereda de monoBehaviour, entonces PatrolAgentFSM tambi�n ya tiene las propiedades de MonoBehaviour.
public class PatrolAgentFSM : NaiveFSM
{
    // Como NaiveFSM ya hereda de monoBehaviour, entonces PatrolAgentFSM tambi�n ya tiene las propiedades de MonoBehaviour.
    private NaivePatrolState _PatrolState;
    private NaiveAlertState _AlertState;
    private NaiveAttackState _AttackState;//Del SCript de AttackState

    // aqu� faltar�a el estado de la clase NaiveAttackState, as� como su Getter, su inicializaci�n, etc.
    // eso les toca a ustedes.

    public NaivePatrolState PatrolStateRef
    {
        get { return _PatrolState; }
    }

    public NaiveAlertState AlertStateRef
    {
        get { return _AlertState; }
    }

    public NaiveAttackState AttackStateRef 
    {
        get { return _AttackState; }
    }

    // Ponerlo en el editor, o setearlo en el start.
    // Los estados pueden acceder a �l a trav�s de la m�quina de estados.
    public NavMeshAgent _NavMeshAgent;

    // Usaremos esta wall layer mask para hacer que el patrullero no pueda ver al jugador a trav�s de las paredes.
    // La inicializamos en el Start de nuestra FSM.
    // Para que funcione, tienen que ir a "Tags & Layers", agregar una nueva layer que se llame "Wall", y
    // asignarle dicha Layer a los gameobjects en su escena que sean paredes. Les recomiendo hacer un gameobject
    // vac�o que sea padre de las walls de su escenario, y hacer que ese padre tenga el Layer de wall, y aplicarle
    // el cambio de layer a todos los hijos (vean la Jerarqu�a de mi escena para que vean que todas mis walls
    // tienen el tag de Wall).
    LayerMask WallLayerMask;

    //Para Cambiar el Color de la luz
    public Light _light;

    // public float MovementSpeed;
    // La posici�n de nuestro agente la podemos acceder a trav�s del transform.

    // est� dado por la posici�n inicial del Patrullero en la escena. Es decir, su transform
    // La hice private porque nadie la debe de modificar por ahora, y la pueden acceder los estados 
    // a trav�s del Getter que puse justo debajo.
    private Vector3 _initialPatrolPosition = Vector3.zero;
    public Vector3 InitialPatrolPosition
    {
        get { return _initialPatrolPosition; }
    }

    // Otra ventaja de tenerlas en la FSM es que estas variables las podemos acceder y modificar desde el editor.
    // NOTA: Estas son el valor base para el estado de Patrullaje, pero se multiplican por un factor
    // en los otros estados. �Ok?
    public float VisionAngle = 45.0f;
    public float VisionDistance = 20.0f;


    // La variable detected plauyer s� queremos que sea accesible desde otros pedazos de c�digo, pero no desde el editor.
    private bool _DetectedPlayer = false;

    // este getter es por si alguno de los estados ya mand� actualizar CheckFieldOfVision pero quiere volver
    // a checar el resultado sin tener que volver a ejecutar toda la funci�n.
    public bool DetectedPlayer
    {
        get { return _DetectedPlayer; }
    }

    // El GameObject a trav�s del cual obtendremos el transform del jugador al que estaremos tratando de atrapar.
    private GameObject _PlayerGameObject;

    private Vector3 _LastKnownPlayerPosition = Vector3.zero;
    // Uso este getter the LastKnownPlayerPosition para brindar la posici�n a los estados. (solo lectura, No escritura)
    public Vector3 LastKnownPlayerPosition
    {
        get { return _LastKnownPlayerPosition; }
    }

    // Pero... �y qu� de las variables que son espec�ficas de ciertos estados?
    // Depende de c�mo lo quieran hacer ustedes.
    // Alternativa 1) s� poner las variables esas aqu� en la FSM :C 
    // Lo malo es que nos podr�an estar haciendo bulto en la FSM ya que no se usan en los dem�s estados.
    // Alternativa 2) Usar un archivo de lectura que contenga todos los valores para esas variables.
    // Por ejemplo, usar un archivo de tipo JSON
    // https://docs.unity3d.com/ScriptReference/JsonUtility.FromJson.html
    // Alternativa 2.5) Leer los valores desde un Excel o base de datos.
    // Cada una tiene sus ventajas y desventajas.

    // Grupo de variables para el estado de Patrullaje (NaivePatrolState)
    // Estas variables solo se van a setear en el estado al iniciar la m�quina de estados (por el momento).
    [Range(15.0f, 180.0f)]
    public float RotationAngle = 45.0f;
    [Range(0.25f, 20.0f)]
    public float TimeBeforeRotating = 3.0f;
    [Range(0.1f, 10.0f)]
    public float TimeDetectingPlayerBeforeEnteringAlert = 1.0f;


    // Grupo de variables para el estado de Alerta (NaiveAlertState)
    [Range(1.0f, 3.0f)]
    public float AlertVisionAngleMultiplier = 1.2f;
    [Range(1.0f, 3.0f)]
    public float AlertVisionDistanceMultiplier = 1.2f;
    [Range(0.5f, 10.0f)]
    public float TimeSinceLastSeenTreshold = 3.0f;
    [Range(5.0f, 50.0f)]
    public float SpeedWhileCheckingLastKnownLocation = 10.0f;  // ahorita no la voy a usar.


    // Grupo de variables para el estado de Ataque (NaiveAttackState).
    // Les toca implementarlo a ustedes.
    [Range(1.0f, 3.0f)]
    public float AttackVisionAngleMultiplier = 1.5f;
    [Range(1.0f, 5.0f)]
    public float AttackVisionDistanceMultiplier = 1.5f;

    // Esta es una extensi�n del Start de la clase FSM base, por lo que hace todo lo que ella har�a, m�s 
    // lo que se necesite espec�ficamente en esta clase.
    public override void Start()
    {
        //_Renderer = GetComponent<MeshRenderer>();
        _PlayerGameObject = GameObject.Find("SpaceRacer");

        // Le decimos expl�citamente que mande a llamar el start de su clase padre.
        base.Start();

        // Aqu� inicializamos esto, que usaremos con un raycast m�s abajo.
        WallLayerMask = LayerMask.GetMask("Wall");

        // Inicializamos esto como la posici�n en la que el agente patrullero inicia en la escena.
        _initialPatrolPosition = transform.position;

        _PatrolState = (NaivePatrolState)_CurrentState;
        _AlertState = new NaiveAlertState(this);
        _AttackState = new NaiveAttackState(this);//Quitamos Comentario jejeje


        // Mandamos a llamar las funciones Init de los estados, las que se encargan de setear las variables
        // de cada estado. Por ahora lo hacemos aqu�, pero recuerden que lo ideal ser�a a trav�s de alg�n 
        // archivo de configuraci�n, que lea estos valores y los ponga en cada estado como corresponda.
        _PatrolState.Init(VisionDistance, VisionAngle, RotationAngle, TimeBeforeRotating,
            TimeDetectingPlayerBeforeEnteringAlert);

        // A�ad� los multiplicadores de �ngulo y distancia a la inicializaci�n de los estados.
        _AlertState.Init(VisionDistance * AlertVisionDistanceMultiplier, VisionAngle * AlertVisionAngleMultiplier,
            TimeSinceLastSeenTreshold, SpeedWhileCheckingLastKnownLocation);

        // Se ponen MultIPLICA VA Y VD de Angule y Distance pero en ATKState
        _AttackState.Init(VisionAngle * AttackVisionAngleMultiplier, VisionDistance * AttackVisionDistanceMultiplier);
    }

    protected override NaiveFSMState GetInitialState()
    {
        // Regresa null para que cause error porque la funci�n de esta clase padre nunca debe de usarse, siempre 
        // se le debe de hacer un override.
        return new NaivePatrolState(this);
    }


    // OJO: esta funci�n la uso con par�metros de Distancia y �ngulo de visi�n en vez de usar 
    // las variables de la FSM, porque cada estado tiene sus propios valores de ellas, �ok?
    // Entonces hay que pasarle las variables de cada estado al mandarla a llamar.
    public bool CheckFieldOfVision(float in_VisionDist, float in_VisionAngle)
    {
        // Checa Distancia de Luz y Etsado de Patrullaje
        if (_PatrolState != null && _CurrentState == _PatrolState)
        {
            UpdateLightRange(_PatrolState.VisionDistance);
        }
        else if (_AlertState != null && _CurrentState == _AlertState)
        {
            UpdateLightRange(_AlertState.VisionDistance);
        }
        else if (_AttackState != null && _CurrentState == _AttackState)
        {
            UpdateLightRange(_AttackState.AttackVisionDistanceMultiplier);
        }
        // Esta funci�n regresa dos cosas, un booleano que dice: s� v� al jugador o no;
        // adem�s, si s� lo vimos, actualizar� la variable _LastKnownPlayerPosition.
        _DetectedPlayer = false; // por defecto la ponemos como falso.

        //Ponemos el Color en Verde
        _light.color = Color.green;
        //_Renderer.material.color = new Color(1, 0, 0, 1);
        Vector3 AgentToTargetVector = (_PlayerGameObject.transform.position - transform.position);

        // Profiling o benchmarking
        float AgentToTargetDist = AgentToTargetVector.magnitude;
        if (AgentToTargetDist > in_VisionDist)
        {
            // NNo esta en Rango
            return false;
        }

        if (Vector3.Angle(AgentToTargetVector, transform.forward) > in_VisionAngle * 0.5)
        {
            // No range
            return false;
        }

        // si el raycast choca primero contra una wall que contra el Target, entonces no  
        // puede ver al Target tal cual, porque hay una pared de por medio.
        if (Physics.Raycast(transform.position, AgentToTargetVector.normalized,
            AgentToTargetVector.magnitude, WallLayerMask))
        {
            return false;
        }

        // Si s� vimos al jugador, actualizar la variable de la �ltima posici�n conocida.
        _LastKnownPlayerPosition = _PlayerGameObject.transform.position;
        _DetectedPlayer = true; // solo cambia a verdadera aqu�. 
        //Cambiamos la luz a amarillo cuando detected player es true
        _light.color = Color.yellow;
        
        return true;
    }

    private void UpdateLightRange(float visionDistance)
    {
        // AJUSTA EL RANGO
        _light.range = visionDistance;
    }

    //Se establece ONDRAWGIZMOS como ya sabemos
    private void OnDrawGizmos()
    {
        if (_PatrolState != null)
        {
            Gizmos.color = Color.green;
            DrawVisionCone(_PatrolState.VisionDistance, _PatrolState.VisionAngle);
        }

        if (_AlertState != null && _CurrentState == _AlertState)
        {
            Gizmos.color = Color.yellow;
            DrawVisionCone(_AlertState.VisionDistance, _AlertState.VisionAngle);
        }

        if (_AttackState != null && _CurrentState == _AttackState)
        {
            Gizmos.color = Color.red;
            DrawVisionCone(_AttackState.AttackVisionDistanceMultiplier, _AttackState.AttackVisionAngleMultiplier);
        }
    }

    //Se diuja el cono de vision como ya sabemos 
    private void DrawVisionCone(float distance, float angle)
    {
        float halfAngle = angle * 0.5f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfAngle, Vector3.up);

        Vector3 startPoint = transform.position;

        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;

        Gizmos.DrawRay(startPoint, leftRayDirection * distance);
        Gizmos.DrawRay(startPoint, rightRayDirection * distance);
        Gizmos.DrawRay(startPoint + leftRayDirection * distance, rightRayDirection * distance - leftRayDirection * distance);
    }

}

