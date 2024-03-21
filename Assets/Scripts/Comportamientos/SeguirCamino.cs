/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using UCM.IAV.Navegacion;
using UnityEngine;

namespace UCM.IAV.Movimiento
{
    /// <summary>
    /// Clase para modelar el comportamiento de SEGUIR un CAMINO
    /// </summary>
    public class SeguirCamino: ComportamientoAgente
    {
        #region Variables
        /// <summary>
        /// Componente Transform del siguiente nodo
        /// </summary>
        private Transform sigNodo;

        /// <summary>
        /// Grafo de la escena
        /// </summary>
        public TheseusGraph graph;
        #endregion

        override public void Update()
        {
            sigNodo = graph.GetNextNode();
            base.Update();
        }

        public override Direccion GetDireccion()
        {
            Direccion direccion = new Direccion();

            if (sigNodo != null)
            {
                direccion.lineal = sigNodo.position - transform.position;
            }
            else
            {
                direccion.lineal = new Vector3(0, 0, 0);
            }

            direccion.lineal.Normalize();
            direccion.lineal *= agente.aceleracionMax;

            return direccion;
        }

        /// <summary>
        /// Resetea el camino
        /// </summary>
        public void ResetPath()
        {
            graph.ResetPath();
        }
    }
}