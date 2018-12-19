//*****************************************************************
// INVEST3D : Simulação e visualização de investimentos
// Augusto Bulow
//
// Criação: 17/12/18
// Ultima alteração: 19/12/18
//
// OBJETO: codigo para objs representativos 3D
// centraliza funções e metodos uteis. Posicao, escala, etx
//
//*****************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objeto : MonoBehaviour {

    //enum : acoes dos objetos 3D representativos
    public enum ObjetoAcao
    {
        pronto, movendo, escala
    }

    //acao atual
    public ObjetoAcao acao;

    //INFO UTIL = mouse over info
    public string info = "";            //info do que representa
    public float valor = 0;             //valor obj esta representando
    public int numero_mes = 0;          //numero mes

    //MOVIMENTO: destino e velocidade movimento
    public Vector3 posicao_destino;
    public float tempo_movimento = 1f;

    //*****************************************************************
    // UPDATE : chamado por frame renderizado = faz os movimentos
    //
    //*****************************************************************

    void Update () {

        //MAQUINA ESTADOS SIMPLES
        switch (acao)
        {
            //PARADO : pronto
            case ObjetoAcao.pronto:

                break;

            //MOVENDO : 
            case ObjetoAcao.movendo:

                //posicao: vai até o destino mundo 3D - slerp aproximação suave
                transform.position = Vector3.Slerp(transform.position, posicao_destino, tempo_movimento * Time.deltaTime);

                //percebe chegou
                if (Vector3.Distance(transform.position, posicao_destino) < 0.1f)
                {
                    transform.position = posicao_destino;
                    acao = ObjetoAcao.pronto;
                }

                break;
            
            //ACERTA ESCALA representativa
            case ObjetoAcao.escala:

                float altura = 10 * (valor / Info.meta_valor_total);

                if (transform.localScale.y < altura)
                {
                    //ajusta escala e corrige altura 3D
                    transform.localScale += new Vector3(0, 0.4f, 0) * Time.deltaTime;
                    transform.position += new Vector3(0, 0.2f, 0) * Time.deltaTime;

                    if (transform.localScale.y >= altura)
                    {
                        transform.localScale = new Vector3(transform.localScale.x, altura, transform.localScale.z);
                        acao = ObjetoAcao.pronto;
                    }
                }

                break;
        }
	}


    //*****************************************************************
    // ATUALIZA ESCALA : ativa animacao basica
    //
    //*****************************************************************

    public void AtualizaValor(float quanto)
    {
        valor = quanto;
        acao = ObjetoAcao.escala;
    }

    //*****************************************************************
    // MOUSE OVER : avisa controle
    //
    //*****************************************************************

    private void OnMouseOver()
    {
        Info.obj_sobre = gameObject;
    }

    private void OnMouseExit()
    {
        Info.obj_sobre = null;
    }
}
