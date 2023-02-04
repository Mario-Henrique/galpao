using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiaNoite : MonoBehaviour
{
    /// <sumary>
    /// Sistema que copia os movimentos de Rotação e Translação da Terra.
    /// Rotação: Movimento da Terra em seu próprio eixo. Aqui representado pelo eixo X. 23 horas 56 minutos 4 segundos e 9 centésimos.
    /// Translação: Movimento da Terra em torno do Sol. Aqui representado pelo eixo Y. 365 dias 5 horas e 47 minutos.
    ///             A cada 4 anos adicionamos mais 1 dia tendo assim o ano bissexto, para compensar as quase seis horas não contabilizadas
    ///             nos outros anos.
    /// A linha imaginária chamada de eixo da Terra atravessa o planeta do polo norte ao polo sul. Essa inclinação é de aproximadamente 23,5 graus.
    /// A existência de dia e noite é resultado do movimento de rotação no sentido anti-horário. A velocidade média dessa rotação é de 1674km/h.
    /// Esse cálculo é feito tomando a linha do Equador como referência. Ela possui aproximadamente 12.756 quilômetros.
    /// Para calcular a posição do sol durante o dia basta multiplicar a hora por 15 que é o grau de rotação. (HH,MM * 15º = PosiçãoSolEmGraus).
    /// Assim sabendo que o sol está em 0º às 6hs da manhã, cada hora adicional aumenta 15º.
    /// 6hs = ? >> 0 * 15 = 0º || 7hs = ? >> 1 * 15 = 15º || 12hs = ? >> 6 * 15 = 90º
    /// Para os minutos precisamos multiplicar por 1,666666666666667, que é 100 dividido por 60 já que temos 60 minutos. Ou seja 10 minutos serão
    /// 10 * 1,666666666666667 = 16,66666666666667, sendo assim 6:10hs = ? >> 0,1666666666666667 * 15 = 2,50º
    ///
    /// 1 dia = 23,9344694444444443 horas = 1.436,068166666666 minutos = 86.164,08999999998 segundos = 861.6408999999998 centésimos de segundo
    /// </sumary>
    
    /// <summary>
    /// Objeto que representa o sol na cena
    /// </summary>
    [Tooltip("Directional Light que representa o sol")]
    public Transform sol;

    [Tooltip("TextMesh para exibir a data e hora do jogo.")]
    public TextMeshProUGUI dataText;

    /// <summary>
    /// Quanto tempo dura 1 dia em segundos
    /// </summary>
    [Tooltip("Quanto tempo dura 1 dia em segundos. Obs: 1 dia real dura 86164 segundos. Não usar acima de 1 dia real.")]
    public int duracaoDiaInGame;

    /// <summary>
    /// Dia de 1-31
    /// </summary>
    [Tooltip("Data que representa o dia de 1-31")]
    [Range(1,31)]
    public int dia = 1;

    /// <summary>
    /// Mês de 1-12
    /// </summary>
    [Tooltip("Mês de 1-12")]
    [Range(1,12)]
    public int mes = 1;

    /// <summary>
    /// Ano de 0 - ?
    /// </summary>
    [Tooltip("Ano de 0 - ?")]
    public int ano = 0;

    /// <summary>
    /// Hora corrente no jogo de 0-23
    /// </summary>
    [Tooltip("Hora corrente no jogo de 0-23")]
    [Range(0,23)]
    public int hora = 0;

    /// <summary>
    /// Minuto corrente no jogo de 0-59
    /// </summary>
    [Tooltip("Minuto corrente no jogo de 0-59")]
    [Range(0,59)]
    public int minuto = 0;

    /// <summary>
    /// Segundo corrente no jogo de 0-59
    /// </summary>
    [Tooltip("Segundo corrente no jogo de 0-59")]
    [Range(0,59)]
    public float segundo = 0;

    // Constante que define o valor de 1 minuto de base 100. 100/60s = 1.666...7;
    private const float const_minuto_100 = 1.6666666667f;

    // Constante que define o valor de 1 dia em minutos
    private const float const_dia_em_minuto = 1436.068166666666f;

    // Constante que define a duração de 1 dia real em segundos.
    private const int duracaoDiaReal = 86164;

    // Segundos correntes no jogo baseado na duração do dia real
    private float segundos = 0;

    // Dias correntes no jogo 1-366
    private int dias = 0;

    // Multiplicador da duração do dia.
    private float multiplicador;

    // Cria apenas uma instância desta classe. Padrão Singleton
    public static DiaNoite Instance {get; private set;}
    private void Awake() {
        if (Instance != null && Instance != this){
            Destroy(gameObject);
        } else{
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Inicializa os parâmetros do dia
    /// </summary>
    void Start()
    {
        multiplicador = duracaoDiaReal / duracaoDiaInGame;

        // Caso inicie com uma data maior do que existe no mês, será o último dia do mês
        if (dia > DiasNoMes(mes))
            dia = DiasNoMes(mes);

        //Calcular e setar quantos dias corridos se passaram no jogo
        for (int i=1; i <= mes; i++){
            if (i == mes)
                dias += dia;
            else
                dias += DiasNoMes(i);
        }

        // Calcular quantos segundos corridos se passaram no jogo
        segundos = SegundosNoDia();

    }

    /// <summary>
    /// Gerencia o relógio a partir da data e hora que o jogo foi inicializado
    /// </summary>
    void Update()
    {
        // Gerencia a passagem de 1 dia no jogo
        segundos += Time.deltaTime * multiplicador;
        if(segundos >= duracaoDiaReal){
            segundos = 0;
            dias++;
        }

        CalcularHorario();
        PosicionarSol(HoraEmGraus(hora, minuto),DiasEmGraus(dias));
        
        dataText.text = "Data: " + getData().dia + "/" + getData().mes + "/" + getData().ano;
        dataText.text += "\nHora: " + getHora().hora + ":" + getHora().minuto + ":" + Mathf.Round(getHora().segundo);

        // Inicia o gerenciador de intensidade de luz
        Claridade();
    }

    // Retorna quantos segundos se passaram no jogo durante 1 dia.
    private float SegundosNoDia(){
        return (hora * 60 + minuto) * 60 + segundo;
    }

    /// <summary>
    /// Rotação. Recebe hora e minuto e devolve a posição do sol em graus. Eixo X
    /// </summary>
    /// <param name="hora" type="double">Hora do dia padrão 24hs. 0-23</param>
    /// <param name="minuto" type="double">Minuto do dia. 0-59</param>
    public float HoraEmGraus(int hora, int minuto){
        float min = minuto * const_minuto_100 / 100;
        float _hora = (hora - 6 + min) * 15;

        return _hora;
    }

    /// <summary>
    /// Translação. Recebe o número de dias e devolve a posição do sol em graus. Eixo Y
    /// 22 Dezembro(356)    / 21 Março(80)     = 315º - 270º
    /// 21 Março(80 dias)   / 22 Junho(173)    = 270º - 225º
    /// 22 Junho(173)       / 23 Setembro(266) = 225º - 270º
    /// 23 Setembro(266)    / 22 Dezembro(356) = 270º - 315º
    /// </summary>
    /// <param name="dias" type="double">Número de dias corridos do ano. 1-365</param>
    public float DiasEmGraus(int dias){
        // Posição em graus para o eixo Y do sol.
        float posInGraus = 0f;

        // Valor de 45 graus dividido por 90 dias.
        float multiplicador_graus = 0.5f;

        // diferença de dias para cálculo da posição Y do sol.
        int dia = 0;

        // Total de dias do ano
        int diasAno = 0;
        if (Bissexto(ano))
            diasAno = 366;
        else
            diasAno = 365;

        switch (dias)
        {
            // Dezembro até Março (315-270)
            case >= 356:
                dia = diasAno - dias;
                posInGraus = 315 - (dia * multiplicador_graus);
                break;

            case <= 80:
                dia = (diasAno - 356) + dias;
                posInGraus = 315 - (dia * multiplicador_graus);
                break;
            
            // Março até Junho (270-225)
            case <= 173:
                dia = dias - 80;
                posInGraus = 270 - dia * multiplicador_graus;
                break;

            // Junho até Setembro (225-270)
            case <= 266:
                dia = dias - 173;
                posInGraus = 225 + dia * multiplicador_graus;
                break;

            // Setembro até Dezembro (270-315)
            case < 356:
                dia = dias - 266;
                posInGraus = 270 + dia * multiplicador_graus;
                break;
        }

        return posInGraus;
    }

    /// <summary>
    /// Gerencia a data e horario do jogo
    /// </summary>
    private void CalcularHorario(){
        segundo += Time.deltaTime * multiplicador;
        if (segundo > 59){
            segundo = 0;
            minuto++;
        }

        if (minuto > 59){
            minuto = 0;
            hora++;
        }

        if (hora > 23){
            hora = 0;
            dia++;
        }
        
        if (dia > DiasNoMes(mes)) {
            dia = 1;
            mes++;
        } 

        if (mes > 12){
            mes = 1;
            ano++;
            dias = 1;
        }
    }

    /// <summary>
    /// Recebe um GameObject que representa o sol e define sua posição de acordo com a hora e o dia no calendário anual.
    /// </summary>
    /// <param name="sol" type="GameObject">Objeto que representa o sol na cena</param>
    /// <param name="horaEmGraus" type="float">A hora do dia em graus</param>
    /// <param name="diaEmGraus" type="float">O dia do ano em graus</param>
    public void PosicionarSol(float horaEmGraus, float diaEmGraus){
        Quaternion rtAnterior = sol.rotation;
        Quaternion rtNova = Quaternion.Euler(new Vector3(horaEmGraus, diaEmGraus, 0f));

        //float rt = Mathf.Lerp(rtAnterior, rtNova, segundos/duracaoDiaReal);
        sol.rotation = Quaternion.Lerp(rtAnterior, rtNova, 0.01f);
    }

    // Constante que define o valor da intensidade de luz por minuto do jogo.
    // De 0 à 720 minutos soma-se a constante. De 720 a 1440 diminui-se.
    float const_exposure = 0.0069444444444444f;

    // Controla a intensidade de luz durante o dia e a noite
    private void Claridade(){
        float alpha = (segundos / 60) * const_exposure;
        float waitTime = duracaoDiaInGame / const_dia_em_minuto;

        switch (hora)
        {
            case <=12:
                alpha = (SegundosNoDia() / 60) * const_exposure;
                break;
            
            case > 12:
                alpha = 5 - (((SegundosNoDia() / 60) * const_exposure) - 5);
                break;
        }
        RenderSettings.skybox.SetFloat("_Exposure", alpha);
    }

    /// <summary>
    /// Retorna a data atual do jogo
    /// </summary>
    public (int dia, int mes, int ano) getData() => (dia, mes, ano);

    /// <summary>
    /// Retorna a Hora atual do jogo
    /// </summary>
    public (int hora, int minuto, float segundo) getHora() => (hora, minuto, segundo);

    // Retorna quantos dias tem o mês
    private int DiasNoMes(int mes){
        int qtdDias = 0;

        if (mes==2 && Bissexto(ano))
            qtdDias = 29;
        else if (mes==2 && !Bissexto(ano))
            qtdDias = 28;
        else if(mes != 2 && (mes%2==0 && mes < 7) || (mes%2!=0 && mes > 8))
            qtdDias = 30;
        else if (mes != 2 && (mes%2==0 && mes > 7) || (mes%2!=0 && mes < 8))
            qtdDias = 31;
        
        return qtdDias;
    }

    // Retorna Verdadeiro se o ano for bissexto
    private bool Bissexto(int ano){
        float _ano = ano / 4f;

        if (_ano % 2 == 0 || _ano % 2 == 1)
            return true;
        else
            return false;
    }
}
