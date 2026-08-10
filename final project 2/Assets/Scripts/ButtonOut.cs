using UnityEngine;

public class QuitHandler : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Гра закривається..."); // Це щоб ти бачив у консолі, що кнопка спрацювала

        // Якщо гра запущена в редакторі Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // Якщо це вже скомільована гра (.exe, .apk тощо)
            Application.Quit();
#endif
    }
}