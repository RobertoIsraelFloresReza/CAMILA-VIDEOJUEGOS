using TMPro;
using UnityEngine;

public class ObjectiveDisplay : MonoBehaviour
{
    public TextMeshProUGUI objectiveText;

    public void UpdateObjective(string newObjective)
    {
        if (objectiveText != null)
        {
            objectiveText.text = "Objetivo: " + newObjective;
        }
    }
}
