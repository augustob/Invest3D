//*****************************************************************
// INVEST3D : Simulação e visualização de investimentos
// Augusto Bulow
//
// Criação: 17/12/18
// Ultima alteração: 19/12/18
//
// CONTROLE: toda simulação e controle principal do aplicativo 
// centralizados neste código
//
//*****************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Controle : MonoBehaviour
{

    //PREFABS - objs representativos para cena 3D - instanciaveis
    public GameObject investimento_prefab;          //obj 3d representa investimento / partes
    public GameObject mes_prefab;                   //obj 3d montagem cena - base visual dos meses
    public GameObject grafico1_prefab;              //obj 3d mostra grafico linha - simula
    public GameObject valor_atual_obj;              //obj 3d representa o valor atual em conta
    public GameObject rendimento_obj;               //obj 3d representa rendimento acumulado
    public GameObject investido_obj;                //obj 3d representa total investido

    //ATALHOS PARA TEXTOS 3D e Menus em cena
    public TextMesh texto_meta;
    public Text texto_prompt;
    public GameObject menu_investe_mes;
    public GameObject menu_inicial;
    public GameObject menu_final;

    //POSICOES PRE-MARCADAS EM CENA  
    public Transform investimento_origem;           //pos cria blocos

    //LISTAS - guardam objs 3d criados - facilita acesso
    List<GameObject> lista_meses;

    //VARIAVEIS AUXILIARES
    Vector3 posicao_inicia_grafico;                 //aux posicao topo grafico mes atual - facilita desenho grafico
    ControleCamera cam_script;                      //acesso ao controle da camera 3D
    GameObject parte_atual;                         //facilita acesso a representacao atual do investimento
    GameObject obj_sobre;                           //controle prompt mouse over

    //*****************************************************
    // INICIALIZACAO
    //
    //*****************************************************

    void Start()
    {
        //acessa script camera
        cam_script = Camera.main.GetComponent<ControleCamera>();

        //inicializa listas controle
        lista_meses = new List<GameObject>();
    }

    //*****************************************************
    // UPDATE: roda todo frame
    //
    //*****************************************************

    void Update()
    {
        //CONTROLA PROMPT
        if (Info.obj_sobre != null)
        {
            //seta texto uma vez
            if (Info.obj_sobre != obj_sobre)
            {
                obj_sobre = Info.obj_sobre;
                Objeto scri = obj_sobre.GetComponent<Objeto>();

                texto_prompt.text = scri.info + "\n";
                texto_prompt.text += "$ " + scri.valor.ToString("0.00") + "\n";
            }

            //posicao texto
            texto_prompt.transform.position = Input.mousePosition;
        }
        else
        {
            texto_prompt.text = "";
        }
    }


    //*****************************************************
    // CALCULA META MES - chamado ao alterar valores menu
    // calculo minimo para definir valor mensal meta
    //*****************************************************

    public void CalculaMetaMensal()
    {
        //busca texto nos inputs de tela
        string input = GameObject.Find("InputMetaValor").GetComponent<InputField>().text;
        float valor = float.Parse(input);
        Info.meta_valor_total = valor;

        //meses selecionados
        input = GameObject.Find("InputMetaTempo").GetComponent<InputField>().text;
        valor = float.Parse(input);
        Info.meta_meses = (int)valor;

        //investimento inicial
        input = GameObject.Find("InputMetaValorInicial").GetComponent<InputField>().text;
        valor = float.Parse(input);
        Info.meta_valor_inicial = valor;

        //define valor mes necessario
        float valor_mensal = (Info.meta_valor_total - Info.meta_valor_inicial) / Info.meta_meses;
        Info.meta_valor_mensal = valor_mensal;

        //mostra na tela
        GameObject.Find("InputMetaValorMensal").GetComponent<InputField>().text = valor_mensal.ToString("0.00");

    }

    //*****************************************************
    // MOVEU SLIDER : recalcular e atualizar menu
    //*****************************************************
    public void AtualizaSliderMes()
    {
        float valor = GameObject.Find("SliderMes").GetComponent<Slider>().value;
        GameObject.Find("InputMetaTempo").GetComponent<InputField>().text = valor.ToString();
        CalculaMetaMensal();
    }


    //*****************************************************
    // BOTAO : Reiniciar
    // reload do mapa para nova simulacao
    //*****************************************************

    public void BotaoReiniciar()
    {
        //RESET : inserir confirmaçao !!

        //reset vars controle
        Info.total_investimentos = 0;
        Info.total_rendimentos = 0;
        Info.atual_mes = 1;
        Info.atual_valor = 0;


        //carrega cena
        SceneManager.LoadScene(0);
    }

    //*****************************************************
    // BOTAO : seta valores da simulação
    // inicia a construcao cena 3D
    //*****************************************************

    public void BotaoInicializa()
    {
        //seta valor atual = investimento inicial
        Info.atual_valor = Info.meta_valor_inicial;
        Info.total_investimentos = Info.meta_valor_inicial;

        //usa coroutine (esquema Unity3D) para temporarização
        StartCoroutine(InicializaTempo());

        //atualiza interface textos
        AtualizaTextosUI();
    }


    //FUNCAO TEMPORIZADA : animação basica montagem cena inicial
    IEnumerator InicializaTempo()
    {
        //Prepara representacao do investimento - numero de partes:  total / meses
        float total_partes = Info.meta_meses;
        float valor_parte = Info.meta_valor_total / Info.meta_meses;

        //escala calculada objs3d
        float escala_y = 10 * (valor_parte / Info.meta_valor_total);

        //para animacao - do texto meta
        float valor_provisorio = 0;

        //representa meses - base visual 3D posicoes
        for (int mes = 0; mes < Info.meta_meses; mes++)
        {
            //cria instancia objeto 3D - para esquerda (x-)
            Vector3 posicao = new Vector3((mes + 1) * -1.1f, -0.5f, 0);
            GameObject atual = Instantiate(mes_prefab, posicao, Quaternion.identity);
            atual.name = "Mes" + mes.ToString();
            lista_meses.Add(atual);
        }

        //representa meta investimento com objetos 3D / 2D
        for (int n = 1; n <= total_partes; n++)
        {
            //cria instancia objeto 3D
            GameObject inv_obj = Instantiate(investimento_prefab, investimento_origem.position, Quaternion.identity);
            //escala do objeto 3D
            inv_obj.transform.localScale = new Vector3(1, escala_y, 1);
            //nome e cor (variada)
            inv_obj.name = "Invest-parte" + n.ToString();
            inv_obj.GetComponent<Renderer>().material.color = new Color(0.05f * n, 0.05f * n, 1, 1);

            //acessa script do objeto novo
            Objeto script = inv_obj.GetComponent<Objeto>();
            if (script)
            {
                //preenche info no novo objeto 3D
                script.info = "Meta";
                script.numero_mes = n;
                script.valor = Info.meta_valor_total;
                script.posicao_destino = new Vector3(0, n * escala_y - escala_y / 2, 0);
                script.acao = Objeto.ObjetoAcao.movendo;
            }

            //mostra meta geral de forma animada / text3d
            valor_provisorio += valor_parte;
            texto_meta.text = "Meta: " + valor_provisorio.ToString("0.00");
            texto_meta.transform.position = new Vector3(0, n * escala_y + 1, -1);

            yield return new WaitForSeconds(0.1f);
        }

        //ESPERA ANIMACAO
        yield return new WaitForSeconds(1f);

        //representa o capital inicial - ponto partida
        CriaCapitalInicial();
    }


    //*****************************************************
    // CRIA OBJ 3D representa investimento inicial
    // inicio - cria obj base do grafico 3D
    //*****************************************************

    void CriaCapitalInicial()
    {
        //mostra investimento inicial com objetos 3D
        GameObject inicial = Instantiate(investimento_prefab, investimento_origem.position, Quaternion.identity);

        //posicao 3D: mes 1 - mais tamanho escala base (alinhado ponto zero)
        Vector3 posicao_mes = lista_meses[lista_meses.Count - 1].transform.position + new Vector3(0, 0.5f, 0);
        inicial.transform.position = posicao_mes;

        //nome e cor
        inicial.GetComponent<Renderer>().material.color = Color.green;
        inicial.name = "Inicial";

        //pequeno - efeito cresce
        inicial.transform.localScale = new Vector3(1, 0, 1);

        //atualiza info - script do objeto
        Objeto script = inicial.GetComponent<Objeto>();
        if (script)
        {
            script.info = "Capital Inicial";
            script.valor = Info.meta_valor_inicial;
            script.acao = Objeto.ObjetoAcao.escala;
        }

        //define obj como parte atual
        parte_atual = inicial;

        //atualiza mostrador 3d capital
        AtualizaValorAtual();
    }


    //*****************************************************
    // SIMULA MES : valores random, representativos
    //
    //*****************************************************

    public void BotaoSimulaMes()
    {
        //simula - adiciona mes
        Info.atual_mes++;

        AtualizaTextosUI();

        //simulacao basica rendimentos diarios - temporizada
        StartCoroutine(SimulaMesTempo());
    }

    IEnumerator SimulaMesTempo()
    {
        //MOSTRA INFO DA SIMULACAO - acesso textos UI
        Text texto_log_dias = GameObject.Find("TextLogDias").GetComponent<Text>();
        texto_log_dias.text = "";

        Text texto_saldos = GameObject.Find("TextSaldos").GetComponent<Text>();
        texto_saldos.text = "Saldo inicial: $ " + Info.atual_valor.ToString("0.00");

        //POSICAO para grafico = referencia ultima parte
        posicao_inicia_grafico = parte_atual.transform.position + new Vector3(0, parte_atual.transform.localScale.y, -0.6f);
        posicao_inicia_grafico.y = 0;

        //POSICIONA CAMERA : mostra mes da acao
        Vector3 pos = posicao_inicia_grafico + new Vector3(0, parte_atual.transform.localScale.y, -5);
        cam_script.MostrarMes(pos);

        //espera movimento camera
        yield return new WaitForSeconds(1f);

        //cria obj3d - representa grafico linha
        GameObject gra = Instantiate(grafico1_prefab, Vector3.zero, Quaternion.identity);
        LineRenderer grafico_atual = gra.GetComponent<LineRenderer>();

        //precisa inicializar todos pontos grafico (evita erro grafico)
        for (int i = 0; i < grafico_atual.positionCount; i++)
        {
            grafico_atual.SetPosition(i, posicao_inicia_grafico);
        }

        //rendimento simulado randomico (buscar valor real) apenas simulação para visualização
        float rende = 0;
        float rende_mes = 0;
        float valor_provisorio = Info.atual_valor;
        float altura_calculada = 0;

        //simula 20 dias uteis (nao real)
        for (int dia = 0; dia < 20; dia++)
        {
            //VALOR ALEATORIO / DIA
            rende = Random.Range(-0.01f, 0.025f);
            //rendimento acumulado mes 
            rende_mes += rende;
            //calcula valor + rende
            valor_provisorio = valor_provisorio + (valor_provisorio * rende);
            //escala para grafico
            altura_calculada = 10 * (valor_provisorio / Info.meta_valor_total);

            //desenha grafico linha
            Vector3 posicao = posicao_inicia_grafico + new Vector3(dia * 0.055f, altura_calculada, 0);
            grafico_atual.positionCount++;
            grafico_atual.SetPosition(dia, posicao);

            //mostra info UI
            texto_log_dias.text += "Dia " + dia.ToString() + ": " + rende.ToString("0.000") + "\n";

            yield return new WaitForSeconds(0.1f);
        }

        //APURA TOTAIS SIMULACAO
        float valor_rende_mes = valor_provisorio - Info.atual_valor;

        //USA VALORES COMO ATUAL - registra
        Info.atual_valor = valor_provisorio;
        Info.total_rendimentos += valor_rende_mes;

        //INFO na tela UI
        texto_saldos.text += "\nRendimentos mês: $ " + valor_rende_mes.ToString("0.00");
        texto_saldos.text += "\nSaldo final: $ " + Info.atual_valor.ToString("0.00");

        //*******************************************************
        //CRIA OBJ3D : representa ganho investimento mes
        //*******************************************************
        Vector3 nova_posicao = posicao_inicia_grafico;
        nova_posicao += new Vector3(0.5f, 0, -2f);
        GameObject recebe_mes = Instantiate(investimento_prefab, nova_posicao, Quaternion.identity);
        //nome e cor
        recebe_mes.GetComponent<Renderer>().material.color = Color.yellow;
        recebe_mes.name = "RendeMes" + Info.atual_mes;
        //inicia pequeno - efeito cresce
        recebe_mes.transform.localScale = new Vector3(1, 0, 1);

        //acessa script do objeto novo
        Objeto script = recebe_mes.GetComponent<Objeto>();
        if (script)
        {
            script.info = "Rendimento (mês " + Info.atual_mes.ToString() + ")";
            script.AtualizaValor(valor_rende_mes);
        }

        //*******************************************************
        //CRIA OBJ3D - proximo mes
        //*******************************************************
        GameObject novo_mes = Instantiate(investimento_prefab, nova_posicao, Quaternion.identity);
        //tamanho - mes atual e cresce
        novo_mes.transform.localScale = parte_atual.transform.localScale;
        novo_mes.transform.position = parte_atual.transform.position + new Vector3(1.1f, 0, 0);

        //nome e cor
        novo_mes.name = "Mes" + Info.atual_mes.ToString();
        novo_mes.GetComponent<Renderer>().material.color = Color.green;

        //acessa script do objeto novo
        script = novo_mes.GetComponent<Objeto>();
        if (script)
        {
            script.info = "Saldo (mês " + Info.atual_mes.ToString() + ")";
            script.AtualizaValor(Info.atual_valor);
        }

        //define novo mes como atual
        parte_atual = novo_mes;

        //atualiza mostrador 3d capital e textos UI
        AtualizaValorAtual();
        AtualizaTextosUI();

        //AGUARDA ANIMACOES
        yield return new WaitForSeconds(2f);

        //menu investimento mes
        //se nao atingiu meta
        if (Info.atual_valor < Info.meta_valor_total)
        {
            MenuInvestimentoMensal();
        }
        //atingiu meta = fim
        else
        {
            MenuFimMeta();
        }
    }


    //*****************************************************
    // FINAL : atingiu meta
    //
    //*****************************************************

    void MenuFimMeta()
    {
        //final temporizado 
        StartCoroutine(MenuFimMetaTempo());
    }

    IEnumerator MenuFimMetaTempo()
    {
        yield return new WaitForSeconds(2);

        //ativa menu basico
        menu_final.SetActive(true);

        //textos : resumo basico da simulação
        string resumo = "Saldo final de $ " + Info.atual_valor.ToString("0.00") + "\n\n";
        resumo += "Rendimentos de $ " + Info.total_rendimentos.ToString("0.00") + "\n\n";
        resumo += "Total investido de $ " + Info.total_investimentos.ToString("0.00") + "\n\n";
        resumo += "Meta alcançada em " + Info.atual_mes.ToString() + " meses \n(planejado em " + Info.meta_meses.ToString() + ")";

        GameObject.Find("TextResumoFinal").GetComponent<Text>().text = resumo;
    }

    //*****************************************************
    // MENU: confirma investimento mensal
    // simula pagamento parcelas 
    //*****************************************************

    void MenuInvestimentoMensal()
    {
        //ativa menu
        menu_investe_mes.SetActive(true);
        //ajusta textos
        GameObject.Find("TextInvesteMes").GetComponent<Text>().text = "Meta planejada: $ " + Info.meta_valor_mensal.ToString("0.00");
        GameObject.Find("TextTituloMensal").GetComponent<Text>().text = "Investimento Mensal\n Mês " + Info.atual_mes.ToString();
    }

    //*****************************************************
    // BOTAO: confirma investimento mensal
    //
    //*****************************************************

    public void BotaoInvesteMes()
    {
        //busca valor input !! (incluir investe valor vairado)
        float valor_investido = Info.meta_valor_mensal;

        //simula como real
        Info.total_investimentos += valor_investido;
        Info.atual_valor += valor_investido;

        //desliga menu
        menu_investe_mes.SetActive(false);

        //atualiza mostrador 3d capital e textos UI
        AtualizaValorAtual();
        AtualizaTextosUI();

        //atualiza 3D mes atual 
        parte_atual.GetComponent<Objeto>().AtualizaValor(Info.atual_valor);

        //POSICIONA CAMERA : mostra mes da acao
        Vector3 pos = Camera.main.transform.position + new Vector3(2, 2, -7);
        cam_script.MostrarMes(pos);

        //*******************************************************
        //CRIA OBJ3D - investimento mes
        //*******************************************************
        Vector3 nova_posicao = new Vector3(parte_atual.transform.position.x, 0, parte_atual.transform.position.z);
        GameObject investe = Instantiate(investimento_prefab, nova_posicao, Quaternion.identity);
        //tamanho - mes atual e cresce
        investe.transform.localScale = new Vector3(1,0,1);
        investe.transform.position +=  new Vector3(-0.5f, 0, -1.1f);

        //nome e cor
        investe.name = "Investido" + Info.atual_mes.ToString();
        investe.GetComponent<Renderer>().material.color = new Color(0, 0.6f, 0.1f, 1);

        //acessa script do objeto novo
        Objeto script = investe.GetComponent<Objeto>();
        if (script)
        {
            script.info = "Investido (mês " + Info.atual_mes.ToString() + ")";
            script.AtualizaValor(valor_investido);
        }


        //FIM > se nao atingiu meta
        if (Info.atual_valor >= Info.meta_valor_total)
        {
            MenuFimMeta();
        }
    }


    //*****************************************************
    // ATUALIZA TEXTOS UI
    //
    //*****************************************************

    void AtualizaTextosUI()
    {
        GameObject.Find("TextMesAtual").GetComponent<Text>().text = "Mês atual: " + Info.atual_mes + "/" + Info.meta_meses;
        GameObject.Find("TextValorAtual").GetComponent<Text>().text = "Saldo Atual $ " + Info.atual_valor.ToString("0.00");
    }

    //*****************************************************
    // ATUALIZA OBJETOS REPRESENTATIVOS 3D
    // 
    //*****************************************************

    void AtualizaValorAtual()
    {
        //ATUALIZA RENDIMENTOS - representacao 3D

        //obj saldo atual
        valor_atual_obj.GetComponent<Objeto>().AtualizaValor(Info.atual_valor);

        //obj rendimento total
        rendimento_obj.GetComponent<Objeto>().AtualizaValor(Info.total_rendimentos);

        //obj total investido
        investido_obj.GetComponent<Objeto>().AtualizaValor(Info.total_investimentos);

    }


    //*****************************************************
    // GRAFICOS DE PROJECAO
    // 
    //*****************************************************

    void CriaGraficosProjecao()
    {
        Debug.Log("Nao finalizado!");

        /*
        //1. grafico capital + parcelas
        //cria obj3d - representa grafico linha
        GameObject gra = Instantiate(grafico1_prefab, Vector3.zero, Quaternion.identity);
        LineRenderer grafico_atual = gra.GetComponent<LineRenderer>();
        grafico_atual.material.color = Color.white;

        float valor_provisorio = Info.meta_valor_inicial;

        for (int n = 0; n <= Info.meta_meses; n++)
        {
            valor_provisorio += Info.meta_valor_mensal;
            Vector3 posicao = posicao_inicia_grafico;
            posicao.x += 1.1f * n;
            posicao.y = 10 * (valor_provisorio / Info.meta_valor_total);
            grafico_atual.SetPosition(n, posicao);
        }
        */

    }

}
