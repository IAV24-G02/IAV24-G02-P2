using UnityEngine;
using UCM.IAV.Navegacion;

namespace UCM.IAV.Movimiento
{
    /// <summary>
    /// Clase que gestiona la influencia de los minotauros en las baldosas del grafo
    /// </summary>
    public class MinoTrigger : MonoBehaviour
    {
        #region Variables
        /// <summary>
        /// Coste de la celda cuando el teseo está en el radio de detección
        /// </summary>
        [SerializeField]
        private float costMutipliyer = 5.0f;

        /// <summary>
        /// Radio de detección
        /// </summary>
        [SerializeField]
        private float detectionRadio = 5.0f;

        /// <summary>
        /// Esfera que representa el radio de detección
        /// </summary>
        private GameObject sphere;

        /// <summary>
        /// Collider esférico para detectar las baldosas
        /// </summary>
        private SphereCollider sphereCollider;
        #endregion

        private void Start()
        {
            sphere = transform.Find("Sphere").gameObject;
            if (sphere != null)
            {
                sphere.transform.localScale = Vector3.one * detectionRadio;
                sphereCollider = sphere.GetComponent<SphereCollider>();
                if (sphereCollider == null)
                    sphereCollider = sphere.AddComponent<SphereCollider>();
                sphereCollider.isTrigger = true;
            }
            else Debug.LogError("No se ha encontrado la esfera de detección");
        }

        private void OnTriggerEnter(Collider other)
        {
            // Por Duck Typing, si el objeto con el que colisiona es una baldosa, cambiamos su coste y color
            if (!ReferenceEquals(other.GetComponent<Vertex>(), null))
                other.GetComponent<Vertex>().SetInfluence(true, costMutipliyer);
        }

        private void OnTriggerExit(Collider other)
        {
            // Por Duck Typing, si el objeto con el que colisiona es una baldosa, cambiamos su coste y color
            if (!ReferenceEquals(other.GetComponent<Vertex>(), null))
                other.GetComponent<Vertex>().SetInfluence(false);
        }
    }
}