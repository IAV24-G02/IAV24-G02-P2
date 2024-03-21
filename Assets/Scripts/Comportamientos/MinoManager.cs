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
    public class MinoManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject minotaurPrefab;  // Prefab del minotauro

        [SerializeField]
        private bool debug = false;         // False: Genera minotauros de forma aleatoria,
                                            // True: Genera minotauros en posiciones fijas a partir de una posición de un GameObject
        [SerializeField]
        private GameObject minoPos;         // Posición de los minotauros para el modo debug

        public List<GameObject> minos;      // Lista de minotauros
        private Graph graph;                // Grafo de la escena
        private int numMinos = 1;           // Número de minotauros

        void Start()
        {
            numMinos = GameManager.instance.getNumMinos();
            minos = new List<GameObject>();
            StartUp();
        }

        void StartUp()
        {
            GameObject graphGO = GameObject.Find("GraphGrid");

            if (graphGO != null)
                graph = graphGO.GetComponent<GraphGrid>();

            for (int i = 0; i < numMinos; i++)
                minos.Add(GenerateMino());
        }

        private GameObject GenerateMino()
        {
            if (debug)
                return Instantiate(minotaurPrefab, minoPos.transform.position, Quaternion.identity);
            else
                return Instantiate(minotaurPrefab, graph.GetRandomPos().transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
        }
    }
}
