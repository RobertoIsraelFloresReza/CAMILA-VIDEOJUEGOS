using TMPro;
using UnityEngine;

public class EnemyCountDisplay : MonoBehaviour
{
    public TextMeshProUGUI countText;
    
    public void SetCount(int count)
    {
        if (countText != null)
        {
            countText.text = "Enemigos restantes: " + count.ToString();

            if (count <= 0)
            {
                countText.text = "¡Todos los enemigos eliminados!";
            }
        }
    }
}
