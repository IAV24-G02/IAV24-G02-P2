using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Navegacion;
using UnityEngine;



namespace UCM.IAV.Movimiento
{
    /// <summary>
    /// Clase para modelar el comportamiento de SEGUIR a otro agente
    /// </summary>
    public class PlayerIAMovement : MonoBehaviour /* : ComportamientoAgente*/
    {
        //Transform sigNodo;

        //public Graph graph;

        //override public void Update()
        //{
        //   // sigNodo = graph.GetNextNode();
        //    base.Update();
        //}

        //public override Direccion GetDireccion()
        //{
        //    Direccion direccion = new Direccion();

        //    if (sigNodo != null)
        //    {
        //        //Direccion actual
        //        direccion.lineal = sigNodo.position - transform.position;
        //    }
        //    else
        //    {
        //        direccion.lineal = new Vector3(0, 0, 0);
        //    }

        //    //Resto de c�lculo de movimiento
        //    direccion.lineal.Normalize();
        //    direccion.lineal *= agente.aceleracionMax;

        //    // Podr�amos meter una rotaci�n autom�tica en la direcci�n del movimiento, si quisi�ramos

        //    return direccion;
        //}

        //public void ResetPath()
        //{
        //    graph.ResetPath();
        //}


        ///// <summary>
        ///// Obtiene la direcci�n
        ///// </summary>
        ///// <returns></returns>
        ///// 
        //public override Direccion GetDireccion()
        //{
        //    Direccion direccion = new Direccion();

            
        //    return direccion;
        //} 
    }
}
