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


    // Posibles algoritmos para buscar caminos en grafos
    // REALMENTE PARA ESTA PRÁCTICA SÓLO SE NECESITA ASTAR, los otros no los usaremos...
    public enum TesterGraphAlgorithm
    {
        BFS, DFS, ASTAR
    }

    public class TheseusGraph : MonoBehaviour
    {
        [SerializeField]
        protected Graph graph;                  // Grafo de nodos

        [SerializeField]
        private TesterGraphAlgorithm algorithm; // Algoritmo a usar

        [SerializeField]
        private bool smoothPath;                // Suavizar el camino

        [SerializeField]
        private Color pathColor;                // Color del camino

        [SerializeField]
        [Range(0.1f, 1f)]
        private float pathNodeRadius = .3f;     // Radio de los nodos del camino

        private bool ariadna;                   // Activa o desactiva el hilo de Ariadna

        bool firstHeuristic = true;             // Indica si se está usando la primera heurística
        protected GameObject srcObj;            // Objeto origen
        protected GameObject dstObj;            // Objeto destino
        protected List<Vertex> path;            // Camino calculado

        protected LineRenderer hilo;            // Hilo de Ariadna
        protected float hiloOffset = 0.2f;      // Offset del hilo

        public virtual void Awake()
        {
            srcObj = GameManager.instance.GetPlayer();
            dstObj = null;
            path = null;
            hilo = GetComponent<LineRenderer>();
            ariadna = false;

            hilo.startWidth = 0.15f;
            hilo.endWidth = 0.15f;
            hilo.positionCount = 0;
        }

        public virtual void Update()
        {
            updateAriadna(Input.GetKeyDown(KeyCode.Mouse1));

            if (Input.GetKeyDown(KeyCode.S))
                smoothPath = !smoothPath;

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
                    //GameManager.instance.SetPlayerNode(path[path.Count - 1].transform);

                    DibujaHilo();
                }
            }
        }

        public virtual Transform GetNextNode()
        {
            if (path != null && path.Count > 0)
                return path[path.Count - 1].transform;

            return null;
        }

        // Dibujado de artilugios en el editor
        // OJO, ESTO SÓLO SE PUEDE VER EN LA PESTAÑA DE SCENE DE UNITY
        virtual public void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            if (ReferenceEquals(graph, null))
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
            int i;
            Gizmos.color = pathColor;
            for (i = 0; i < path.Count; i++)
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

        // Mostrar el camino calculado
        public void ShowPathVertices(List<Vertex> path, Color color)
        {
            int i;
            for (i = 0; i < path.Count; i++)
            {
                Vertex v = path[i];
                Renderer r = v.GetComponent<Renderer>();
                if (ReferenceEquals(r, null))
                    continue;
                r.material.color = color;
            }
        }

        // Dibuja el hilo de Ariadna
        public virtual void DibujaHilo()
        {
            hilo.positionCount = path.Count + 1;
            hilo.SetPosition(0, new Vector3(srcObj.transform.position.x, srcObj.transform.position.y + hiloOffset, srcObj.transform.position.z));

            for (int i = path.Count - 1; i >= 0; i--)
            {
                Vector3 vertexPos = new Vector3(path[i].transform.position.x, path[i].transform.position.y + hiloOffset, path[i].transform.position.z);
                hilo.SetPosition(path.Count - i, vertexPos);
            }
        }

        void updateAriadna(bool ar)
        {
            ariadna = ar;
            hilo.enabled = ariadna;
        }

        public string ChangeHeuristic()
        {
            firstHeuristic = !firstHeuristic;
            return firstHeuristic ? "Euclidea" : "Manhattan";
        }

        public virtual void ResetPath()
        {
            path = null;
        }

        // Heurísticas
        public float Euclidean(Vertex a, Vertex b)
        {
            return Vector3.Distance(a.transform.position, b.transform.position);
        }
        public float Manhattan(Vertex a, Vertex b)
        {
            Vector2 posA = a.transform.position;
            Vector2 posB = b.transform.position;
            return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
        }
    }
}
