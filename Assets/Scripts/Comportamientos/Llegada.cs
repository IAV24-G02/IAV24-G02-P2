/*    
   Copyright (C) 2020 Federico Peinado
   http://www.federicopeinado.com

   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).

   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
namespace UCM.IAV.Movimiento
{
    using UnityEngine;

    /// <summary>
    /// Clase para modelar el comportamiento de SEGUIR a otro agente
    /// </summary>
    public class Llegada : ComportamientoAgente
    {
        [SerializeField]
        private float targetRadius; // El radio para llegar al objetivo

        [SerializeField]
        private float slowingRadius; // El radio en el que se empieza a ralentizarse

        [SerializeField]
        private float slowingForce; // La fuerza de ralentizado

        [SerializeField]
        private float avoidQuantity = 5; // Cantidad de evasión

        [SerializeField]
        private int raycastDistance = 7;   // Distancia de raycast

        private float timeToTarget = 0.1f; // El tiempo en el que conseguir la aceleracion objetivo

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

            //direccion.lineal += Avoidance();

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

                // Draw lines to show the incoming "beam" and the reflection.
                //Debug.DrawLine(pos, hit.point, Color.red);
                //Debug.DrawRay(hit.point, reflectVec, Color.green);

                return hit.point + hit.normal * avoidQuantity;
            }
            else
            {
                //Debug.DrawLine(pos, dir * distance, Color.yellow);
                return Vector3.zero;
            }
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
