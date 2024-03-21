/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Clase para controlar el comportamiento de los dropdowns
/// </summary>
public class DropDown : MonoBehaviour
{
    /// <summary>
    /// Si es un dropdown de minos
    /// </summary>
    [SerializeField]
    private bool mino = false;

    void Start()
    {
        // Establece changeSize al OnValueChanged del Dropdown
        if (!mino)
            gameObject.GetComponent<Dropdown>().onValueChanged.AddListener(delegate { UCM.IAV.Movimiento.GameManager.instance.ChangeMazeSize(); });
        else
            gameObject.GetComponent<Dropdown>().onValueChanged.AddListener(delegate { UCM.IAV.Movimiento.GameManager.instance.setNumMinos(); });
    }
}
