using System.Linq.Expressions;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class playerMoviment : MonoBehaviour
{
    //Váriável de animação
    //Setando o tipo de variável, o animator é do tipo animator
    private Animator animator;


    //variável de velocidade
    public float speed = 2f;

    

   

    private void Start()
    {
        //Pegando o component de animação
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        
        //Variáveis para o vetor 3
    float horizontal = Input.GetAxis("Horizontal");
    float vertical = Input.GetAxis("Vertical");


        //Função responsável pelo movimento do personagem 
        transform.Translate(new Vector3(horizontal, vertical ,0) * speed * Time.deltaTime);


        //Setando as animações 
        if (vertical < 0)
            animator.Play("Front-walk");
        else if (vertical > 0)
            animator.Play("Back-walk");
        else if (horizontal < 0)
            animator.Play("Left-walk");
        else if (horizontal > 0)
            animator.Play("Right-walk");
        else animator.Play("Idle");
    }
}
