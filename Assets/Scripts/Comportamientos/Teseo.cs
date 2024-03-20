/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCM.IAV.Movimiento
{
    public class Teseo : MonoBehaviour
    {

        bool ariadna = false;

        SeguirCamino segCam;
        ControlJugador contJug;

        void Start()
        {
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

        public void updateAriadna(bool ar)
        {
            ariadna = ar;
            segCam.enabled = ariadna;
            contJug.enabled = !ariadna;
        }
    }
}
