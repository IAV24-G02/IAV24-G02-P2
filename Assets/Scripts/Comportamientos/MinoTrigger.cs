using UnityEngine;
using UCM.IAV.Navegacion;

namespace UCM.IAV.Movimiento
{
    public class MinoTrigger : MonoBehaviour
    {
        [SerializeField]
        private float costMutipliyer = 5.0f;    // Coste de la celda cuando el teseo est� en el radio de detecci�n

        [SerializeField]
        private float detectionRadio = 5.0f;    // Radio de detecci�n

        private GameObject sphere;              // Esfera que representa el radio de detecci�n
        private SphereCollider sphereCollider;  // Collider esf�rico para detectar al teseo

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
            else Debug.LogError("No se ha encontrado la esfera de detecci�n");
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