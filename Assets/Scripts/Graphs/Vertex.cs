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
        public int id; // Identificador del nodo

        private GameObject influence;

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

            GameManager.instance.UpdatePathCost(this, costMultiPliyer);
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
