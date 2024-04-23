using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// esta es la clase FSM "BASE", es decir, todas las máquinas de estados van a heredar de ésta.
// La FSM sí hereda de monobehaviour porque se le asigna al GameObject en editor (o a través de código)
public class NaiveFSM : MonoBehaviour
{
    public NaiveFSMState _CurrentState;

    // Que pueda transicionar entre todos los estados de nuestro dise�o (de nuestro diagrama o lo que sea que tengamos).
    // Para ello, hacemos que la FSM los contenga.
    //NaivePatrolState _PatrolState;
    //NaiveAlertState _AlertState;
    //NaiveAttackState _AttackState;
    // public Dictionary<string, NaiveFSMState> _StatesDict;

    // Start is called before the first frame update
    public virtual void Start()
    {
        // Una FSM siempre inicia en su estado inicial.
        _CurrentState = GetInitialState();
        // Ahora nos toca entrar al estado (es decir, llamar su funci�n Enter() )
        if (_CurrentState == null)
            Debug.LogError("No hay un estado inicial valido asignado.");
        else
        {
            _CurrentState.Enter();
        }
    }

    protected virtual NaiveFSMState GetInitialState()
    {
        // Regresa null para que cause error porque la funci�n de esta clase padre nunca debe de usarse, siempre 
        // se le debe de hacer un override.
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        // Mayormente solo debe importar lo que hace el estado actual, y nada de otros estados de la m�quina.
        if (_CurrentState != null)
            _CurrentState.Update();
    }

    // Lo mismo va a pasar con el FixedUpdate.

    // La funci�n para cambiar entre estados.
    public void ChangeState(NaiveFSMState newState)
    {
        // Manda a llamar el Exit() del estado actual.
        _CurrentState.Exit();
        // Pone que el estado nuevo es ahora el estado actual (current)
        _CurrentState = newState;
        // Manda a llamar el Enter() de este nuevo estado.
        _CurrentState.Enter();
    }

    private void OnGUI()
    {
        string text = _CurrentState != null ? _CurrentState.Name : "No current State asigned";
        GUILayout.Label($"<size=40>{text}</size>");
    }
}
