using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Movimiento;
using UnityEngine;

public class Deteccion : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!ReferenceEquals(other.gameObject.GetComponent<Teseo>(), null))
        {
            Merodear merodeo = GetComponentInParent<Merodear>();
            Llegada llegar = GetComponentInParent<Llegada>();
            merodeo.enabled = false;
            llegar.enabled = true;
            llegar.objetivo = other.gameObject;
            Debug.Log("entra detec");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!ReferenceEquals(other.gameObject.GetComponent<Teseo>(), null))
        {
            Merodear merodeo = GetComponentInParent<Merodear>();
            Llegada llegar = GetComponentInParent<Llegada>();
            merodeo.enabled = true;
            llegar.enabled = false;
            Debug.Log("sale detec");
        }
    }
}
