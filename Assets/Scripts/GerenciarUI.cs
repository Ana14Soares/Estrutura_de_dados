using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GerenciarUI : MonoBehaviour
{
    [Header("Referências UI")]
    public TMP_InputField campoDePesquisa;
    public GameObject painelDetalhes;
    public GameObject backgroundBlur;
    public TMP_Text tituloProblema;
    public TMP_Text tituloTitulosUI;
    public TMP_Text subtituloTitulosUI;
    public TMP_Text descricaoProblema;
    public Image imagemProblema;
    public TMP_Text solucaoProblema;

    [Header("Resultados")]
    public GameObject templateProblema;
    public Transform listaDeResultados;

    private CanvasGroup blurCanvasGroup;
    private string idiomaAtual = "pt";

    private void Start()
    {
        campoDePesquisa.onSubmit.AddListener(_ => Pesquisar());
        campoDePesquisa.onValueChanged.AddListener(_ => Pesquisar());

        templateProblema.SetActive(false);
        painelDetalhes.SetActive(false);

        blurCanvasGroup = backgroundBlur.GetComponent<CanvasGroup>();
        blurCanvasGroup.alpha = 0f;
        backgroundBlur.SetActive(false);

        idiomaAtual = IdiomaManager.Instance.ObterIdioma();
        
        TrocarIdioma(idiomaAtual); // Carrega o idioma padrão
    }

    public void TrocarIdioma(string novoIdioma)
    {
        idiomaAtual = novoIdioma;

        TextAsset arquivo = Resources.Load<TextAsset>($"banco_de_dados_{idiomaAtual}");

        if (arquivo != null)
        {
            Wrapper wrapper = JsonUtility.FromJson<Wrapper>(arquivo.text);

            // 🟢 Aqui atualiza o título e subtítulo da interface
            if (wrapper.titulos != null)
            {
                tituloTitulosUI.text = wrapper.titulos.titulo_cena;
                // Crie essa referência no Inspector para o subtítulo
                subtituloTitulosUI.text = wrapper.titulos.subtitulo;
            }

            // 🟢 Aqui carrega a lista de problemas
            CarregarBancoDeDados.Problemas = new List<Problema>(wrapper.problemas);

            Pesquisar(); // Atualiza a lista na tela
        }
        else
        {
            Debug.LogError($"Arquivo banco_de_dados_{idiomaAtual}.json não encontrado.");
        }

        painelDetalhes.SetActive(false);
    }


    public void Pesquisar()
    {
        foreach (Transform filho in listaDeResultados)
        {
            if (filho != templateProblema.transform)
                Destroy(filho.gameObject);
        }

        if (CarregarBancoDeDados.Problemas == null)
        {
            Debug.LogError("Lista de problemas não carregada!");
            return;
        }

        string termo = campoDePesquisa.text.ToLower();

        List<Problema> resultados;
        if (string.IsNullOrEmpty(termo))
        {
            resultados = CarregarBancoDeDados.Problemas;
        }
        else
        {
            resultados = CarregarBancoDeDados.Problemas
                .Where(p => p.titulo.ToLower().StartsWith(termo))
                .Concat(CarregarBancoDeDados.Problemas
                    .Where(p => p.titulo.ToLower().Contains(termo) && !p.titulo.ToLower().StartsWith(termo)))
                .ToList();
        }

        foreach (var problema in resultados)
        {
            GameObject item = Instantiate(templateProblema, listaDeResultados);
            item.SetActive(true);
            item.GetComponentInChildren<TMP_Text>().text = problema.titulo;

            Button botao = item.GetComponent<Button>();
            botao.onClick.RemoveAllListeners();
            botao.onClick.AddListener(() => MostrarDetalhes(problema));
        }
    }

    void MostrarDetalhes(Problema problema)
    {
        tituloProblema.text = problema.titulo;
        descricaoProblema.text = problema.descricao;
        solucaoProblema.text = problema.solucao;

        if (!string.IsNullOrEmpty(problema.imagem))
        {
            Sprite sprite = Resources.Load<Sprite>(problema.imagem);
            imagemProblema.sprite = sprite;
            imagemProblema.gameObject.SetActive(sprite != null);
        }
        else
        {
            imagemProblema.gameObject.SetActive(false);
        }

        painelDetalhes.SetActive(true);

        RectTransform rt = painelDetalhes.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, -1080);

        StartCoroutine(FadeIn(0.4f));
        StartCoroutine(AnimarPopup(rt, new Vector2(0, 0), 0.4f));
    }

    public void OcultarPainelDetalhes()
    {
        StartCoroutine(FecharPopup());
    }

    IEnumerator AnimarPopup(RectTransform painel, Vector2 destino, float duracao)
    {
        Vector2 origem = painel.anchoredPosition;
        float tempo = 0f;

        while (tempo < duracao)
        {
            painel.anchoredPosition = Vector2.Lerp(origem, destino, tempo / duracao);
            tempo += Time.deltaTime;
            yield return null;
        }

        painel.anchoredPosition = destino;
    }

    IEnumerator FecharPopup()
    {
        RectTransform rt = painelDetalhes.GetComponent<RectTransform>();
        Vector2 origem = rt.anchoredPosition;
        Vector2 destino = new Vector2(0, -1080);
        float duracao = 0.4f;
        float tempo = -0.2f;

        StartCoroutine(FadeOut(duracao));

        while (tempo < duracao)
        {
            rt.anchoredPosition = Vector2.Lerp(origem, destino, tempo / duracao);
            tempo += Time.deltaTime;
            yield return null;
        }

        rt.anchoredPosition = destino;
        painelDetalhes.SetActive(false);
    }

    IEnumerator FadeIn(float duracao)
    {
        backgroundBlur.SetActive(true);
        blurCanvasGroup.alpha = 0f;

        float tempo = 0f;
        while (tempo < duracao)
        {
            blurCanvasGroup.alpha = Mathf.Lerp(0f, 1f, tempo / duracao);
            tempo += Time.deltaTime;
            yield return null;
        }

        blurCanvasGroup.alpha = 1f;
    }

    IEnumerator FadeOut(float duracao)
    {
        float tempo = 0f;
        float inicio = blurCanvasGroup.alpha;

        while (tempo < duracao)
        {
            blurCanvasGroup.alpha = Mathf.Lerp(inicio, 0f, tempo / duracao);
            tempo += Time.deltaTime;
            yield return null;
        }

        blurCanvasGroup.alpha = 0f;
        backgroundBlur.SetActive(false);
    }

    [System.Serializable]
    private class Wrapper
    {
        public TitulosUI titulos;

        public Problema[] problemas;
    }
}


