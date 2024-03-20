/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using UCM.IAV.Movimiento;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace UCM.IAV.Navegacion
{
    public enum TesterGraphAlgorithm
    {
        BFS, DFS, ASTAR
    }

    public class TheseusGraph : MonoBehaviour
    {
        [SerializeField]
        protected Graph graph;                      // Grafo de nodos

        [SerializeField]
        private TesterGraphAlgorithm algorithm;     // Algoritmo a usar

        [SerializeField]
        private GameObject spherePrefab;            // Prefab de esfera

        List<GameObject> pathSpheres;               // Esferas del camino

        [SerializeField]
        private Material pathThreadMaterial;        // Material del camino

        [SerializeField]
        private Color pathColor;                    // Color del camino

        [SerializeField]
        [Range(0.1f, 1f)]
        private float pathNodeRadius = .3f;         // Radio de los nodos del camino

        private bool ariadna;                       // Activa o desactiva el hilo de Ariadna
        private bool smoothPath;                    // Suavizar el camino
        private bool firstHeuristic;                // Indica si se está usando la primera heurística
        protected GameObject srcObj;                // Objeto origen
        protected GameObject dstObj;                // Objeto destino
        protected List<Vertex> path;                // Camino calculado
        private int pathVisited;                    // Nodos visitados
        private int pathLength;                     // Longitud del camino
        private float pathCost;                     // Coste del camino
        private float pathSearchTime;               // Tiempo de búsqueda
        private float pathPercentageTimeConsumed;   // Porcentaje de tiempo consumido en la búsqueda

        protected LineRenderer hilo;                // Hilo de Ariadna
        protected float hiloOffset = 0.2f;          // Offset del hilo

        public virtual void Awake()
        {
            srcObj = GameManager.instance.GetPlayer();
            dstObj = null;
            path = null;
            hilo = GetComponent<LineRenderer>();
            ariadna = false;
            smoothPath = false;
            firstHeuristic = true;
            pathVisited = 0;
            pathLength = 0;
            pathCost = 0;
            pathSearchTime = 0;
            pathPercentageTimeConsumed = 0;

            hilo.startWidth = 0.15f;
            hilo.endWidth = 0.15f;
            hilo.positionCount = 0;
            pathSpheres = new List<GameObject>();
        }

        public virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                updateAriadna(!ariadna);
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                updateSmooth(!smoothPath);
            }

            if (ariadna)
            {
                if (srcObj == null) srcObj = GameManager.instance.GetPlayer();
                if (dstObj == null) dstObj = GameManager.instance.GetExitNode();

                switch (algorithm)
                {
                    case TesterGraphAlgorithm.ASTAR:
                        if (firstHeuristic) path = graph.GetPathAstar(srcObj, dstObj, Euclidean);
                        else path = graph.GetPathAstar(srcObj, dstObj, Manhattan);
                        break;
                    default:
                    case TesterGraphAlgorithm.BFS:
                        path = graph.GetPathBFS(srcObj, dstObj);
                        break;
                    case TesterGraphAlgorithm.DFS:
                        path = graph.GetPathDFS(srcObj, dstObj);
                        break;
                }

                if (smoothPath)
                    path = graph.Smooth(path); // Suavizar el camino, una vez calculado

                if (path != null && path.Count > 0)
                {
                    UpdatePathInfo();
                    DrawThread();
                    DrawSpheres();
                }
            }
        }

        // Devuelve el grafo
        public Graph GetGraph()
        {
            return graph;
        }

        // Devuelve el siguiente nodo del camino
        public virtual Transform GetNextNode()
        {
            if (path != null && path.Count > 1)
                return path[1].transform;
            else if (path != null && path.Count == 1)
                return path[0].transform;

            return null;
        }

        // Actualiza la información del camino
        private void UpdatePathInfo()
        {
            if (pathLength == 0)
            {
                pathLength = path.Count - 1;
                GameManager.instance.UpdateLength(pathLength);
            }
            pathVisited = pathLength - path.Count + 1;
            GameManager.instance.UpdateVisited(pathVisited);
            if (pathCost == 0)
            {
                pathCost = graph.GetPathCost(path);
                GameManager.instance.UpdateCost(pathCost);
            }
            if (pathSearchTime == 0)
            {
                pathSearchTime = graph.GetPathSearchTime(path);
                GameManager.instance.UpdateSearchTime(pathSearchTime);
            }
            if (pathPercentageTimeConsumed == 0)
            {
                pathPercentageTimeConsumed = graph.GetPathSearchTimePercentage(path);
                GameManager.instance.UpdateSearchTimePercentage(pathPercentageTimeConsumed);
            }
        }

        // Dibuja el hilo de Ariadna
        public virtual void DrawThread()
        {
            hilo.positionCount = path.Count;

            Vector3 vertexInitialPos = new Vector3(srcObj.transform.position.x, srcObj.transform.position.y + hiloOffset, srcObj.transform.position.z);
            hilo.SetPosition(0, vertexInitialPos);

            for (int i = 1; i < path.Count; i++)
            {
                Vector3 vertexPos = new Vector3(path[i].transform.position.x,
                    path[i].transform.position.y + hiloOffset, path[i].transform.position.z);
                hilo.SetPosition(i, vertexPos);
                pathThreadMaterial.EnableKeyword("_EMISSION");
                pathThreadMaterial.SetColor("_EmissionColor", pathColor);
            }
        }

        // Dibujar esferas en el camino
        public void DrawSpheres()
        {
            if (spherePrefab == null)
                return;

            RemoveSpheres();

            foreach (Vertex pathVertex in path)
            {
                GameObject pathSphere = Instantiate(spherePrefab, pathVertex.transform.position, Quaternion.identity);
                pathSphere.transform.localScale = Vector3.one * pathNodeRadius;

                Renderer sphereRenderer = pathSphere.GetComponent<Renderer>();
                if (sphereRenderer != null) sphereRenderer.material.color = pathColor;
                else Debug.LogWarning("No se encontró componente Renderer en el prefab de la esfera");

                pathSpheres.Add(pathSphere);
            }
        }

        // Eliminar esferas del camino
        public void RemoveSpheres()
        {
            foreach (GameObject go in pathSpheres)
            {
                Destroy(go);
            }
            pathSpheres.Clear();
        }

        // Mostrar el camino calculado
        public void ShowPathVertices(List<Vertex> path, Color color)
        {
            for (int i = 0; i < path.Count; i++)
            {
                Vertex v = path[i];
                Renderer r = v.GetComponent<Renderer>();
                if (ReferenceEquals(r, null))
                    continue;
                r.material.color = color;
            }
        }

        // Dibujado de esferas y líneas en el editor
        virtual public void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            if (ReferenceEquals(graph, null))
                return;

            if (ReferenceEquals(path, null))
                return;

            Vertex v;
            if (!ReferenceEquals(srcObj, null))
            {
                Gizmos.color = Color.green; // Verde es el nodo inicial
                v = graph.GetNearestVertex(srcObj.transform.position);
                Gizmos.DrawSphere(v.transform.position, pathNodeRadius);
            }
            if (!ReferenceEquals(dstObj, null))
            {
                Gizmos.color = Color.red; // Rojo es el color del nodo de destino
                v = graph.GetNearestVertex(dstObj.transform.position);
                Gizmos.DrawSphere(v.transform.position, pathNodeRadius);
            }
            Gizmos.color = pathColor;
            for (int i = 0; i < path.Count; i++)
            {
                v = path[i];
                Gizmos.DrawSphere(v.transform.position, pathNodeRadius);
                if (smoothPath && i != 0)
                {
                    Vertex prev = path[i - 1];
                    Gizmos.DrawLine(v.transform.position, prev.transform.position);
                }
            }
        }

        // Actualiza el hilo de Ariadna, activando o desactivando el hilo y quitando las esferas
        void updateAriadna(bool ar)
        {
            ariadna = ar;
            hilo.enabled = ariadna;
            if (!ariadna)
            {
                pathVisited = 0;
                pathLength = 0;
                pathCost = 0;
                pathSearchTime = 0;
                GameManager.instance.UpdateVisited(0);
                GameManager.instance.UpdateLength(0);
                GameManager.instance.UpdateCost(0);
                GameManager.instance.UpdateSearchTime(0);
                RemoveSpheres();
            }
        }

        // Reinicia el camino
        public virtual void ResetPath()
        {
            path = null;
        }

        // Actualiza el coste de una celda
        public void UpdatePathCost(Vector3 position, float costMultipliyer)
        {
            graph.UpdateVertexCost(position, costMultipliyer);
        }

        void updateSmooth(bool smooth)
        {
            smoothPath = smooth;
            GameManager.instance.ChangeSmooth(smoothPath);
        }

        // Cambia la heurística entre Euclidea y Manhattan
        public string ChangeHeuristic()
        {
            firstHeuristic = !firstHeuristic;
            return firstHeuristic ? "Euclidean" : "Manhattan";
        }

        // Heurística Euclidea
        public float Euclidean(Vertex a, Vertex b)
        {
            return Vector3.Distance(a.transform.position, b.transform.position);
        }

        // Heurística Manhattan
        public float Manhattan(Vertex a, Vertex b)
        {
            Vector2 posA = a.transform.position;
            Vector2 posB = b.transform.position;
            return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
        }
    }
}
