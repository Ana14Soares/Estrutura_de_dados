using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class SingleObject : MonoBehaviour
{
    public GameObject placedObject;

    public void OnObjectPlaced(ARObjectPlacementEventArgs args)
    {
        // Destroi o objeto anteriormente colocado, se existir
        if (placedObject != null)
        {
            Destroy(placedObject);
        }

        // Atualiza a referência para o novo objeto colocado
        placedObject = args.placementObject;

        // Aqui você pode adicionar lógica adicional, como ativar um Canvas ou iniciar uma animação
    }
}
