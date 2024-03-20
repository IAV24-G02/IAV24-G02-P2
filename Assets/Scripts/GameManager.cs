/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using System.Collections.Generic;
using UCM.IAV.Navegacion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UCM.IAV.Movimiento
{
    public class GameManager : MonoBehaviour
    {
        // Singleton
        public static GameManager instance = null;

        // Textos UI
        Text fRText;
        Text heuristicText;
        Text label;
        Text label2;
        Text smoothText;
        Text mazeSizeText;
        string mazeSize = "10x10";
        Text visitedText;
        Text lengthText;
        Text costText;
        Text searchTimeText;
        Text percentageTimeText;

        private int frameRate = 60;
        TheseusGraph theseusGraph;

        // Variables de timer de framerate
        int m_frameCounter = 0;
        float m_timeCounter = 0.0f;
        float m_lastFramerate = 0.0f;
        float m_refreshTime = 0.5f;

        GameObject player = null;
        GameObject exitSlab = null;
        GameObject startSlab = null;
        GameObject exit = null;

        int numMinos = 1;

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

        // Delegado para hacer cosas cuando una escena termina de cargar (no necesariamente cuando ha cambiado/switched)
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            FindGO();
        }

        // Se llama cuando el juego ha terminado
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }


        // Update is called once per frame
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

            //Input
            if (Input.GetKeyDown(KeyCode.R))
                RestartScene();
            if (Input.GetKeyDown(KeyCode.F))
                ChangeFrameRate();
            if (Input.GetKeyDown(KeyCode.C))
                heuristicText.text = theseusGraph.ChangeHeuristic();
        }

        private void FindGO()
        {
            if (SceneManager.GetActiveScene().name == "Menu") // Nombre de escena que habría que llevar a una constante
            {
                label = GameObject.FindGameObjectWithTag("DDLabel").GetComponent<Text>();
                label2 = GameObject.FindGameObjectWithTag("MinoLabel").GetComponent<Text>();
            }
            else if (SceneManager.GetActiveScene().name == "Labyrinth") // Nombre de escena que habría que llevar a una constante
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

        public GameObject GetPlayer()
        {
            if (player == null) player = GameObject.Find("Avatar");
            return player;
        }

        public void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void setNumMinos()
        {
            numMinos = int.Parse(label2.text);
        }

        public int getNumMinos()
        {
            return numMinos;
        }

        public void goToScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }

        public GameObject GetExitNode()
        {
            return exit;
        }

        public void SetExit(int i, int j, float size)
        {
            exit = new GameObject(); exit.name = "Exit";
            exit.transform.position = new Vector3(i * size, 0, j * size);
            exitSlab.transform.position = new Vector3(i * size, 0.3f, j * size);
        }

        public void SetStart(int i, int j, float size)
        {
            player.transform.position = new Vector3(i * size, 0.2f, j * size);
            startSlab.transform.position = new Vector3(i * size, 0.2f, j * size);
        }

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

        public void ChangeMazeSize()
        {
            mazeSize = label.text;
        }
        public string getMazeSize()
        {
            return mazeSize;
        }

        public void ChangeSmooth(bool smooth)
        {
            if (smooth)
                smoothText.text = "On";
            else
                smoothText.text = "Off";
        }

        public void UpdateVisited(int visited)
        {
            visitedText.text = visited.ToString();
        }

        public void UpdateLength(int length)
        {
            lengthText.text = length.ToString();
        }

        public void UpdateCost(float cost)
        {
            costText.text = cost.ToString();
        }

        public void UpdateSearchTime(float time)
        {
            searchTimeText.text = (time * 1000.0f).ToString("F0") + " ms";
        }

        public void UpdateSearchTimePercentage(float percentage)
        {
            percentageTimeText.text = percentage.ToString() + " %";
        }

        public void UpdatePathCost(Vector3 position, float costMultipliyer)
        {
            theseusGraph.UpdatePathCost(position, costMultipliyer);
        }
    }
}