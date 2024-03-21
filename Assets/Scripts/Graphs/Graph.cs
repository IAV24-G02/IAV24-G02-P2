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
    using System.Collections.Generic;

    /// <summary>
    /// Clase abstracta para grafos
    /// </summary>
    public abstract class Graph : MonoBehaviour
    {
        public GameObject vertexPrefab;                         // Prefab de vértice
        protected List<Vertex> vertices;                        // Lista de vértices
        protected List<List<Vertex>> neighbourVertex;           // Lista de vecinos de cada vértice
        protected List<List<float>> costs;                      // Costes de las conexiones
        protected bool[,] mapVertices;                          // Mapa de vértices
        protected float[,] costsVertices;                       // Costes de los vértices
        protected int numCols, numRows;                         // Número de columnas y filas
        protected List<Connection> neighbourConnections;        // Listas de conexiones vecinas

        // Delegado para la heurística
        public delegate float Heuristic(Vertex a, Vertex b);

        // Used for getting path in frames
        public List<Vertex> path;

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

        public virtual void UpdateVertexCost(Vertex v, float costMultipliyer) { }

        public virtual Vertex GetNearestVertex(Vector3 position)
        {
            return null;
        }

        public virtual GameObject GetRandomPos()
        {
            return null;
        }

        // Devuelve una lista de conexiones vecinas
        public virtual HashSet<Connection> GetConnectionNeighbours(Vertex v)
        {
            // Usar un set para evitar la duplicación de conexiones
            HashSet<Connection> uniqueConnections = new HashSet<Connection>();
            foreach (Connection connection in neighbourConnections)
            {
                if (connection != null && connection.FromNode == v)
                    uniqueConnections.Add(connection);
            }
            // Devolverlo como una lista
            return uniqueConnections;
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

        // Encuentra caminos óptimos
        public List<Vertex> GetPathAstar(GameObject startObject, GameObject endObject, Heuristic heuristic = null)
        {
            Vertex startNode = GetNearestVertex(startObject.transform.position);
            Vertex goalNode = GetNearestVertex(endObject.transform.position);
            if (startNode == null || goalNode == null)
                return null;

            NodeRecord startRecord = new NodeRecord(startNode, null, 0, heuristic(startNode, goalNode));

            PathFindingList open = new PathFindingList();
            open.Add(startRecord);
            PathFindingList closed = new PathFindingList();

            NodeRecord current = new NodeRecord();

            while (open.Length() > 0)
            {
                current = open.SmallestElement();
                if (current.Node == goalNode)
                    break;

                HashSet<Connection> connections = GetConnectionNeighbours(current.Node);

                foreach (Connection connection in connections)
                {
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

                        endNodeHeuristic = endNodeRecord.Cost - endNodeRecord.CostSoFar;
                    }
                    else
                    {
                        endNodeRecord = new NodeRecord(endNode, current, connection.Cost);
                        endNodeHeuristic = heuristic(endNode, goalNode);
                    }

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
                path = new List<Vertex>() { current.Node }; // path.Add(current.Node);
                while (current.PreviousNode != startRecord.PreviousNode)
                {
                    path.Add(current.PreviousNode.Node);
                    current = current.PreviousNode;
                }
                
                path.Reverse();
                return path;
            }
        }

        public List<Vertex> Smooth(List<Vertex> inputPath)
        {
            // Si el camino es solo de dos nodos de longitud, entonces no se puede suavizar
            if (inputPath.Count == 2)
                return inputPath;

            // Compilar un camino de salida
            List<Vertex> outputPath = new List<Vertex>() { inputPath[0] }; // outputPath.Add(inputPath[0]);

            //Se hace un seguimiento de la posición en la que se encuentra. Se empieza con dos, ya que se asume que dos nodos adyacentes pasarán el trazado del rayo
            int inputIndex = 2;

            //Bucle hasta que encontramos el último item del input.
            while (inputIndex < inputPath.Count - 1)
            {
                Vector3 fromPt = outputPath[outputPath.Count - 1].transform.position;
                Vector3 toPt = inputPath[inputIndex].transform.position;
                RaycastHit hitInfo;
                // Si el trazado de rayo ha fallado, se añade el último nodo al final de la lista de salida.
                if (Physics.SphereCast(fromPt, 2f, toPt - fromPt, out hitInfo, (toPt - fromPt).magnitude))
                {
                    outputPath.Add(inputPath[inputIndex - 1]);
                }

                // Se considera el siguiente nodo.
                inputIndex++;
            }

            // Se ha llegado al final del camino de entrada, por lo que se añade el último nodo al de salida y se devuelve
            outputPath.Add(inputPath[inputPath.Count - 1]);
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

        // Devuelve el coste del camino
        public float GetPathCost(List<Vertex> path)
        {
            float cost = 0;
            for (int i = 0; i < path.Count - 1; i++)
            {
                int j = (int)Mathf.Floor(path[i].id / numCols);
                int k = (int)Mathf.Floor(path[i].id % numCols);
                cost += costsVertices[j, k];
            }
            return cost;
        }

        // Devuelve el tiempo de búsqueda del camino
        public float GetPathSearchTime(List<Vertex> path)
        {
            return path.Count * Time.deltaTime;
        }

        // Devuelve el porcentaje del tiempo consumido en la búsqueda del camino
        public float GetPathSearchTimePercentage(List<Vertex> path)
        {
            return GetPathSearchTime(path) / Time.deltaTime;
        }
    }
}
