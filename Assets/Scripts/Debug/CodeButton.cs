using System.Diagnostics;
using System.IO;
using UnityEngine;

public class CodeButton : MonoBehaviour
{
// Puedes asignar una ruta específica desde el Inspector, o dejarla en blanco para usar persistentDataPath
    [SerializeField] private string customPath = "";

    public void OpenSpecificFolder()
    {
        // Si no definiste una ruta en el inspector, usa persistentDataPath por defecto
        string targetPath = string.IsNullOrEmpty(customPath) ? Application.persistentDataPath : customPath;

        // Validar que la ruta sea segura y exista
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        // El bloque #if se evalúa al compilar el juego
        #if UNITY_STANDALONE_WIN
            // Formatea la ruta con comillas dobles para evitar errores si tiene espacios
            Process.Start("explorer.exe", $"\"{targetPath.Replace('/', '\\')}\"");
        
        #elif UNITY_STANDALONE_LINUX
            // xdg-open abre el gestor de archivos nativo de la distribución Linux
            Process.Start("xdg-open", $"\"{targetPath}\"");
            
        #else
            UnityEngine.Debug.LogWarning("Plataforma no soportada para apertura automática de carpetas.");
        #endif
    }
}
