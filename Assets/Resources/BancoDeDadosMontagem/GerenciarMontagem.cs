using UnityEngine;
using TMPro;

public class GerenciarMontagem : MonoBehaviour
{
    [Header("Referências UI Montagem")]
    public TMP_Text textoTitulo;
    public TMP_Text textoBotaoProjetar;
    public TMP_Text textoBotaoAssistir;
    public TMP_Text textoSubtitulo;
    

    private string idiomaAtual = "pt";

    private void Start()
    {
        TrocarIdioma(idiomaAtual);
    }

    public void TrocarIdioma(string novoIdioma)
    {
        idiomaAtual = novoIdioma;
        CarregarBancoDeDadosMontagem.Carregar(idiomaAtual);

        var dados = CarregarBancoDeDadosMontagem.DadosMontagem;
        textoTitulo.text = dados.titulo;
        textoBotaoProjetar.text = dados.botao_projetar;
        textoBotaoAssistir.text = dados.botao_assistir;
        textoSubtitulo.text = dados.subtitulo;
    }
}
