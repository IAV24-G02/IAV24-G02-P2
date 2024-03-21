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
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!ReferenceEquals(other.gameObject.GetComponent<Teseo>(), null))
            {
                merodear.enabled = false;
                llegada.enabled = true;
                llegada.objetivo = other.gameObject;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!ReferenceEquals(other.gameObject.GetComponent<Teseo>(), null))
            {
                merodear.enabled = true;
                GetComponent<Merodear>().ResetPosition();
                llegada.enabled = false;
            }
        }
    }
}
