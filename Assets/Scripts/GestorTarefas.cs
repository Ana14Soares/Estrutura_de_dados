using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using TMPro;

public class GestorTarefas : MonoBehaviour
{
    private IntPtr listaPtr;
    private IntPtr pilhaConcluidasPtr;
    private IntPtr pilhaDeletadasPtr;
    private IntPtr pilhaAcoesPtr;

    [Header("Referências")]
    public Transform content;
    public GameObject tarefaPrefab;
    public TMP_InputField campoDeletar;

#if UNITY_EDITOR
    // Simulação no Editor
    private Stack<string> pilhaAcoesEditor = new Stack<string>();
    private Stack<string> pilhaConcluidasEditor = new Stack<string>();
    private Stack<string> pilhaDeletadasEditor = new Stack<string>();
#endif

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        listaPtr = Marshal.AllocHGlobal(1024);
        pilhaConcluidasPtr = Marshal.AllocHGlobal(1024);
        pilhaDeletadasPtr = Marshal.AllocHGlobal(1024);
        pilhaAcoesPtr = Marshal.AllocHGlobal(1024);

        TrabalhoNativo.Lista_cria(listaPtr);
        TrabalhoNativo.Pilha_create(pilhaConcluidasPtr, 64, 50, 0);
        TrabalhoNativo.Pilha_create(pilhaDeletadasPtr, 64, 50, 0);
        TrabalhoNativo.Pilha_create(pilhaAcoesPtr, 64, 50, 0);

        Debug.Log("Estruturas criadas.");
#else
        Debug.Log("Editor - estruturas simuladas.");
#endif
    }

    public void AdicionarTarefa(string nomeTarefa)
    {
        Debug.Log($"AdicionarTarefa chamada com '{nomeTarefa}'");

#if UNITY_ANDROID && !UNITY_EDITOR
        TrabalhoNativo.Lista_inserir(listaPtr, nomeTarefa);
#else
        // No editor, só loga
#endif

        GameObject novaTarefa = Instantiate(tarefaPrefab, content);
        TMP_Text label = novaTarefa.GetComponentInChildren<TMP_Text>();
        if (label != null)
            label.text = nomeTarefa;

        Button btn = novaTarefa.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => ConcluirTarefa(novaTarefa, nomeTarefa));
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        TrabalhoNativo.Pilha_push(pilhaAcoesPtr, "ADICIONAR|" + nomeTarefa);
#else
        pilhaAcoesEditor.Push("ADICIONAR|" + nomeTarefa);
#endif
    }

    public void ConcluirTarefa(GameObject tarefaObj, string nomeTarefa)
    {
        Debug.Log($"Concluindo tarefa '{nomeTarefa}'.");

#if UNITY_ANDROID && !UNITY_EDITOR
        TrabalhoNativo.Lista_removerPorNome(listaPtr, nomeTarefa);
        TrabalhoNativo.Pilha_push(pilhaConcluidasPtr, nomeTarefa);
        TrabalhoNativo.Pilha_push(pilhaAcoesPtr, "CONCLUIR|" + nomeTarefa);
#else
        pilhaConcluidasEditor.Push(nomeTarefa);
        pilhaAcoesEditor.Push("CONCLUIR|" + nomeTarefa);
#endif

        TMP_Text label = tarefaObj.GetComponentInChildren<TMP_Text>();
        if (label != null)
            label.text = "[OK] " + nomeTarefa;

        Button btn = tarefaObj.GetComponent<Button>();
        if (btn != null)
            btn.interactable = false;
    }

    public void DeletarTarefaPorNome()
    {
        string nome = campoDeletar.text.Trim();
        if (string.IsNullOrEmpty(nome))
        {
            Debug.LogWarning("Nenhum nome digitado.");
            return;
        }

        bool encontrado = false;

        foreach (Transform filho in content)
        {
            TMP_Text label = filho.GetComponentInChildren<TMP_Text>();
            if (label != null && label.text.Contains(nome))
            {
                encontrado = true;

#if UNITY_ANDROID && !UNITY_EDITOR
                TrabalhoNativo.Lista_removerPorNome(listaPtr, nome);
                TrabalhoNativo.Pilha_push(pilhaDeletadasPtr, nome);
                TrabalhoNativo.Pilha_push(pilhaAcoesPtr, "DELETAR|" + nome);
#else
                pilhaDeletadasEditor.Push(nome);
                pilhaAcoesEditor.Push("DELETAR|" + nome);
#endif

                filho.gameObject.SetActive(false);
                Debug.Log($"Tarefa '{nome}' deletada.");
                break;
            }
        }

        if (!encontrado)
        {
            Debug.LogWarning($"Tarefa '{nome}' não encontrada.");
        }
    }

    public void DesfazerUltimaAcao()
    {
        Debug.Log("Tentando desfazer última ação.");

#if UNITY_ANDROID && !UNITY_EDITOR
        if (TrabalhoNativo.Pilha_isEmpty(pilhaAcoesPtr))
        {
            Debug.Log("Nada para desfazer.");
            return;
        }

        StringBuilder buffer = new StringBuilder(64);
        bool sucesso = TrabalhoNativo.Pilha_pop(pilhaAcoesPtr, buffer);
        if (!sucesso)
        {
            Debug.LogWarning("Erro ao desempilhar ação.");
            return;
        }

        string acao = buffer.ToString();
        string[] partes = acao.Split('|');
        if (partes.Length != 2)
        {
            Debug.LogWarning("Formato de ação inválido.");
            return;
        }

        string tipo = partes[0];
        string nomeTarefa = partes[1];
#else
        if (pilhaAcoesEditor.Count == 0)
        {
            Debug.Log("Nada para desfazer.");
            return;
        }

        string acao = pilhaAcoesEditor.Pop();
        string[] partes = acao.Split('|');
        string tipo = partes[0];
        string nomeTarefa = partes[1];
#endif

        Debug.Log($"Desfazendo ação: {tipo} '{nomeTarefa}'.");

        if (tipo == "CONCLUIR")
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            StringBuilder bufTarefa = new StringBuilder(64);
            TrabalhoNativo.Pilha_pop(pilhaConcluidasPtr, bufTarefa);
            TrabalhoNativo.Lista_inserir(listaPtr, nomeTarefa);
#endif
            bool restaurado = false;
            foreach (Transform filho in content)
            {
                TMP_Text label = filho.GetComponentInChildren<TMP_Text>();
                if (label != null && label.text == "[OK] " + nomeTarefa)
                {
                    label.text = nomeTarefa;

                    Button btn = filho.GetComponent<Button>();
                    if (btn != null)
                        btn.interactable = true;

                    restaurado = true;
                    Debug.Log($"Tarefa '{nomeTarefa}' restaurada.");
                    break;
                }
            }
            if (!restaurado)
                Debug.LogWarning($"Botão '[OK] {nomeTarefa}' não encontrado para restaurar.");
        }
        else if (tipo == "DELETAR")
{
#if UNITY_ANDROID && !UNITY_EDITOR
    StringBuilder bufTarefa = new StringBuilder(64);
    TrabalhoNativo.Pilha_pop(pilhaDeletadasPtr, bufTarefa);
    TrabalhoNativo.Lista_inserir(listaPtr, nomeTarefa);
#endif
    GameObject novaTarefa = Instantiate(tarefaPrefab, content);
    TMP_Text label = novaTarefa.GetComponentInChildren<TMP_Text>();
    if (label != null)
        label.text = nomeTarefa;

    Button btn = novaTarefa.GetComponent<Button>();
    if (btn != null)
    {
        btn.onClick.AddListener(() => ConcluirTarefa(novaTarefa, nomeTarefa));
        btn.interactable = true;
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    TrabalhoNativo.Pilha_push(pilhaAcoesPtr, "ADICIONAR|" + nomeTarefa);
#else
    pilhaAcoesEditor.Push("ADICIONAR|" + nomeTarefa);
#endif

    Debug.Log($"Tarefa '{nomeTarefa}' recriada e ação ADICIONAR registrada.");
    }

        else if (tipo == "ADICIONAR")
        {
            bool removido = false;
            foreach (Transform filho in content)
            {
                TMP_Text label = filho.GetComponentInChildren<TMP_Text>();
                if (label != null && label.text == nomeTarefa)
                {
                    Destroy(filho.gameObject);
                    removido = true;
                    Debug.Log($"Tarefa '{nomeTarefa}' removida ao desfazer adição.");
                    break;
                }
            }
            if (!removido)
            {
                Debug.LogWarning($"Tarefa '{nomeTarefa}' não encontrada para remoção.");
            }
        }
    }

    void OnDestroy()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Marshal.FreeHGlobal(listaPtr);
        Marshal.FreeHGlobal(pilhaConcluidasPtr);
        Marshal.FreeHGlobal(pilhaDeletadasPtr);
        Marshal.FreeHGlobal(pilhaAcoesPtr);
#endif
    }
}