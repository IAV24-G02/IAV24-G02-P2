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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UCM.IAV.Movimiento
{
    /// <summary>
    /// Gestor del juego
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;  // Singleton

        private string mazeSize = "10x10";          // Tamaño del laberinto
        private int frameRate = 60;                 // Framerate
        private TheseusGraph theseusGraph;          // Grafo del laberinto
        private int numMinos = 1;                   // Número de minotauros

        private GameObject player = null;           // Jugador
        private GameObject startSlab = null;        // Baldosa de inicio
        private GameObject exitSlab = null;         // Baldosa de salida
        private GameObject exit = null;             // Nodo de salida

        // Textos UI
        private Text fRText;                        // Texto de framerate
        private Text heuristicText;                 // Texto de la heurística
        private Text label;                         // Texto del tamaño del laberinto en el menú
        private Text label2;                        // Texto del número de minotauros en el menú
        private Text smoothText;                    // Texto de suavizado del camino
        private Text mazeSizeText;                  // Texto del tamaño del laberinto en el laberinto
        private Text visitedText;                   // Texto de baldosas visitadas
        private Text lengthText;                    // Texto de longitud del camino
        private Text costText;                      // Texto de coste del camino
        private Text searchTimeText;                // Texto de tiempo de búsqueda
        private Text percentageTimeText;            // Texto de porcentaje de tiempo de búsqueda

        // Variables de timer de framerate
        private int m_frameCounter = 0;
        private float m_timeCounter = 0.0f;
        private float m_lastFramerate = 0.0f;
        private float m_refreshTime = 0.5f;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            Application.targetFrameRate = frameRate;
            FindGO();
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            FindGO();
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void Update()
        {
            // Timer para mostrar el frameRate a intervalos
            if (m_timeCounter < m_refreshTime)
            {
                m_timeCounter += Time.deltaTime;
                m_frameCounter++;
            }
            else
            {
                m_lastFramerate = (float)m_frameCounter / m_timeCounter;
                m_frameCounter = 0;
                m_timeCounter = 0.0f;
            }

            // Texto con el framerate y 2 decimales
            if (fRText != null)
                fRText.text = (((int)(m_lastFramerate * 100 + .5) / 100.0)).ToString();

            if (player != null && (player.transform.position - exit.transform.position).magnitude < 0.5f)
                goToScene("Menu");

            // Input
            if (Input.GetKeyDown(KeyCode.R))
                RestartScene();
            if (Input.GetKeyDown(KeyCode.F))
                ChangeFrameRate();
            if (Input.GetKeyDown(KeyCode.C))
                heuristicText.text = theseusGraph.ChangeHeuristic();
        }

        // Encuentra los GameObjects, los asigna y los inicializa
        private void FindGO()
        {
            if (SceneManager.GetActiveScene().name == "Menu")
            {
                label = GameObject.FindGameObjectWithTag("DDLabel").GetComponent<Text>();
                label2 = GameObject.FindGameObjectWithTag("MinoLabel").GetComponent<Text>();
            }
            else if (SceneManager.GetActiveScene().name == "Labyrinth")
            {
                fRText = GameObject.FindGameObjectWithTag("Framerate").GetComponent<Text>();
                heuristicText = GameObject.FindGameObjectWithTag("Heuristic").GetComponent<Text>();
                smoothText = GameObject.FindGameObjectWithTag("Smooth").GetComponent<Text>();
                ChangeSmooth(false); // Inicialmente no suavizamos el camino
                mazeSizeText = GameObject.FindGameObjectWithTag("Dimensions").GetComponent<Text>();
                mazeSizeText.text = mazeSize;
                visitedText = GameObject.FindGameObjectWithTag("Visited").GetComponent<Text>();
                visitedText.text = "0";
                lengthText = GameObject.FindGameObjectWithTag("Length").GetComponent<Text>();
                lengthText.text = "0";
                costText = GameObject.FindGameObjectWithTag("Cost").GetComponent<Text>();
                costText.text = "0";
                searchTimeText = GameObject.FindGameObjectWithTag("Search Time").GetComponent<Text>();
                searchTimeText.text = "0 ms";
                percentageTimeText = GameObject.FindGameObjectWithTag("Percentage Time Consumed").GetComponent<Text>();
                percentageTimeText.text = "0.0 %";
                theseusGraph = GameObject.FindGameObjectWithTag("TesterGraph").GetComponent<TheseusGraph>();
                exitSlab = GameObject.FindGameObjectWithTag("Exit");
                startSlab = GameObject.FindGameObjectWithTag("Start");
                player = GameObject.Find("Avatar");
            }
        }

        // Cambia la escena
        public void goToScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }

        // Reinicia la escena
        public void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        // Devuelve el Avatar
        public GameObject GetPlayer()
        {
            if (player == null)
                player = GameObject.Find("Avatar");
            return player;
        }

        // Devuelve el grafo
        public Graph GetGraph()
        {
            return theseusGraph.GetGraph();
        }

        // Devuelve el número de minotauros
        public int getNumMinos()
        {
            return numMinos;
        }

        // Cambia el número de minotauros
        public void setNumMinos()
        {
            numMinos = int.Parse(label2.text);
        }

        // Establece la posición y tamaño de inicio
        public void SetStart(int i, int j, float size)
        {
            player.transform.position = new Vector3(i * size, 0.2f, j * size);
            startSlab.transform.position = new Vector3(i * size, 0.2f, j * size);
        }

        // Devuelve el nodo de salida
        public GameObject GetExitNode()
        {
            return exit;
        }

        // Establece la posición y tamaño de la salida
        public void SetExit(int i, int j, float size)
        {
            exit = new GameObject(); exit.name = "Exit";
            exit.transform.position = new Vector3(i * size, 0, j * size);
            exitSlab.transform.position = new Vector3(i * size, 0.3f, j * size);
        }

        // Cambia el coste de un nodo
        public void UpdatePathCost(Vector3 position, float costMultipliyer)
        {
            theseusGraph.UpdatePathCost(position, costMultipliyer);
        }

        // Cambia el framerate
        private void ChangeFrameRate()
        {
            if (frameRate == 30)
            {
                frameRate = 60;
                Application.targetFrameRate = 60;
            }
            else
            {
                frameRate = 30;
                Application.targetFrameRate = 30;
            }
        }

        // Cambia el texto del tamaño del laberinto
        public void ChangeMazeSize()
        {
            mazeSize = label.text;
        }

        // Devuelve el tamaño del laberinto
        public string getMazeSize()
        {
            return mazeSize;
        }

        // Cambia el texto de suavizado del camino
        public void ChangeSmooth(bool smooth)
        {
            if (smooth)
                smoothText.text = "On";
            else
                smoothText.text = "Off";
        }

        // Cambia el texto del número de baldosas visitadas
        public void UpdateVisited(int visited)
        {
            visitedText.text = visited.ToString();
        }

        // Cambia el texto de la longitud del camino
        public void UpdateLength(int length)
        {
            lengthText.text = length.ToString();
        }

        // Cambia el texto del coste del camino
        public void UpdateCost(float cost)
        {
            costText.text = cost.ToString();
        }

        // Cambia el texto del tiempo de búsqueda
        public void UpdateSearchTime(float time)
        {
            searchTimeText.text = (time * 1000.0f).ToString("F0") + " ms";
        }

        // Cambia el texto del porcentaje de tiempo de búsqueda
        public void UpdateSearchTimePercentage(float percentage)
        {
            percentageTimeText.text = percentage.ToString() + " %";
        }
    }
}