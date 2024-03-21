/*    
   Copyright (C) 2020 Federico Peinado
   http://www.federicopeinado.com

   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).

   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using UnityEngine;

namespace UCM.IAV.Movimiento
{
    /// <summary>
    /// Clase para modelar el comportamiento de SEGUIR a otro agente
    /// </summary>
    public class Llegada : ComportamientoAgente
    {
        #region Variables
        /// <summary>
        /// El radio para llegar al objetivo
        /// </summary>
        [SerializeField]
        private float targetRadius;

        /// <summary>
        /// El radio en el que se empieza a ralentizarse
        /// </summary>
        [SerializeField]
        private float slowingRadius;

        /// <summary>
        /// La fuerza de ralentizado
        /// </summary>
        [SerializeField]
        private float slowingForce;

        /// <summary>
        /// Cantidad de evasión
        /// </summary>
        [SerializeField]
        private float avoidQuantity = 5;

        /// <summary>
        /// Distancia de raycast
        /// </summary>
        [SerializeField]
        private int raycastDistance = 7;

        /// <summary>
        /// El tiempo en el que conseguir la aceleracion objetivo
        /// </summary>
        private float timeToTarget = 0.1f;
        #endregion

        public override Direccion GetDireccion()
        {
            Direccion direccion = new Direccion();

            // Distancia de objeto al agente
            float distance = (objetivo.transform.position - transform.position).magnitude;

            // Si ha alcanzado el radio objetivo se para
            if (distance < targetRadius)
            {
                direccion.lineal = new Vector3(0, 0, 0);
                return direccion;
            }

            float targetAccel;

            // Máxima aceleración desde fuera del radio de frenado
            if (distance > slowingRadius)
                targetAccel = agente.aceleracionMax;
            // Aceleración escalada
            else
                targetAccel = agente.aceleracionMax * distance / (slowingRadius * slowingForce);

            // Velocity combina aceleración y dirección
            Vector3 targetVelocity = objetivo.transform.position - transform.position;
            targetVelocity.Normalize();
            targetVelocity *= targetAccel;

            // La aceleración se posiciona al nivel de la del objetivo
            direccion.lineal = targetVelocity - agente.velocidad;
            direccion.lineal /= timeToTarget;

            // Comprobamos que no se pase de aceleración
            if (direccion.lineal.magnitude > agente.aceleracionMax)
            {
                direccion.lineal.Normalize();
                direccion.lineal *= agente.aceleracionMax;
            }

            return direccion;
        }

        Vector3 RayCastCollision(Vector3 pos, Vector3 dir, LayerMask lMask)
        {
            RaycastHit hit;
            if (Physics.Raycast(pos, dir, out hit, raycastDistance, lMask))
            {
                // Find the line from the gun to the point that was clicked.
                Vector3 incomingVec = hit.point - pos;

                if (incomingVec.magnitude > raycastDistance) return Vector3.zero;

                // Use the point's normal to calculate the reflection vector.
                Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);

                return hit.point + hit.normal * avoidQuantity;
            }
            else
                return Vector3.zero;
        }

        Vector3 Avoidance()
        {
            LayerMask lMask = 1 << 8;

            Vector3 dirAcc = Vector3.zero;
            dirAcc += RayCastCollision(transform.position, transform.forward, lMask) * 10;
            dirAcc += RayCastCollision(transform.position, (transform.forward * 2 + transform.right).normalized, lMask);
            dirAcc += RayCastCollision(transform.position, (transform.forward * 2 - transform.right).normalized, lMask);

            return dirAcc.normalized * avoidQuantity;
        }
    }
}
