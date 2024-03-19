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

        //public struct NodeRecord
        //{
        //    private Vertex node; // Nodo seleccionado
        //    private Connection connection; // Conexión seleccionada a partir del nodo seleccionado
        //    private float costSoFar; // Coste acumulado hasta el nodo seleccionado
        //    private float estimatedTotalCost; // Coste total estimado hasta el nodo seleccionado
        //    private float cost;

        //    public NodeRecord(Vertex Node, Connection Connection = null, float CostSoFar = 0, float EstimatedTotalCost = 0, float Cost = 0)
        //    {
        //        node = Node;
        //        connection = Connection;
        //        costSoFar = CostSoFar;
        //        estimatedTotalCost = EstimatedTotalCost;
        //        cost = Cost;
        //    }

        //    public Vertex Node { get { return node; } set { node = value; } }
        //    public Connection Connection { get { return connection; } set { connection = value; } }
        //    public float CostSoFar { get { return costSoFar; } set { costSoFar = value; } }
        //    public float EstimatedTotalCost { get { return estimatedTotalCost; } set { estimatedTotalCost = value; } }
        //    public float Cost { get { return cost; } set { cost = value; } }
        //}

        //public struct PathFindingList
        //{
        //    private List<NodeRecord> records; // Lista de nodos

        //    public PathFindingList(List<NodeRecord> Records)
        //    {
        //        records = Records;
        //    }

        //    public void Add(NodeRecord record) { records.Add(record); }
        //    public void Remove(NodeRecord record) { records.Remove(record); }
        //    public bool Contains(Vertex node)
        //    {
        //        return Find(node) != null;
        //    }
        //    public NodeRecord? Find(Vertex node)
        //    {
        //        foreach (NodeRecord record in records)
        //        {
        //            if (record.Node == node) return record;
        //        }
        //        return null;
        //    }
        //    public NodeRecord? SmallestElement()
        //    {
        //        if (records == null || records.Count == 0) return null;

        //        NodeRecord smallest = records[0];
        //        foreach (NodeRecord record in records)
        //        {
        //            if (record.EstimatedTotalCost < smallest.EstimatedTotalCost)
        //            {
        //                smallest = record;
        //            }
        //        }
        //        return smallest;
        //    }

        //    public int Length() { return records.Count; }
        //}

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
                if (connection.FromNode == v)
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
            //Vertex startNode = GetNearestVertex(startObject.transform.position);
            //Vertex goalNode = GetNearestVertex(endObject.transform.position);
            //NodeRecord startRecord = new NodeRecord(startNode, null, 0, h(startNode, goalNode));
            //Debug.Log(startRecord);
            //PathFindingList open = new PathFindingList();
            //open.Add(startRecord);
            //Debug.Log(startRecord);
            //Debug.Log(open.SmallestElement());
            //PathFindingList closed = new PathFindingList();
            //NodeRecord current = new NodeRecord();

            //while (open.Length() > 0)
            //{
            //    current = open.SmallestElement();
            //    if (current.Node == goalNode) { break; }

            //    List<Connection> connections = GetConnectionNeighbours(current.Node);

            //    foreach (Connection connection in connections)
            //    {
            //        Vertex endNode = connection.ToNode;
            //        float endNodeCost = current.CostSoFar + connection.Cost;
            //        NodeRecord endNodeRecord = new NodeRecord();
            //        float endNodeHeuristic;

            //        if (closed.Contains(endNode))
            //        {
            //            endNodeRecord = closed.Find(endNode);

            //            if (endNodeRecord.CostSoFar <= endNodeCost)
            //            {
            //                continue;
            //            }

            //            closed.Remove(endNodeRecord);

            //            endNodeHeuristic = endNodeRecord.EstimatedTotalCost - endNodeRecord.CostSoFar;
            //        }
            //        else if (open.Contains(endNode))
            //        {
            //            endNodeRecord = open.Find(endNode);

            //            if (endNodeRecord.CostSoFar <= endNodeCost)
            //            {
            //                continue;
            //            }
            //            endNodeHeuristic = endNodeRecord.Cost - endNodeRecord.CostSoFar; // wtf
            //        }
            //        else
            //        {
            //            endNodeRecord = new NodeRecord();
            //            endNodeRecord.Node = endNode;
            //            endNodeHeuristic = h(endNode, goalNode);
            //        }

            //        endNodeRecord.Cost = endNodeCost; // wtf
            //        endNodeRecord.Connection = connection;
            //        endNodeRecord.EstimatedTotalCost = endNodeCost + endNodeHeuristic;

            //        if (!open.Contains(endNode))
            //        {
            //            open.Add(endNodeRecord);
            //        }
            //    }
            //    open.Remove(current);
            //    closed.Add(current);
            //}
            //if (current.Node != goalNode)
            //{
            //    return null;
            //}
            //else
            //{
            //    path.Add(current.Connection.FromNode);
            //    while (current.Node != startRecord.Node)
            //    {
            //        path.Add(current.Connection.ToNode);
            //        current.Node = current.Connection.FromNode;
            //    }

            //    path.Reverse();
            //    return path;
            //}
            return path;
        }

        public List<Vertex> Smooth(List<Vertex> inputPath)
        {
            // IMPLEMENTAR SUAVIZADO DE CAMINOS

            List<Vertex> outputPath = new List<Vertex>();

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
