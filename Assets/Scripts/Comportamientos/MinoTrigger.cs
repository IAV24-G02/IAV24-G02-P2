using UCM.IAV.Movimiento;
using UnityEngine;

public class MinoTrigger : MonoBehaviour
{
    [SerializeField]
    private float costMutipliyer = 5.0f;

    private void OnTriggerEnter(Collider other)
    {
        // Por Duck Typing, si el objeto tiene un componente Teseo
        // es porque es el Avatar. En ese caso, recalculamos el coste.
        if (other.gameObject.GetComponent<Teseo>() != null)
            GameManager.instance.UpdatePathCost(transform.position, costMutipliyer);       
    }

    private void OnTriggerExit(Collider other)
    {
        // Por Duck Typing, si el objeto tiene un componente Teseo
        // es porque es el Avatar. En ese caso, recalculamos el coste.
        if (other.gameObject.GetComponent<Teseo>() != null)
            GameManager.instance.UpdatePathCost(transform.position, 1.0f);
    }
}
