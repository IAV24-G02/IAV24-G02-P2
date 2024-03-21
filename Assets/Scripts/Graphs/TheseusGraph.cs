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
    /// <summary>
    /// Enumerado de algoritmos de búsqueda
    /// </summary>
    public enum TesterGraphAlgorithm
    {
        BFS, DFS, ASTAR
    }

    /// <summary>
    /// Clase para modelar el grafo de nodos de la escena
    /// </summary>
    public class TheseusGraph : MonoBehaviour
    {
        #region Variables
        /// <summary>
        /// Grafo de nodos
        /// </summary>
        [SerializeField]
        protected Graph graph;

        /// <summary>
        /// Algoritmo a usar
        /// </summary>
        [SerializeField]
        private TesterGraphAlgorithm algorithm;

        /// <summary>
        /// Prefab de esfera
        /// </summary>
        [SerializeField]
        private GameObject spherePrefab;

        /// <summary>
        /// Esferas del camino
        /// </summary>
        List<GameObject> pathSpheres;

        /// <summary>
        /// Material del camino
        /// </summary>
        [SerializeField]
        private Material pathThreadMaterial;

        /// <summary>
        /// Color del camino
        /// </summary>
        [SerializeField]
        private Color pathColor;

        /// <summary>
        /// Radio de los nodos del camino
        /// </summary>
        [SerializeField]
        [Range(0.1f, 1f)]
        private float pathNodeRadius = .3f;

        /// <summary>
        /// Activa o desactiva el hilo de Ariadna
        /// </summary>
        private bool ariadna;

        /// <summary>
        /// Suavizar el camino
        /// </summary>
        private bool smoothPath;

        /// <summary>
        /// Indica si se está usando la primera heurística
        /// </summary>
        private bool firstHeuristic;

        /// <summary>
        /// Objeto origen
        /// </summary>
        protected GameObject srcObj;

        /// <summary>
        /// Objeto destino
        /// </summary>
        protected GameObject dstObj;

        /// <summary>
        /// Camino calculado
        /// </summary>
        protected List<Vertex> path;

        /// <summary>
        /// Nodos visitados
        /// </summary>
        private int pathVisited;

        /// <summary>
        /// Longitud del camino
        /// </summary>
        private int pathLength;

        /// <summary>
        /// Coste del camino
        /// </summary>
        private float pathCost;

        /// <summary>
        /// Tiempo de búsqueda
        /// </summary>
        private float pathSearchTime;

        /// <summary>
        /// Porcentaje de tiempo consumido en la búsqueda
        /// </summary>
        private float pathPercentageTimeConsumed;

        /// <summary>
        /// Hilo de Ariadna
        /// </summary>
        protected LineRenderer hilo;

        /// <summary>
        /// Offset del hilo
        /// </summary>
        protected float hiloOffset = 0.2f;
        #endregion

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

        /// <summary>
        /// Devuelve el grafo
        /// </summary>
        /// <returns></returns>
        public Graph GetGraph()
        {
            return graph;
        }

        /// <summary>
        /// Devuelve el siguiente nodo del camino
        /// </summary>
        /// <returns></returns>
        public virtual Transform GetNextNode()
        {
            if (path != null && path.Count > 1)
                return path[1].transform;
            else if (path != null && path.Count == 1)
                return path[0].transform;

            return null;
        }

        /// <summary>
        /// Actualiza la información del camino
        /// </summary>
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

        /// <summary>
        /// Dibuja el hilo de Ariadna
        /// </summary>
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

        /// <summary>
        /// Dibujar esferas en el camino
        /// </summary>
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

        /// <summary>
        /// Eliminar esferas del camino
        /// </summary>
        public void RemoveSpheres()
        {
            foreach (GameObject go in pathSpheres)
            {
                Destroy(go);
            }
            pathSpheres.Clear();
        }

        /// <summary>
        /// Mostrar el camino calculado
        /// </summary>
        /// <param name="path"></param>
        /// <param name="color"></param>
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

        /// <summary>
        /// Dibujado de esferas y líneas en el editor
        /// </summary>
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

        /// <summary>
        /// Actualiza el hilo de Ariadna, activando o desactivando el hilo 
        /// y quitando las esferas
        /// </summary>
        /// <param name="ar"></param>
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

        /// <summary>
        /// Reinicia el camino
        /// </summary>
        public virtual void ResetPath()
        {
            path = null;
        }

        /// <summary>
        /// Actualiza el coste de una celda
        /// </summary>
        /// <param name="position"></param>
        /// <param name="costMultipliyer"></param>
        public void UpdatePathCost(Vector3 position, float costMultipliyer)
        {
            graph.UpdateVertexCost(position, costMultipliyer);
        }

        /// <summary>
        /// Activa/Desactiva el suavizado del camino
        /// </summary>
        void updateSmooth(bool smooth)
        {
            smoothPath = smooth;
            GameManager.instance.ChangeSmooth(smoothPath);
        }

        /// <summary>
        /// Cambia la heurística entre Euclidea y Manhattan
        /// </summary>
        /// <returns></returns>
        public string ChangeHeuristic()
        {
            firstHeuristic = !firstHeuristic;
            return firstHeuristic ? "Euclidean" : "Manhattan";
        }

        /// <summary>
        /// Heurística Euclidea
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public float Euclidean(Vertex a, Vertex b)
        {
            return Vector3.Distance(a.transform.position, b.transform.position);
        }

        /// <summary>
        /// Heurística Manhattan
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public float Manhattan(Vertex a, Vertex b)
        {
            Vector2 posA = a.transform.position;
            Vector2 posB = b.transform.position;
            return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
        }
    }
}
