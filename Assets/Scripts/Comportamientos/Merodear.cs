using UnityEngine;
using UCM.IAV.Navegacion;

namespace UCM.IAV.Movimiento
{
    /// <summary>
    /// Clase para modelar el comportamiento de WANDER a otro agente
    /// </summary>
    public class Merodear : ComportamientoAgente
    {
        [SerializeField]
        public float offset = 0.2f;         // Distancia para considerar que ha llegado a la posición objetivo

        [SerializeField]
        private float waitingTime = 0.2f;   // Tiempo de espera

        private Graph graph;                // Grafo de la escena
        private Vertex lastVertex;          // Último vértice visitado
        private Vertex actualVertex;        // Vértice actual
        private Vector3 nextPosition;       // Posición objetivo
        private bool isWaiting;             // Indica si el agente parado o no
        private float counterTime;          // Contador de tiempo

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

                    // Si el agente está en un callejón sin salida
                    if (neighbours.Length == 1)
                    {
                        lastVertex = actualVertex;
                        actualVertex = neighbours[0];
                    }
                    // Si no, se evita el vértice anterior
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

                // Se dirige hacia la posición objetivo
                direccion.lineal = nextPosition - newPos;
                direccion.lineal.Normalize();
                direccion.lineal *= agente.velocidadMax;
            }
            return direccion;
        }

        // Devuelve la posición sin tener en cuenta la altura
        private Vector3 GetPositionWithoutHeight()
        {
            return new Vector3(transform.position.x, 0, transform.position.z);
        }

        // Reinicia la posición objetivo
        public void ResetPosition()
        {
            nextPosition = GetPositionWithoutHeight();
        }
    }
}
