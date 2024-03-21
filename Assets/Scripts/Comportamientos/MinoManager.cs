/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using System.Collections.Generic;
using UCM.IAV.Movimiento;
using UnityEngine;

namespace UCM.IAV.Navegacion
{
    /// <summary>
    /// Clase que gestiona la creación de los minotauros
    /// </summary>
    public class MinoManager : MonoBehaviour
    {
        #region Variables
        /// <summary>
        /// Prefab del minotauro
        /// </summary>
        [SerializeField]
        private GameObject minotaurPrefab;

        /// <summary>
        /// False: Genera minotauros de forma aleatoria
        /// True: Genera minotauros en posiciones fijas a partir de una posición de un GameObject
        /// </summary>
        [SerializeField]
        private bool debug = false;

        /// <summary>
        /// Posición de los minotauros para el modo debug
        /// </summary>
        [SerializeField]
        private GameObject minoPos;

        /// <summary>
        /// Lista de minotauros
        /// </summary>
        private List<GameObject> minos;

        /// <summary>
        /// Grafo de la escena
        /// </summary>
        private Graph graph;

        /// <summary>
        /// Número de minotauros
        /// </summary>
        private int numMinos = 1;
        #endregion

        void Start()
        {
            numMinos = GameManager.instance.getNumMinos();
            minos = new List<GameObject>();
            StartUp();
        }

        /// <summary>
        /// Inicializa la lista de minotauros y el grafo
        /// </summary>
        void StartUp()
        {
            GameObject graphGO = GameObject.Find("GraphGrid");

            if (graphGO != null)
                graph = graphGO.GetComponent<GraphGrid>();

            for (int i = 0; i < numMinos; i++)
                minos.Add(GenerateMino());
        }

        /// <summary>
        /// Genera un minotauro dependiendo si estamos en modo debug o no
        /// Si no, lo genera en una posición aleatoria del grafo
        /// </summary>
        private GameObject GenerateMino()
        {
            if (debug)
                return Instantiate(minotaurPrefab, minoPos.transform.position, Quaternion.identity);
            else
                return Instantiate(minotaurPrefab, graph.GetRandomPos().transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
        }
    }
}
