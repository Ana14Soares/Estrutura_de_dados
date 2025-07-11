using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GerenciarUI_Substituicao : MonoBehaviour
{
    [Header("Títulos UI Sub")]
    public TMP_Text titulo_cena;
    public TMP_Text subtitulo;
    public TMP_Text caixa_texto;

    [Header("Referências UI")]
    public TMP_InputField campoDePesquisa;
    public TMP_Text tituloProblema;

    [Header("Resultados")]
    public GameObject templateSubstituicao;
    public Transform listaDeResultados;

    private string idiomaAtual = "pt";

    private void Start()
    {
        campoDePesquisa.onSubmit.AddListener(_ => Pesquisar());
        campoDePesquisa.onValueChanged.AddListener(_ => Pesquisar());

        if (templateSubstituicao != null)
        {
            templateSubstituicao.SetActive(false);
        }
        else
        {
            Debug.LogError("O template de substituição não foi atribuído no inspetor.");
        }

        TrocarIdioma(idiomaAtual); // idioma padrão
    }

    public void TrocarIdioma(string novoIdioma)
{
    CarregarBancoDeDadosSubstituicao.Carregar(novoIdioma);

    if (CarregarBancoDeDadosSubstituicao.TitulosUI != null)
    {
        titulo_cena.text = CarregarBancoDeDadosSubstituicao.TitulosUI.titulo_cena;
        subtitulo.text = CarregarBancoDeDadosSubstituicao.TitulosUI.subtitulo;
        caixa_texto.text = CarregarBancoDeDadosSubstituicao.TitulosUI.caixa_texto;
    }

    Pesquisar();
}


    public void Pesquisar()
    {
        if (CarregarBancoDeDadosSubstituicao.Substituicoes == null)
        {
            Debug.LogError("Lista de Substituições não foi carregada!");
            return;
        }

        foreach (Transform filho in listaDeResultados)
        {
            if (filho != templateSubstituicao.transform)
                Destroy(filho.gameObject);
        }

        string termo = campoDePesquisa.text.ToLower();

        List<Substituicao> lista = CarregarBancoDeDadosSubstituicao.Substituicoes;

        List<Substituicao> resultados;

        if (string.IsNullOrEmpty(termo))
        {
            resultados = lista;
        }
        else
        {
            resultados = lista
                .Where(p => p.titulo.ToLower().StartsWith(termo))
                .Concat(lista.Where(p => p.titulo.ToLower().Contains(termo) && !p.titulo.ToLower().StartsWith(termo)))
                .ToList();
        }

        foreach (var substituicao in resultados)
        {
            GameObject item = Instantiate(templateSubstituicao, listaDeResultados);
            item.SetActive(true);
            item.GetComponentInChildren<TMP_Text>().text = substituicao.titulo;

            Button botao = item.GetComponent<Button>();
            botao.onClick.RemoveAllListeners();
            // Nenhuma ação de clique
        }
    }

}

