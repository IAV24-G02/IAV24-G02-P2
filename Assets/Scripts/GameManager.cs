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
        #region Variables
        /// <summary>
        /// Singleton
        /// </summary>
        public static GameManager instance = null;

        /// <summary>
        /// Tamaño del laberinto
        /// </summary>
        private string mazeSize = "10x10";

        /// <summary>
        /// Framerate
        /// </summary>
        private int frameRate = 60;

        /// <summary>
        /// Grafo del laberinto
        /// </summary>
        private TheseusGraph theseusGraph;

        /// <summary>
        /// Número de minotauros
        /// </summary>
        private int numMinos = 1;

        /// <summary>
        /// Jugador
        /// </summary>
        private GameObject player = null;

        /// <summary>
        /// Baldosa de inicio
        /// </summary>
        private GameObject startSlab = null;

        /// <summary>
        /// Baldosa de salida
        /// </summary>
        private GameObject exitSlab = null;

        /// <summary>
        /// Nodo de salida
        /// </summary>
        private GameObject exit = null;

        /// <summary>
        /// Texto de framerate
        /// </summary>
        private Text fRText;

        /// <summary>
        /// Texto de la heurística
        /// </summary>
        private Text heuristicText;

        /// <summary>
        /// Texto del tamaño del laberinto en el menú
        /// </summary>
        private Text label;

        /// <summary>
        /// Texto del número de minotauros en el menú
        /// </summary>
        private Text label2;

        /// <summary>
        /// Texto de suavizado del camino
        /// </summary>
        private Text smoothText;

        /// <summary>
        /// Texto del tamaño del laberinto en el laberinto
        /// </summary>
        private Text mazeSizeText;

        /// <summary>
        /// Texto de baldosas visitadas
        /// </summary>
        private Text visitedText;

        /// <summary>
        /// Texto de longitud del camino
        /// </summary>
        private Text lengthText;

        /// <summary>
        /// Texto de coste del camino
        /// </summary>
        private Text costText;

        /// <summary>
        /// Texto de tiempo de búsqueda
        /// </summary>
        private Text searchTimeText;

        /// <summary>
        /// Texto de porcentaje de tiempo de búsqueda
        /// </summary>
        private Text percentageTimeText;

        /// <summary>
        /// Variables de timer de framerate
        /// </summary>
        private int m_frameCounter = 0;
        private float m_timeCounter = 0.0f;
        private float m_lastFramerate = 0.0f;
        private float m_refreshTime = 0.5f;
        #endregion

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

        /// <summary>
        /// Encuentra los GameObjects, los asigna y los inicializa
        /// </summary>
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

        /// <summary>
        /// Cambia la escena
        /// </summary>
        /// <param name="scene"></param>
        public void goToScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }

        /// <summary>
        /// Reinicia la escena
        /// </summary>
        public void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Devuelve el Avatar
        /// </summary>
        /// <returns></returns>
        public GameObject GetPlayer()
        {
            if (player == null)
                player = GameObject.Find("Avatar");
            return player;
        }

        /// <summary>
        /// Devuelve el grafo
        /// </summary>
        /// <returns></returns>
        public Graph GetGraph()
        {
            return theseusGraph.GetGraph();
        }

        /// <summary>
        /// Devuelve el número de minotauros
        /// </summary>
        /// <returns></returns>
        public int getNumMinos()
        {
            return numMinos;
        }

        /// <summary>
        /// Cambia el número de minotauros
        /// </summary>
        public void setNumMinos()
        {
            numMinos = int.Parse(label2.text);
        }

        /// <summary>
        /// Establece la posición y tamaño de inicio
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="size"></param>
        public void SetStart(int i, int j, float size)
        {
            player.transform.position = new Vector3(i * size, 0.2f, j * size);
            startSlab.transform.position = new Vector3(i * size, 0.2f, j * size);
        }

        /// <summary>
        /// Devuelve el nodo de salida
        /// </summary>
        /// <returns></returns>
        public GameObject GetExitNode()
        {
            return exit;
        }

        /// <summary>
        /// Establece la posición y tamaño de la salida
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="size"></param>
        public void SetExit(int i, int j, float size)
        {
            exit = new GameObject(); exit.name = "Exit";
            exit.transform.position = new Vector3(i * size, 0, j * size);
            exitSlab.transform.position = new Vector3(i * size, 0.3f, j * size);
        }

        /// <summary>
        /// Cambia el coste de un nodo
        /// </summary>
        /// <param name="position"></param>
        /// <param name="costMultipliyer"></param>
        public void UpdatePathCost(Vector3 position, float costMultipliyer)
        {
            theseusGraph.UpdatePathCost(position, costMultipliyer);
        }

        /// <summary>
        /// Cambia el framerate
        /// </summary>
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

        /// <summary>
        /// Cambia el texto del tamaño del laberinto
        /// </summary>
        public void ChangeMazeSize()
        {
            mazeSize = label.text;
        }

        /// <summary>
        /// Devuelve el tamaño del laberinto
        /// </summary>
        /// <returns></returns>
        public string getMazeSize()
        {
            return mazeSize;
        }

        /// <summary>
        /// Cambia el texto de suavizado del camino
        /// </summary>
        /// <param name="smooth"></param>
        public void ChangeSmooth(bool smooth)
        {
            if (smooth)
                smoothText.text = "On";
            else
                smoothText.text = "Off";
        }

        /// <summary>
        /// Cambia el texto del número de baldosas visitadas
        /// </summary>
        /// <param name="visited"></param>
        public void UpdateVisited(int visited)
        {
            visitedText.text = visited.ToString();
        }

        /// <summary>
        /// Cambia el texto de la longitud del camino
        /// </summary>
        /// <param name="length"></param>
        public void UpdateLength(int length)
        {
            lengthText.text = length.ToString();
        }

        /// <summary>
        /// Cambia el texto del coste del camino
        /// </summary>
        /// <param name="cost"></param>
        public void UpdateCost(float cost)
        {
            costText.text = cost.ToString();
        }

        /// <summary>
        /// Cambia el texto del tiempo de búsqueda
        /// </summary>
        /// <param name="time"></param>
        public void UpdateSearchTime(float time)
        {
            searchTimeText.text = (time * 1000.0f).ToString("F0") + " ms";
        }

        /// <summary>
        /// Cambia el texto del porcentaje de tiempo de búsqueda
        /// </summary>
        /// <param name="percentage"></param>
        public void UpdateSearchTimePercentage(float percentage)
        {
            percentageTimeText.text = percentage.ToString() + " %";
        }
    }
}