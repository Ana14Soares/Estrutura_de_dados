using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TMPro; // <-- Importa o TextMesh Pro


public class LocalizationManager : MonoBehaviour
{
    public static event Action OnLanguageChanged;
    public TextMeshProUGUI titleText; // <-- Agora aceita TMP


    public void SetLanguageToEnglish()
    {
    LoadLocalizedText("en.json");
    LoadLocalizedText("banco_de_dado_en.json");
    }

    public void SetLanguageToPortuguese()
    {
    LoadLocalizedText("pt.json");
    LoadLocalizedText("banco_de_dado_pt.json");
    }

    private Dictionary<string, string> localizedText;


    public bool TryGetTranslation(string key, out string value)
    {
        if (localizedText != null && localizedText.ContainsKey(key))
        {
            value = localizedText[key];
            return true;
        }

        value = null;
        return false;
    }

    public void LoadLocalizedText(string fileName)
    {
        Debug.Log("Tentando carregar: " + fileName);
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            localizedText = new Dictionary<string, string>();
            foreach (LocalizationItem item in loadedData.items)
            {
                localizedText[item.key] = item.value;
            }

            ApplyLocalization();
            OnLanguageChanged?.Invoke();
        }
        else
        {
            Debug.LogError("Arquivo de localização não encontrado: " + filePath);
        }
    }

    private void ApplyLocalization()
    {
        Debug.Log("Aplicando tradução...");
        if (localizedText.ContainsKey("titulo"))
        {
            titleText.text = localizedText["titulo"];
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

}


