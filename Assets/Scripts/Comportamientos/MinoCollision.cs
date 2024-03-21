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
    public class MinoCollision : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            // Por Duck Typing, si el objeto con el que colisiona es el Avatar, reiniciamos la escena
            if (!ReferenceEquals(collision.gameObject.GetComponent<Teseo>(), null))
                GameManager.instance.RestartScene();
        }
    }
}