/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
namespace UCM.IAV.Movimiento
{
    using UnityEngine;

    public class ControlJugador: ComportamientoAgente
    {
        private Vector3 worldPoint;             // Punto en el mundo
        private Camera mainCamera;              // Cámara principal

        [SerializeField]
        private string vertexTag = "Vertex";    // Etiqueta de un nodo normal

        [SerializeField]
        private string obstacleTag = "Wall";    // Etiqueta de un obstáculo, tipo pared...

        public override void Awake()
        {
            base.Awake();
            mainCamera = Camera.main;
        }

        private GameObject GetNodeFromScreen(Vector3 screenPosition)
        {
            GameObject node = null;
            Ray ray = mainCamera.ScreenPointToRay(screenPosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            foreach (RaycastHit h in hits)
            {
                if (!h.collider.CompareTag(vertexTag) && !h.collider.CompareTag(obstacleTag))
                    continue;
                node = h.collider.gameObject;
                break;
            }
            return node;
        }

        public override Direccion GetDireccion()
        {
            Direccion direccion = new Direccion();

            if (Input.GetMouseButton(0))
            {
                GameObject node = GetNodeFromScreen(Input.mousePosition);
                if (node != null)
                {
                    worldPoint = node.transform.position;
                    direccion.lineal.x = worldPoint.x - agente.transform.position.x;
                    direccion.lineal.z = worldPoint.z - agente.transform.position.z;
                }
            }
            else
            {
                // Direccion actual
                direccion.lineal.x = Input.GetAxis("Horizontal");
                direccion.lineal.z = Input.GetAxis("Vertical");
            }

            // Resto de cálculo de movimiento
            direccion.lineal.Normalize();
            direccion.lineal *= agente.aceleracionMax;

            return direccion;
        }
    }
}