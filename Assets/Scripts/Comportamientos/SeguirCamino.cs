/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Inform�tica de la Universidad Complutense de Madrid (Espa�a).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
namespace UCM.IAV.Movimiento
{
    using UCM.IAV.Navegacion;
    using UnityEngine;

    public class SeguirCamino: ComportamientoAgente
    {
        Transform sigNodo;

        public TheseusGraph graph;

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

        public void ResetPath()
        {
            graph.ResetPath();
        }
    }
}