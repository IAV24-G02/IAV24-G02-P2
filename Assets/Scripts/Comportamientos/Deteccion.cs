using UnityEngine;
using static UnityEngine.UI.Image;

namespace UCM.IAV.Movimiento
{
    /// <summary>
    /// Clase para modelar el comportamiento de DETECCION de otro agente
    /// </summary>
    public class Deteccion : MonoBehaviour
    {
        private Merodear merodear;              // Componente de merodear
        private Llegada llegada;                // Componente de llegada

        Vector3[] positionsInCone;

        [SerializeField]
        float coneAngle = 45f; // Ángulo del cono en grados

        [SerializeField]
        float coneDistance = 5f; // Distancia del cono

        [SerializeField]
        Transform objetivo;

        private GameObject avatarGO;

        private void Awake()
        {
            merodear = GetComponent<Merodear>();
            if (merodear != null)
                merodear.enabled = true;
            else Debug.LogError("No se ha encontrado el componente Merodear");
            llegada = GetComponent<Llegada>();
            if (llegada != null)
                llegada.enabled = false;
            else Debug.LogError("No se ha encontrado el componente Llegada");

            avatarGO = GameObject.Find("Avatar");
            objetivo = avatarGO.transform;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!ReferenceEquals(other.gameObject.GetComponent<Teseo>(), null))
            {
                Persigue(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!ReferenceEquals(other.gameObject.GetComponent<Teseo>(), null))
            {
                Merodea();
            }
        }

        private void Update()
        { 
            Vector3 directionToPlayer = objetivo.position - transform.position;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if ((directionToPlayer.magnitude <= coneDistance) && (angleToPlayer <= coneAngle * 0.5f))
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, coneDistance))
                {
                    // Se ha detectado una colisión en la capa especificada
                    // Verificar si el objeto golpeado tiene el componente requerido
                    if (!ReferenceEquals(hit.collider.gameObject.GetComponent<Teseo>(), null))
                    {
                        // El objeto golpeado tiene el componente requerido
                        Debug.Log("RayCast hit: Persigue");
                        Persigue(hit.collider);
                    }
                    else
                    {
                        Merodea();
                    }

                }
            }
        }

        private void Merodea()
        {
            merodear.enabled = true;
            GetComponent<Merodear>().ResetPosition();
            llegada.enabled = false;
        }

        private void Persigue(Collider other)
        {
            merodear.enabled = false;
            llegada.enabled = true;
            llegada.objetivo = other.gameObject;
        }

        //Vector3[] CalculatePositionsInCone()
        //{
        //    int numberOfPositions = 10; // Número de posiciones a calcular en el cono


        //    Vector3[] positions = new Vector3[numberOfPositions];

        //    // Calcular el ángulo de separación entre cada posición
        //    float angleStep = coneAngle / (numberOfPositions - 1);

        //    // Obtener la dirección del cono
        //    Vector3 coneDirection = transform.forward;

        //    // Calcular la matriz de rotación que representa la rotación del cono
        //    Quaternion coneRotation = Quaternion.AngleAxis(coneAngle * 0.5f, transform.up);

        //    // Calcular las posiciones en el cono
        //    for (int i = 0; i < numberOfPositions; i++)
        //    {
        //        // Calcular la dirección de esta posición en el cono
        //        float currentAngle = -coneAngle * 0.5f + angleStep * i;
        //        Quaternion rotation = Quaternion.AngleAxis(currentAngle, transform.up);
        //        Vector3 direction = rotation * coneDirection;

        //        // Calcular la posición correspondiente a esta dirección
        //        Vector3 position = transform.position + direction * coneDistance;

        //        // Almacenar la posición en el array
        //        positions[i] = position;
        //    }

        //    return positions;
        //}

        //void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.yellow;

        //    // Dibujar el cono
        //    float halfAngleRad = coneAngle * 0.5f * Mathf.Deg2Rad;
        //    Quaternion coneRotation = Quaternion.AngleAxis(coneAngle * 0.5f, transform.up);
        //    Vector3 coneWidth = coneRotation * transform.right * coneDistance * Mathf.Tan(halfAngleRad);

        //    Vector3 coneStart = transform.position + transform.forward * coneDistance;
        //    Vector3 coneEndLeft = coneStart + coneWidth;
        //    Vector3 coneEndRight = coneStart - coneWidth;

        //    Gizmos.DrawLine(transform.position, coneStart);
        //    Gizmos.DrawLine(coneStart, coneEndLeft);
        //    Gizmos.DrawLine(coneStart, coneEndRight);

        //    // Dibujar las líneas que conectan el vértice del cono con las posiciones calculadas
        //    //Vector3[] positionsInCone = CalculatePositionsInCone();
        //    foreach (Vector3 position in positionsInCone)
        //    {
        //        Gizmos.DrawLine(transform.position, position);
        //    }
        //}
    }
}
