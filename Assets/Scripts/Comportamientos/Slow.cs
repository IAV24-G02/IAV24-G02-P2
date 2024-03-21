/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using UnityEngine;
using UCM.IAV.Movimiento;

namespace UCM.IAV.Navegacion
{
    /// <summary>
    /// Clase que modela el comportamiento de ralentizar al jugador al entrar en contacto con el objeto
    /// </summary>
    public class Slow : MonoBehaviour
    {
        #region Variables
        /// <summary>
        /// Velocidad del jugador
        /// </summary>
        private float vel = 0.0f;
        #endregion
    
        private void OnTriggerEnter(Collider other)
        {
            ControlJugador animator = other.gameObject.GetComponent<ControlJugador>();
            if(!ReferenceEquals(animator, null))
            {
                Agente agent = other.gameObject.GetComponent<Agente>();
                vel = agent.velocidadMax;
                agent.velocidadMax = 1;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            ControlJugador animator = other.gameObject.GetComponent<ControlJugador>();
            if (!ReferenceEquals(animator, null))
            {
                Agente agent = other.gameObject.GetComponent<Agente>();
                agent.velocidadMax = vel;
            }
        }
    }
}
