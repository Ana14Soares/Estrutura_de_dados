using UnityEngine;
using System.Collections.Generic;

public class CarregarBancoDeDadosSubstituicao : MonoBehaviour
{
    public static List<Substituicao> Substituicoes { get; set; }
    public static TitulosUI_sub TitulosUI { get; set; }

    public static void Carregar(string idioma)
    {
        TextAsset arquivo = Resources.Load<TextAsset>($"banco_de_dados_substituicoes_{idioma}");

        if (arquivo != null)
        {
            Wrapper wrapper = JsonUtility.FromJson<Wrapper>(arquivo.text);
            Substituicoes = new List<Substituicao>(wrapper.substituicoes);
            TitulosUI = wrapper.titulos;
        }
        else
        {
            Debug.LogError($"Arquivo banco_de_dados_substituicoes_{idioma}.json não encontrado em Resources!");
            Substituicoes = new List<Substituicao>();
            TitulosUI = null;
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public Substituicao[] substituicoes;
        public TitulosUI_sub titulos;
    }
}


