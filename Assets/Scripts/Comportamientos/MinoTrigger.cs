using UnityEngine;
using UCM.IAV.Navegacion;

namespace UCM.IAV.Movimiento
{
    public class MinoTrigger : MonoBehaviour
    {
        [SerializeField]
        private float costMutipliyer = 5.0f;    // Coste de la celda cuando el teseo está en el radio de detección

        [SerializeField]
        private float detectionRadio = 5.0f;    // Radio de detección

        private GameObject sphere;              // Esfera que representa el radio de detección
        private SphereCollider sphereCollider;  // Collider esférico para detectar al teseo

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
            {
                other.GetComponent<Vertex>().SetInfluence(true, costMutipliyer);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Por Duck Typing, si el objeto con el que colisiona es una baldosa, cambiamos su coste y color
            if (!ReferenceEquals(other.GetComponent<Vertex>(), null))
            {
                other.GetComponent<Vertex>().SetInfluence(false);
            }
        }
    }
}