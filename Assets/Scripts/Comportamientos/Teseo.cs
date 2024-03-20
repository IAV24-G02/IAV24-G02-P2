/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using UnityEngine;

namespace UCM.IAV.Movimiento
{
    /// <summary>
    /// Clase para controlar el comportamiento del teseo
    /// </summary>
    public class Teseo : MonoBehaviour
    {
        private bool ariadna;           // Variable para controlar si hay que seguir el camino o no
        private SeguirCamino segCam;    // Componente de SeguirCamino
        private ControlJugador contJug; // Componente de ControlJugador

        void Start()
        {
            ariadna = false;
            segCam = GetComponent<SeguirCamino>();
            contJug = GetComponent<ControlJugador>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                updateAriadna(!ariadna);
            }
        }

        // Actualiza el comportamiento del teseo
        public void updateAriadna(bool ar)
        {
            ariadna = ar;
            segCam.enabled = ariadna;
            contJug.enabled = !ariadna;
        }
    }
}
