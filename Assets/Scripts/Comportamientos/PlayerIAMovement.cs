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

        //    //Resto de cálculo de movimiento
        //    direccion.lineal.Normalize();
        //    direccion.lineal *= agente.aceleracionMax;

        //    // Podríamos meter una rotación automática en la dirección del movimiento, si quisiéramos

        //    return direccion;
        //}

        //public void ResetPath()
        //{
        //    graph.ResetPath();
        //}


        ///// <summary>
        ///// Obtiene la dirección
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
