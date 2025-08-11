using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class triggerScript : MonoBehaviour
{
    private int indiceDoAlvoAtual = 0;
    //lista pra associar os pontos
    public List<Transform> pontosDoCaminho;
    public Animator animatorTrigger;
    public Transform sapo;
    
    public void Update()
    {
        if (taTrigado == true)
        {
            
            //ele é a verocidade, cuidado com a ele.
            float euSouAVerocidade = 7f * Time.deltaTime;

            //lembrar de botar .position porque ta pegando a posicao, &%#$@!
            //coloquei uma variavel para nao ter que escrever o calculo o tempo todo
            //o MoveTowards segue a regra (PontoInicial, PontoFinal, VelocidadeQueAvança)
            Vector2 amendoim = Vector2.MoveTowards(sapo.transform.position, pontosDoCaminho[indiceDoAlvoAtual].position, euSouAVerocidade);
            Vector2 novaPosicao = amendoim;

            sapo.transform.position = novaPosicao;

            Vector2.Distance(sapo.transform.position, pontosDoCaminho[indiceDoAlvoAtual].position);

            if (Vector2.Distance(sapo.transform.position, pontosDoCaminho[indiceDoAlvoAtual].position) < 0.1f)
            {
                //se for o ultimo
                if (indiceDoAlvoAtual == pontosDoCaminho.Count - 1)
                {
                    taTrigado = false;
                    controle.enabled = true;
                    animatorTrigger.SetBool("AndandoSozinho", false);
                }
                else
                {
                    //aqui é para o parado (nesse caso, se o indice for a posicao do primeiro )
                    if (indiceDoAlvoAtual == 0)
                    {
                        taTrigado = false;
                        animatorTrigger.SetBool("Parado", true);
                        controle.enabled = false;
                        animatorTrigger.SetBool("AndandoSozinho", false);
                        StartCoroutine(StopWaitAMinute());
                        //animatorNormal.podeLer = true;
                    }

                    indiceDoAlvoAtual++;
                }
            }

        }
        //ver distancia dos pontos
        //Debug.Log(Vector2.Distance(sapo.transform.position, pontosDoCaminho[indiceDoAlvoAtual].position));
    }
    public PlayerControllerMOdificado controle;
    public PlayerInputToAnimator animatorNormal;
    
    bool taTrigado = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            Debug.Log("Trigou");
            controle.enabled = false;
            taTrigado = true;
            animatorTrigger.SetBool("AndandoSozinho", true);
            //animatorNormal.podeLer = false;
        }
    }
    IEnumerator StopWaitAMinute()
    {

        Debug.Log("to no ponto A e to esperandoooooo ososaodaiiofewhniusndf");

        yield return new WaitForSeconds(4f);

        Debug.Log("JA ESPEREI DEMAIS");

        taTrigado = true;
        animatorTrigger.SetBool("Parado", false);
        animatorTrigger.SetBool("AndandoSozinho", true);
    }
}
