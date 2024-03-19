using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// En este script va a estar todo lo que tenga que ver con que el grafo funcione a un nivel básico.

public class Node
{
    public string ID;
    // Una que no me sabía yo de Unity C#: Las structs no pueden tener objetos de su propia struct, 
    // pero las classes sí. Probablemente se debe a que las Classes son siempre referencias (punteros) en C#.
    public Node parent;
     //public float X; // Coordenada X
    //public float Y; // Coordenada Y
    public Node(string in_Id)
    {
        ID = in_Id;
        parent = null;
    }
    //== es el operador de igualdad.
    //public static bool operator ==(Node lhs, Node rhs)
    //{
    //    if (rhs.ID == lhs.ID)
    //        return true;
    //    return false;
    //}

    //// != es el operador de inigualdad.
    //public static bool operator !=(Node lhs, Node rhs)
    //{
    //    if (rhs.ID != lhs.ID)
    //        return true;
    //    return false;
    //}

    // !
}

public struct Edge
{
    public Node a;
    public Node b;

    public Edge(Node in_a, Node in_b)
    {
        a = in_a;
        b = in_b;
    }
}
// Templates o plantillas.

public enum NodeState
{
    Unknown = 0,
    Open,
    Closed,
    MAX
}

public class BG : MonoBehaviour
{
    public List<Edge> Edges = new List<Edge>();
    public List<Node> Nodes = new List<Node>();
    public Dictionary<Node, NodeState> NodeStateDict = new Dictionary<Node, NodeState>();
    public Queue<Node> OpenQueue = new Queue<Node>();
    public HashSet<Node> ClosedSetList = new HashSet<Node>();

    private void GrafoDePrueba()
    {
        // Me faltaba ponerle el "public" al constructor!
        // Ponemos todos los nodos de nuestro diagrama.
        Node A = new Node("A");
        Node B = new Node("B");
        Node C = new Node("C");
        Node D = new Node("D");
        Node E = new Node("E");
        Node F = new Node("F");
        Node G = new Node("G");
        Node H = new Node("H");

        // Por defecto, nuestro Diccionario (que tiene tanto la lista de abiertos como de cerrados)
        // Va a tener a todos nuestros nodos.
        Nodes.Add(A);
        Nodes.Add(B);
        Nodes.Add(C);
        Nodes.Add(D);
        Nodes.Add(E);
        Nodes.Add(F);
        Nodes.Add(G);
        Nodes.Add(H);

        foreach (Node node in Nodes)
        {
            NodeStateDict.Add(node, NodeState.Unknown);
        }

        // Ahora ponemos los Edges/Aristas, es decir, conexiones entre nodos.

        Edge AB = new Edge(A, B);
        Edge AE = new Edge(A, E);
        Edge BC = new Edge(B, C);
        Edge BD = new Edge(B, D);
        Edge EF = new Edge(E, F);
        Edge EG = new Edge(E, G);
        Edge EH = new Edge(E, H);

        // Metemos nuestras aristas en la lista de Aristas.

        Edges.Add(EH);
        Edges.Add(EG);
        Edges.Add(EF);
        Edges.Add(BD);
        Edges.Add(BC);
        Edges.Add(AE);
        Edges.Add(AB);

        NodeStateDict[H] = NodeState.Open;
        bool pathExists = BusquedaEnAnchura(H, D);
        if (pathExists)
        {
            Debug.Log("Sí hay un camino de H a D.");
            List<Node> pathToGoal = new List<Node>();
            Node currentNode = D;
            while (currentNode != null)
            {
                pathToGoal.Insert(0, currentNode);
                currentNode = currentNode.parent;
            }
            foreach (Node node in pathToGoal)
            {
                Debug.Log("El nodo: " + node.ID + " fue parte del camino a la meta");
            }
        }
        else
            Debug.Log("No hay camino de H a D.");
    }

    public bool BusquedaEnAnchura(Node Origen, Node Objetivo)
    {
        OpenQueue.Enqueue(Origen);

        while (OpenQueue.Count != 0)
        {
            Node nodoActual = OpenQueue.Dequeue();
            List<Edge> vecinosActuales = EncontrarVecinos(nodoActual);
            foreach (Edge e in vecinosActuales)
            {
                Node nodoNoActual = nodoActual != e.a ? e.a : e.b;
                if (ClosedSetList.Contains(nodoNoActual))
                    continue;
                if (nodoNoActual == Objetivo)
                {
                    nodoNoActual.parent = nodoActual;
                    return true;
                }
                else
                {
                    OpenQueue.Enqueue(nodoNoActual);
                    nodoNoActual.parent = nodoActual;
                }
            }
            ClosedSetList.Add(nodoActual);
        }
        return false;
    }

    public List<Edge> EncontrarVecinos(Node nodo)
    {
        List<Edge> listaSalida = new List<Edge>();
        foreach (Edge miArista in Edges)
        {
            if (miArista.a == nodo || miArista.b == nodo)
            {
                listaSalida.Add(miArista);
            }
        }
        return listaSalida;
    }

    void Start()
    {
        GrafoDePrueba();
    }
}
