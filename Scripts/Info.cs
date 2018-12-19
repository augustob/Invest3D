//*****************************************************************
// INVEST3D : Simulação e visualização de investimentos
// Augusto Bulow
//
// Criação: 17/12/18
// Ultima alteração: 19/12/18
//
// INFO: centraliza informações essenciais
// static: facilita acesso de qualquer objeto
//
//*****************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Info {

    //VALORES BASICOS DA SIMULACAO
    public static float meta_valor_total = 10000;
    public static int meta_meses = 12;
    public static float meta_valor_inicial = 1000f;
    public static float meta_valor_mensal = 750;

    //VALORES POSICAO ATUAL
    public static int atual_mes = 1;
    public static float atual_valor = 0;

    //TOTAIS
    public static float total_rendimentos = 0;
    public static float total_investimentos = 0;

    //AUXILIAR PARA PROMPT - sobre objeto
    public static GameObject obj_sobre;

}
