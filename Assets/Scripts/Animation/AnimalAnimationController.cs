/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using UnityEngine;

public class AnimalAnimationController : MonoBehaviour
{
    private Animator animator;

    [SerializeField]
    private Rigidbody rigid;

    [SerializeField]
    private float threshold = 1.5f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (rigid.velocity.magnitude >= threshold)
            animator.SetBool("Moving", true);
        else
            animator.SetBool("Moving", false);
    }
}
