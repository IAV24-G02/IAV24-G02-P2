using UnityEngine;
using UCM.IAV.Navegacion;

namespace UCM.IAV.Movimiento
{
    /// <summary>
    /// Clase para modelar el comportamiento de WANDER a otro agente
    /// </summary>
    public class Merodear : ComportamientoAgente
    {
        #region Variables
        /// <summary>
        /// Distancia para considerar que ha llegado a la posici�n objetivo
        /// </summary>
        [SerializeField]
        public float offset = 0.2f;

        /// <summary>
        /// Tiempo de espera
        /// </summary>
        [SerializeField]
        private float waitingTime = 0.2f;

        /// <summary>
        /// Grafo de la escena
        /// </summary>
        private Graph graph;

        /// <summary>
        /// �ltimo v�rtice visitado
        /// </summary>
        private Vertex lastVertex;

        /// <summary>
        /// V�rtice actual
        /// </summary>
        private Vertex actualVertex;

        /// <summary>
        /// Posici�n objetivo
        /// </summary>
        private Vector3 nextPosition;

        /// <summary>
        /// Indica si el agente parado o no
        /// </summary>
        private bool isWaiting;

        /// <summary>
        /// Contador de tiempo
        /// </summary>
        private float counterTime;
        #endregion

        private void Start()
        {
            graph = GameManager.instance.GetGraph();
            lastVertex = new Vertex();
            actualVertex = graph.GetNearestVertex(transform.position);
            nextPosition = GetPositionWithoutHeight();
            isWaiting = false;
            counterTime = 0.0f;
        }

        public override Direccion GetDireccion()
        {
            Direccion direccion = new Direccion();

            // No se mueve por estar esperando
            if (isWaiting)
            {
                counterTime += Time.deltaTime;
                if (counterTime > waitingTime)
                {
                    isWaiting = false;
                    counterTime = 0;
                }
            }
            else
            {
                Vector3 newPos = GetPositionWithoutHeight();

                if ((nextPosition - newPos).magnitude <= offset)
                {
                    Vertex[] neighbours = graph.GetNeighbours(graph.GetNearestVertex(transform.position));

                    // Si el agente est� en un callej�n sin salida
                    if (neighbours.Length == 1)
                    {
                        lastVertex = actualVertex;
                        actualVertex = neighbours[0];
                    }
                    // Si no, se evita el v�rtice anterior
                    else
                    {
                        Vertex v = neighbours[Random.Range(0, neighbours.Length)];
                        while (v.id == lastVertex.id)
                            v = neighbours[Random.Range(0, neighbours.Length)];
                        lastVertex = actualVertex;
                        actualVertex = v;
                    }

                    nextPosition = actualVertex.GetComponent<Transform>().position;
                    nextPosition.y = 0; // No se tiene en cuenta la altura

                    agente.velocidad = Vector3.zero;
                    isWaiting = true; // Se pone a esperar
                }

                // Se dirige hacia la posici�n objetivo
                direccion.lineal = nextPosition - newPos;
                direccion.lineal.Normalize();
                direccion.lineal *= agente.velocidadMax;
            }
            return direccion;
        }

        /// <summary>
        /// Devuelve la posici�n sin tener en cuenta la altura
        /// </summary>
        /// <returns></returns>
        private Vector3 GetPositionWithoutHeight()
        {
            return new Vector3(transform.position.x, 0, transform.position.z);
        }

        /// <summary>
        /// Reinicia la posici�n objetivo
        /// </summary>
        public void ResetPosition()
        {
            nextPosition = GetPositionWithoutHeight();
        }
    }
}
