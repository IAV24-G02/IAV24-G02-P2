using UCM.IAV.Movimiento;
using UnityEngine;

public class Deteccion : MonoBehaviour
{
    [SerializeField]
    private float costMutipliyer = 5.0f;    // Coste de la celda cuando el teseo está en el radio de detección

    [SerializeField]
    private float detectionRadio = 5.0f;    // Radio de detección

    private Merodear merodear;              // Componente de merodear
    private Llegada llegada;                // Componente de llegada
    private GameObject sphere;              // Esfera que representa el radio de detección
    private SphereCollider sphereCollider;  // Collider esférico para detectar al teseo

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
        if (!ReferenceEquals(other.gameObject.GetComponent<Teseo>(), null))
        {
            merodear.enabled = false;
            llegada.enabled = true;
            llegada.objetivo = other.gameObject;

            // Actualiza el coste de la celda de la celda a un valor costMutipliyer veces mayor
            GameManager.instance.UpdatePathCost(transform.position, costMutipliyer);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!ReferenceEquals(other.gameObject.GetComponent<Teseo>(), null))
        {
            merodear.enabled = true;
            llegada.enabled = false;

            // Actualiza el coste de la celda de vuelva a su valor original
            GameManager.instance.UpdatePathCost(transform.position, 1.0f);
        }
    }
}
