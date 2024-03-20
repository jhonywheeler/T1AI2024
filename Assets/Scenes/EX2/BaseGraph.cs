using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Se define un nodo en el grafo con un identificador único,
una referencia al nodo padre (para la reconstrucción
del camino) y una posición en el espacio de Unity. */
public class Node
{
    public string ID; // Identificador único del nodo
    public Node parent; // Nodo padre en el camino encontrado
    public Vector3 position; // Posición en el espacio de Unity

    // Constructor de la clase Node
    public Node(string in_Id, Vector3 in_position)
    {
        ID = in_Id;
        parent = null;
        position = in_position;
    }
}

// Estructura para representar una arista en el grafo
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

/* Este método enumera los posibles estados de un
   nodo durante la búsqueda. Unknown se utiliza para 
   inicializar el estado, Open para marcar un nodo como 
   visitado pero aun no explorado, y Closed para marcar un
   nodo como visitado y explorado. */
public enum NodeState
{
    Unknown = 0, // Estado inicial desconocido
    Open,  // Nodo marcado como abierto pero no explorado
    Closed,// Nodo marcado como explorado
    MAX
}

/* Se define el grafo y el algoritmo BFS para encontrar un
 camino desde un nodo de origen a un nodo objetivo */
public class BaseGraph : MonoBehaviour
{
    // GameObjects para representar los nodos en Unity
    public GameObject nodePrefabA;
    public GameObject nodePrefabB;
    public GameObject nodePrefabC;
    public GameObject nodePrefabD;
    public GameObject nodePrefabE;
    public GameObject nodePrefabF;
    public GameObject nodePrefabG;
    public GameObject nodePrefabH;

    // Método para obtener los nodos objetivo del grafo
    public List<GameObject> GetTargetNodes()
    {
        List<GameObject> targetNodes = new List<GameObject>();
        if (nodePrefabA != null) targetNodes.Add(nodePrefabA);
        if (nodePrefabB != null) targetNodes.Add(nodePrefabB);
        if (nodePrefabC != null) targetNodes.Add(nodePrefabC);
        if (nodePrefabD != null) targetNodes.Add(nodePrefabD);
        if (nodePrefabE != null) targetNodes.Add(nodePrefabE);
        if (nodePrefabF != null) targetNodes.Add(nodePrefabF);
        if (nodePrefabG != null) targetNodes.Add(nodePrefabG);
        if (nodePrefabH != null) targetNodes.Add(nodePrefabH);
        return targetNodes;
    }

    private List<Node> pathToFollow = new List<Node>();
    public Color targetColor = Color.red; // Color para resaltar el camino encontrado
    public float colorChangeDelay = 0.5f; // Retraso entre cambios de color

    // Listas y diccionarios para almacenar nodos, aristas y estados de nodos
    public List<Edge> edges = new List<Edge>();
    public List<Node> nodes = new List<Node>();
    public Dictionary<Node, NodeState> nodeStateDict = new Dictionary<Node, NodeState>();

    // Cola para los nodos abiertos y conjunto para los nodos cerrados
    public Queue<Node> openQueue = new Queue<Node>();
    public HashSet<Node> closedSetList = new HashSet<Node>();

    // Método para crear un grafo de prueba y realizar la búsqueda BFS
    private void GrafoDePrueba()
    {
        // Crear nodos y definir posiciones
        Node A = new Node("A", new Vector3(0, 0, 0));
        Node B = new Node("B", new Vector3(1, 0, 0));
        Node C = new Node("C", new Vector3(2, 0, 0));
        Node D = new Node("D", new Vector3(2, 1, 0));
        Node E = new Node("E", new Vector3(1, 1, 0));
        Node F = new Node("F", new Vector3(0, 1, 0));
        Node G = new Node("G", new Vector3(0, 2, 0));
        Node H = new Node("H", new Vector3(1, 2, 0));

        // Agregar nodos
        nodes.Add(A);
        nodes.Add(B);
        nodes.Add(C);
        nodes.Add(D);
        nodes.Add(E);
        nodes.Add(F);
        nodes.Add(G);
        nodes.Add(H);

        // Inicializar los estados de los nodos como desconocidos
        nodeStateDict.Add(A, NodeState.Unknown);
        nodeStateDict.Add(B, NodeState.Unknown);
        nodeStateDict.Add(C, NodeState.Unknown);
        nodeStateDict.Add(D, NodeState.Unknown);
        nodeStateDict.Add(E, NodeState.Unknown);
        nodeStateDict.Add(F, NodeState.Unknown);
        nodeStateDict.Add(G, NodeState.Unknown);
        nodeStateDict.Add(H, NodeState.Unknown);

        // Crear aristas
        Edge AB = new Edge(A, B);
        Edge AE = new Edge(A, E);
        Edge BC = new Edge(B, C);
        Edge BD = new Edge(B, D);
        Edge EF = new Edge(E, F);
        Edge EG = new Edge(E, G);
        Edge EH = new Edge(E, H);

        // Agregar aristas al grafo
        edges.Add(AB);
        edges.Add(AE);
        edges.Add(BC);
        edges.Add(BD);
        edges.Add(EF);
        edges.Add(EG);
        edges.Add(EH);

        // Iniciar BFS desde el nodo H hacia el nodo D
        nodeStateDict[H] = NodeState.Open;
        bool pathExists = IterativeBFS(H, C);
        if (pathExists)
        {
            Debug.Log("Sí hay un camino de H a C.");
            StorePathToFollow(C); // Almacenar el camino encontrado
            PrintPath(C);
        }
        else
            Debug.Log("No hay camino de H a C.");
    }

    private void StorePathToFollow(Node target)
    {
        pathToFollow.Clear();
        Node currentNode = target;
        while (currentNode != null)
        {
            pathToFollow.Add(currentNode);
            currentNode = currentNode.parent;
        }
        pathToFollow.Reverse(); // Invertir el camino para que sea en el orden correcto
    }

    // Método para obtener el camino que el agente debe seguir
    public List<Node> GetPathToFollow()
    {
        return pathToFollow;
    }

    // Método para realizar la búsqueda BFS de manera iterativa

    public bool IterativeBFS(Node Origin, Node Target)
    {
        openQueue.Enqueue(Origin);

        while (openQueue.Count != 0)
        {
            Node currentNode = openQueue.Dequeue();
            List<Edge> currentNeighbors = FindNeighbors(currentNode);

            foreach (Edge e in currentNeighbors)
            {
                Node NonCurrentNode = currentNode != e.a ? e.a : e.b;
                if (closedSetList.Contains(NonCurrentNode))
                    continue;

                if (NonCurrentNode == Target)
                {
                    NonCurrentNode.parent = currentNode;
                    return true;
                }
                else
                {
                    openQueue.Enqueue(NonCurrentNode);
                    NonCurrentNode.parent = currentNode;
                }
            }

            closedSetList.Add(currentNode);
        }

        return false;
    }

    // Método para imprimir el camino encontrado
    public void PrintPath(Node target)
    {
        List<Node> pathToGoal = new List<Node>();
        Node currentNode = target;
        while (currentNode != null)
        {
            pathToGoal.Add(currentNode);
            currentNode = currentNode.parent;
        }
        pathToGoal.Reverse(); // Invertir el camino para imprimirlo en orden correcto

        StartCoroutine(ColorPath(pathToGoal));
    }

    // Corrutina para cambiar el color de los nodos en el camino
    IEnumerator ColorPath(List<Node> path)
    {
        foreach (Node node in path)
        {
            Debug.Log("El nodo: " + node.ID + " fue parte del camino a la meta.");
            ColorNode(node);  // Cambiar el color del nodo en Unity
            yield return new WaitForSeconds(colorChangeDelay);
        }
    }

    // Método para encontrar los nodos vecinos de un nodo dado
    public List<Edge> FindNeighbors(Node in_node)
    {
        List<Edge> out_list = new List<Edge>();
        foreach (Edge myEdge in edges)
        {
            if (myEdge.a == in_node || myEdge.b == in_node)
            {
                out_list.Add(myEdge);
            }
        }
        return out_list;
    }

    void Start()
    {
        GrafoDePrueba();
    }

    void Update()
    {

    }

    void ColorNode(Node node)
    {
        switch (node.ID)
        {
            case "A":
                nodePrefabA.GetComponent<Renderer>().material.color = targetColor;
                break;
            case "B":
                nodePrefabB.GetComponent<Renderer>().material.color = targetColor;
                break;
            case "C":
                nodePrefabC.GetComponent<Renderer>().material.color = targetColor;
                break;
            case "D":
                nodePrefabD.GetComponent<Renderer>().material.color = targetColor;
                break;
            case "E":
                nodePrefabE.GetComponent<Renderer>().material.color = targetColor;
                break;
            case "F":
                nodePrefabF.GetComponent<Renderer>().material.color = targetColor;
                break;
            case "G":
                nodePrefabG.GetComponent<Renderer>().material.color = targetColor;
                break;
            case "H":
                nodePrefabH.GetComponent<Renderer>().material.color = targetColor;
                break;
            default:
                break;
        }
    }
}

