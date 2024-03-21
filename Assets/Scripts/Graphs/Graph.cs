/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using UnityEngine;
using System.Collections.Generic;

namespace UCM.IAV.Navegacion
{
    /// <summary>
    /// Clase abstracta para grafos
    /// </summary>
    public abstract class Graph : MonoBehaviour
    {
        #region Variables
        /// <summary>
        /// Prefab de vértice
        /// </summary>
        public GameObject vertexPrefab;

        /// <summary>
        /// Lista de vértices
        /// </summary>
        protected List<Vertex> vertices;

        /// <summary>
        /// Lista de vecinos de cada vértice
        /// </summary>
        protected List<List<Vertex>> neighbourVertex;

        /// <summary>
        /// Costes de las conexiones
        /// </summary>
        protected List<List<float>> costs;

        /// <summary>
        /// Mapa de vértices
        /// </summary>
        protected bool[,] mapVertices;

        /// <summary>
        /// Costes de los vértices
        /// </summary>
        protected float[,] costsVertices;

        /// <summary>
        /// Número de columnas y filas
        /// </summary>
        protected int numCols, numRows;

        /// <summary>
        /// Camino
        /// </summary>
        public List<Vertex> path;
        #endregion

        /// <summary>
        /// Delegado para la heurística
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public delegate float Heuristic(Vertex a, Vertex b);

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

        /// <summary>
        /// Devuelve una lista de vértices vecinos
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public virtual Vertex[] GetNeighbours(Vertex v)
        {
            if (ReferenceEquals(neighbourVertex, null) || neighbourVertex.Count == 0 ||
                v.id < 0 || v.id >= neighbourVertex.Count)
                return new Vertex[0];
            return neighbourVertex[v.id].ToArray();
        }

        /// <summary>
        /// Devuelve una lista de costes de los vértices vecinos
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Algoritmo de búsqueda BFS
        /// </summary>
        /// <param name="srcO"></param>
        /// <param name="dstO"></param>
        /// <returns></returns>
        public List<Vertex> GetPathBFS(GameObject srcO, GameObject dstO)
        {
            // IMPLEMENTAR ALGORITMO BFS
            return new List<Vertex>();
        }

        /// <summary>
        /// Algoritmo de búsqueda DFS
        /// </summary>
        /// <param name="srcO"></param>
        /// <param name="dstO"></param>
        /// <returns></returns>
        public List<Vertex> GetPathDFS(GameObject srcO, GameObject dstO)
        {
            // IMPLEMENTAR ALGORITMO DFS
            return new List<Vertex>();
        }

        /// <summary>
        /// Algoritmo de búsqueda A*
        /// </summary>
        /// <param name="startObject"></param>
        /// <param name="endObject"></param>
        /// <param name="heuristic"></param>
        /// <returns></returns>
        public List<Vertex> GetPathAstar(GameObject startObject, GameObject endObject, Heuristic heuristic = null)
        {
            Vertex start = GetNearestVertex(startObject.transform.position);
            Vertex goal = GetNearestVertex(endObject.transform.position);
            if (start == null || goal == null)
                return null;

            start.PreviousId = -1; // Lo inicializamos a "null"
            start.CostSoFar = 0;
            start.EstimatedTotalCost = heuristic(start, goal);

            BinaryHeap<Vertex> open = new BinaryHeap<Vertex>();
            open.Add(start);
            BinaryHeap<Vertex> closed = new BinaryHeap<Vertex>();

            Vertex current = new Vertex();

            while (open.Count > 0)
            {
                current = open.Top;
                if (current.id == goal.id)
                    break;

                Vertex[] connections = GetNeighbours(current);

                foreach (Vertex connection in connections)
                {
                    Vertex endNode = connection;
                    float endNodeCost = current.CostSoFar + connection.Cost;
                    float endNodeHeuristic;

                    if (closed.Contains(endNode))
                    {
                        if (endNode.CostSoFar <= endNodeCost)
                            continue;

                        closed.Remove(endNode);

                        endNodeHeuristic = endNode.EstimatedTotalCost - endNode.CostSoFar;
                    }
                    else if (open.Contains(endNode))
                    {
                        if (endNode.CostSoFar <= endNodeCost)
                            continue;

                        endNodeHeuristic = endNode.EstimatedTotalCost - endNode.CostSoFar;
                    }
                    else
                        endNodeHeuristic = heuristic(endNode, goal);

                    endNode.CostSoFar = endNodeCost;
                    endNode.PreviousId = current.id;
                    endNode.EstimatedTotalCost = endNodeCost + endNodeHeuristic;

                    if (!open.Contains(endNode))
                        open.Add(endNode);
                }

                open.Remove(current);
                closed.Add(current);
            }

            if (current.id != goal.id)
                return null;
            else
                return BuildPath(current, start);
        }


        /// <summary>
        /// Algoritmo de suavizado de camino
        /// </summary>
        /// <param name="inputPath"></param>
        /// <returns></returns>
        public List<Vertex> Smooth(List<Vertex> inputPath)
        {
            if (inputPath == null || inputPath.Count == 0)
                return inputPath;

            LayerMask obstaculo = LayerMask.GetMask("Obstaculo"); // defino la capa de obstáculos a examinar

            // Si el camino es solo de dos nodos de longitud, entonces no se puede suavizar
            if (inputPath.Count == 2)
                return inputPath;

            // Compilar un camino de salida
            List<Vertex> outputPath = new List<Vertex>() { inputPath[0] };

            // Se hace un seguimiento de la posición en la que se encuentra. Se empieza con dos, ya que se asume que dos nodos adyacentes pasarán el trazado del rayo
            int inputIndex = 2;

            // Bucle hasta que encontramos el último item del input.
            while (inputIndex < inputPath.Count - 1)
            {
                Vector3 fromPt = outputPath[outputPath.Count - 1].transform.position; // vector posición inicio nodo salida
                Vector3 toPt = inputPath[inputIndex].transform.position; // vector posición siguiente nodo entrada

                fromPt.y += 1.0f; // subo las alturas
                toPt.y += 1.0f; // de los rayos esféricos

                RaycastHit hitInfo; // no se necesita en sí pero el método siguiente lo necesita para funcionar

                // Si el trazado de rayo ha fallado, se añade el último nodo al final de la lista de salida.
                if (Physics.SphereCast(fromPt, 0.5f, (toPt - fromPt), out hitInfo, (toPt - fromPt).magnitude, obstaculo))
                {
                    outputPath.Add(inputPath[inputIndex - 1]); // añado el siguiente nodo a la lista de salida
                }

                // Se considera el siguiente nodo.
                inputIndex++;
            }

            // Se ha llegado al final del camino de entrada, por lo que se añade el último nodo al de salida y se devuelve
            outputPath.Add(inputPath[inputPath.Count - 1]);

            return outputPath; // devuelvo la lista
        }

        /// <summary>
        /// Reconstruye el camino reviertiendo la lista de nodos
        /// </summary>
        /// <param name="current"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        private List<Vertex> BuildPath(Vertex current, Vertex start)
        {
            List<Vertex> path = new List<Vertex>() { current }; // path.Add(current);
            while (current.id != start.id)
            {
                Vertex prevVertex = GetVertexById(current.PreviousId);
                path.Add(prevVertex);
                current = prevVertex;
            }
            path.Reverse();
            return path;
        }

        /// <summary>
        /// Devuelve un vértice a partir de un identificador
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Vertex GetVertexById(int id)
        {
            if (id < 0 || id >= vertices.Count)
                return null;
            return vertices[id];
        }

        /// <summary>
        /// Devuelve el coste del camino
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Devuelve el tiempo de búsqueda del camino
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public float GetPathSearchTime(List<Vertex> path)
        {
            return path.Count * Time.deltaTime;
        }

        /// <summary>
        /// Devuelve el porcentaje del tiempo consumido en la búsqueda del camino
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public float GetPathSearchTimePercentage(List<Vertex> path)
        {
            return GetPathSearchTime(path) / Time.deltaTime;
        }
    }
}
