/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
namespace UCM.IAV.Navegacion
{

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Unity.PlasticSCM.Editor.WebApi;
    using System.Runtime.InteropServices;
    using UnityEditor.Experimental.GraphView;

    /// <summary>
    /// Abstract class for graphs
    /// </summary>
    public abstract class Graph : MonoBehaviour
    {
        // Aquí el grafo entero es representado con estas listas, que luego puede aprovechar el algoritmo A*.
        // El pseudocódigo de Millington no asume que tengamos toda la información del grafo representada y por eso va guardando registros de los nodos que visita.
        public GameObject vertexPrefab;
        protected List<Vertex> vertices;
        protected List<List<Vertex>> neighbourVertex;
        protected List<List<float>> costs;
        protected bool[,] mapVertices;
        protected float[,] costsVertices;
        protected int numCols, numRows;
        protected List<Connection> neighbourConnections;

        // this is for informed search like A*
        // Un delegado especifica la cabecera de una función, la que sea, que cumpla con esos parámetros y devuelva ese tipo.
        // Cuidado al implementarlas, porque no puede ser que la distancia -por ejemplo- entre dos casillas tenga una heurística más cara que el coste real de navegar de una a otra.
        public delegate float Heuristic(Vertex a, Vertex b);

        // Used for getting path in frames
        public List<Vertex> path;

        [Range(0, Mathf.Infinity)]
        public float defaultCost = 1f;
        [Range(0, Mathf.Infinity)]
        public float maximumCost = Mathf.Infinity;

        public virtual void Start()
        {
            Load();
        }

        public virtual void Load() { }

        public virtual int GetSize()
        {
            if (ReferenceEquals(vertices, null))
                return 0;
            return vertices.Count;
        }

        public virtual void UpdateVertexCost(Vector3 position, float costMultipliyer) { }

        public virtual Vertex GetNearestVertex(Vector3 position)
        {
            return null;
        }

        public virtual GameObject GetRandomPos()
        {
            return null;
        }
        public virtual List<Connection> GetConnectionNeighbours(Vertex v)
        {
            List<Connection> connections = new List<Connection>();
            foreach (Connection connection in neighbourConnections)
            {
                if (connection != null && connection.FromNode == v)
                {
                    connections.Add(connection);
                }
            }
            return connections;
        }

        public virtual Vertex[] GetNeighbours(Vertex v)
        {
            if (ReferenceEquals(neighbourVertex, null) || neighbourVertex.Count == 0 ||
                v.id < 0 || v.id >= neighbourVertex.Count)
                return new Vertex[0];
            return neighbourVertex[v.id].ToArray();
        }

        public virtual float[] GetNeighboursCosts(Vertex v)
        {
            if (ReferenceEquals(neighbourVertex, null) || neighbourVertex.Count == 0 ||
                v.id < 0 || v.id >= neighbourVertex.Count)
                return new float[0];

            Vertex[] neighs = neighbourVertex[v.id].ToArray();
            float[] costsV = new float[neighs.Length];
            for (int neighbour = 0; neighbour < neighs.Length; neighbour++)
            {
                int j = (int)Mathf.Floor(neighs[neighbour].id / numCols);
                int i = (int)Mathf.Floor(neighs[neighbour].id % numCols);
                costsV[neighbour] = costsVertices[j, i];
            }

            return costsV;
        }

        // Encuentra caminos óptimos
        public List<Vertex> GetPathBFS(GameObject srcO, GameObject dstO)
        {
            // IMPLEMENTAR ALGORITMO BFS
            return new List<Vertex>();
        }

        // No encuentra caminos óptimos
        public List<Vertex> GetPathDFS(GameObject srcO, GameObject dstO)
        {
            // IMPLEMENTAR ALGORITMO DFS
            return new List<Vertex>();
        }

        public List<Vertex> GetPathAstar(GameObject startObject, GameObject endObject, Heuristic h = null)
        {
            Vertex startNode = GetNearestVertex(startObject.transform.position);
            Vertex goalNode = GetNearestVertex(endObject.transform.position);
            if (startNode == null || goalNode == null)
                return null;

            NodeRecord startRecord = new NodeRecord(startNode, null, 0, h(startNode, goalNode));

            PathFindingList open = new PathFindingList();
            open.Add(startRecord);
            PathFindingList closed = new PathFindingList();

            NodeRecord current = new NodeRecord();

            while (open.Length() > 0)
            {
                current = open.SmallestElement();
                if (current.Node == goalNode) 
                    break;

                List<Connection> connections = GetConnectionNeighbours(current.Node);

                foreach (Connection connection in connections)
                {
                    Debug.Log("Connection: " + connection.FromNode + " -> " + connection.ToNode + " Cost: " + connection.Cost);
                    Vertex endNode = connection.ToNode;
                    float endNodeCost = current.CostSoFar + connection.Cost;
                    NodeRecord endNodeRecord;
                    float endNodeHeuristic;

                    if (closed.Contains(endNode))
                    {
                        endNodeRecord = closed.Find(endNode);

                        if (endNodeRecord.CostSoFar <= endNodeCost)
                            continue;

                        closed.Remove(endNodeRecord);

                        endNodeHeuristic = endNodeRecord.EstimatedTotalCost - endNodeRecord.CostSoFar;
                    }
                    else if (open.Contains(endNode))
                    {
                        endNodeRecord = open.Find(endNode);

                        if (endNodeRecord.CostSoFar <= endNodeCost)
                            continue;

                        endNodeHeuristic = endNodeRecord.Connection.Cost - endNodeRecord.CostSoFar;
                    }
                    else
                    {
                        Connection newConnection = new Connection(current.Node, endNode, defaultCost);
                        endNodeRecord = new NodeRecord(endNode, newConnection);
                        endNodeHeuristic = h(endNode, goalNode);
                    }

                    endNodeRecord.Connection = connection;
                    endNodeRecord.CostSoFar = endNodeCost;
                    endNodeRecord.EstimatedTotalCost = endNodeCost + endNodeHeuristic;

                    if (!open.Contains(endNode))
                        open.Add(endNodeRecord);
                }

                open.Remove(current);
                closed.Add(current);
            }

            if (current.Node != goalNode)
                return null;
            else
            {
                path.Add(current.Connection.FromNode);
                while (current.Node != startRecord.Node)
                {
                    path.Add(current.Connection.ToNode);
                    current.Node = current.Connection.FromNode;
                }

                path.Reverse();
                return path;
            }
        }

        public List<Vertex> Smooth(List<Vertex> inputPath)
        {
            // IMPLEMENTAR SUAVIZADO DE CAMINOS
            inputPath.Reverse();

            //Si el camino es solo de dos nodos de longitud, entonces no se puede suavizar
            if (inputPath.Count <= 2) return inputPath;

            //Compilar un camino de salida
            List<Vertex> outputPath = new List<Vertex>();
            outputPath.Add(inputPath[0]);

            //Se hace un seguimiento de la posición en la que se encuentra. Se empieza con dos, ya que se asume que dos nodos adyacentes pasarán el trazado del rayo
            int inputIndex = 2;

            //Bucle hasta que encontramos el último item del input.
            while (inputIndex < inputPath.Count - 1)
            {
                Vector3 fromPt = outputPath[outputPath.Count - 1].transform.position;
                Vector3 toPt = inputPath[inputIndex].transform.position;

                RaycastHit hitInfo = new RaycastHit();
                //Si el trazado de rayo ha fallado, se añade el último nodo al final de la lista de salida.
                if (Physics.SphereCast(fromPt, 2f, toPt - fromPt, out hitInfo, (toPt - fromPt).magnitude))
                {
                    outputPath.Add(inputPath[inputIndex - 1]);
                }
                
                //Se considera el siguiente nodo.
                inputIndex++;
            }

            //Se ha llegado al final del camino de entrada, por lo que se añade el último nodo al de salida y se devuelve
            outputPath.Add(inputPath[inputPath.Count - 1]);
            outputPath.Reverse();
            return outputPath;

        }

        // Reconstruir el camino, dando la vuelta a la lista de nodos 'padres' /previos que hemos ido anotando
        private List<Vertex> BuildPath(int srcId, int dstId, ref int[] prevList)
        {
            List<Vertex> path = new List<Vertex>();

            if (dstId < 0 || dstId >= vertices.Count)
                return path;

            int prev = dstId;
            do
            {
                path.Add(vertices[prev]);
                prev = prevList[prev];
            } while (prev != srcId);
            return path;
        }
    }
}
