//*****************************************************************
// INVEST3D : Simulação e visualização de investimentos
// Augusto Bulow
//
// Criação: 17/12/18
// Ultima alteração: 19/12/18
//
// CAMERA : funcoes basicas para controle e movimento de camera
//
//
//*****************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleCamera : MonoBehaviour {

    //POSICOES UTEIS
    Vector3 posicao_inicial;
    Vector3 posicao_destino;

    //ativa movimento
    bool mover_destino; 

    //VELOCIDADES : public para regulagem no inspector
    public float tempo_movimento = 1;
    public float velocidade_scroll = 1000;
    public float velocidade_mouse = 15;

    //LIMITES
    public Vector3 limite_min = new Vector3(-20f, 1.5f, -30f);
    public Vector3 limite_max = new Vector3(20f, 15f, -3f);

    //*****************************************************************
    // INICIALIZA
    //*****************************************************************

    void Start() {
        //guarda posicao inicial - home
        posicao_inicial = transform.position;
    }

    //*****************************************************************
    // UPDATE : roda todo frame
    //*****************************************************************

    void Update() {

        //MOUSE CONTROLA POSICAO 3D CAMERA
        //SCROLL = zoom - altera distancia Z da camera
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            posicao_destino = transform.position + new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel") * velocidade_scroll * Time.deltaTime);
            mover_destino = true;
        }

        //BOTAO MOUSE 2 ou 3 + MOVIMENTO = move camera
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            posicao_destino.x -= Input.GetAxis("Mouse X") * velocidade_mouse * Time.deltaTime;
            posicao_destino.y -= Input.GetAxis("Mouse Y") * velocidade_mouse * Time.deltaTime;
            posicao_destino.z = transform.position.z;
            mover_destino = true;
        }

        // MOVENDO DESTINOS: vai posicoes planejadas para mostrar evento / parte
        if (mover_destino)
        {
            //LIMITANDO POSICOES DA CAMERA 
            posicao_destino.x = Mathf.Clamp(posicao_destino.x, limite_min.x, limite_max.x);
            posicao_destino.y = Mathf.Clamp(posicao_destino.y, limite_min.y, limite_max.y);
            posicao_destino.z = Mathf.Clamp(posicao_destino.z, limite_min.z, limite_max.z);

            //posicao: vai até o destino mundo 3D
            transform.position = Vector3.Slerp(transform.position, posicao_destino, tempo_movimento * Time.deltaTime);

            //percebe chegou
            if (Vector3.Distance(transform.position, posicao_destino) < 0.1f)
            {
                mover_destino = false;
            }
        }
        //PARADA
        else
        {
            posicao_destino = transform.position;
        }
    }

    //*****************************************************************
    // VISAO do MES
    //*****************************************************************

    public void MostrarMes(Vector3 destino)
    {
        posicao_destino = destino;
        mover_destino = true;
    }

    //*****************************************************************
    // BOTAO HOME 
    //*****************************************************************

    public void BotaoHome()
    {
        posicao_destino = posicao_inicial;
        mover_destino = true;
    }

}
