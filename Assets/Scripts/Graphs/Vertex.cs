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
    [System.Serializable]
    public class Vertex : MonoBehaviour, IComparable<Vertex>
    {
        public int id { get; set; }                     // Identificador del nodo
        public int PreviousId { get; set; }             // Identificador del nodo
        public float Cost { get; set; }                 // Coste del nodo
        public float CostSoFar { get; set; }            // Coste acumulado hasta el nodo seleccionado
        public float EstimatedTotalCost { get; set; }   // Coste total estimado hasta el nodo seleccionado

        private GameObject influence; // Objeto para mostrar la influencia

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
