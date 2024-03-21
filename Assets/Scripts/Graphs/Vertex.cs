/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using UnityEngine;
using System;
using UCM.IAV.Movimiento;

namespace UCM.IAV.Navegacion
{
    /// <summary>
    /// Clase que modela un nodo de un grafo
    /// </summary>
    [System.Serializable]
    public class Vertex : MonoBehaviour, IComparable<Vertex>
    {
        #region Variables
        /// <summary>
        /// Identificador del nodo
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Identificador del nodo anterior
        /// </summary>
        public int PreviousId { get; set; }

        /// <summary>
        /// Coste del nodo
        /// </summary>
        public float Cost { get; set; }

        /// <summary>
        /// Coste acumulado hasta el nodo
        /// </summary>
        public float CostSoFar { get; set; }

        /// <summary>
        /// Coste total estimado hasta el nodo
        /// </summary>
        public float EstimatedTotalCost { get; set; }

        /// <summary>
        /// Objeto para mostrar la influencia
        /// </summary>
        private GameObject influence;
        #endregion

        public Vertex(int previousId = -1, float cost = 1.0f, float costSoFar = 0.0f, float estimatedTotalCost = 0.0f)
        {
            PreviousId = previousId;
            Cost = cost;
            CostSoFar = costSoFar;
            EstimatedTotalCost = estimatedTotalCost;
        }

        private void Start()
        {
            influence = transform.Find("Influence")?.gameObject;
            if (influence != null)
                influence.SetActive(false);
        }

        /// <summary>
        /// Activa o desactiva la influencia del nodo y actualiza el coste de la baldosa
        /// </summary>
        public void SetInfluence(bool doesInfluence, float costMultiPliyer = 1.0f)
        {
            if (influence != null)
                influence.SetActive(doesInfluence);

            Cost *= costMultiPliyer;
            GameManager.instance.UpdatePathCost(this.transform.position, costMultiPliyer);
        }

        public int CompareTo(Vertex other)
        {
            return this.id.CompareTo(other.id);
        }

        public bool Equals(Vertex other)
        {
            return (other.id == this.id);
        }

        public override bool Equals(object obj)
        {
            Vertex other = (Vertex)obj;
            if (ReferenceEquals(obj, null)) return false;
            return (other.id == this.id);
        }

        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }
    }
}
