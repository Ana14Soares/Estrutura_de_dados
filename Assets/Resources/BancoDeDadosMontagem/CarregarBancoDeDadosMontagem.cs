using UnityEngine;

public class CarregarBancoDeDadosMontagem : MonoBehaviour
{
    public static MontagemTextos DadosMontagem { get; private set; }

    public static void Carregar(string idioma)
    {
        TextAsset arquivo = Resources.Load<TextAsset>($"BancoDeDadosMontagem/banco_montagem_{idioma}");

        if (arquivo != null)
        {
            Wrapper wrapper = JsonUtility.FromJson<Wrapper>(arquivo.text);
            DadosMontagem = wrapper.montagem;
        }
        else
        {
            Debug.LogError($"Arquivo banco_montagem_{idioma}.json não encontrado!");
            DadosMontagem = new MontagemTextos(); // vazio
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public MontagemTextos montagem;
    }

    [System.Serializable]
    public class MontagemTextos
    {
        public string botao_projetar;
        public string botao_assistir;
        public string titulo;
        public string subtitulo;
    }
}
