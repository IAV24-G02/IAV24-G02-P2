using UnityEngine;

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
        float coneAngle = 45f; // �ngulo del cono en grados

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
                Persigue(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!ReferenceEquals(other.gameObject.GetComponent<Teseo>(), null))
                Merodea();
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
                    // Se ha detectado una colisi�n en la capa especificada
                    // Verificar si el objeto golpeado tiene el componente requerido
                    if (!ReferenceEquals(hit.collider.gameObject.GetComponent<Teseo>(), null))
                        Persigue(hit.collider);
                    else
                        Merodea();

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
    }
}
