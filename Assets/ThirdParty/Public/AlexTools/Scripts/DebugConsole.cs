using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// Consola de depuración visual en pantalla. Muestra mensajes, advertencias y errores directamente en el juego.
/// Puede incluir timestamp, logs de Unity y mostrar los FPS.
/// </summary>
public class DebugConsole : SingletonMonoBehaviour<DebugConsole>
{
    //Configuración del Singleton
    public override string PrefabPath => "Prefabs/DebugConsole";
    public override bool Persistant => true;

    [Header("Settings")]
    [SerializeField, Tooltip("Mostrar el tiempo en los logs")]
    private bool showTimestamp = true;
    [SerializeField, Tooltip("Se muestran los logs de Unity")]
    private bool logUnityLogs = true;
    [SerializeField, Tooltip("Mostrar los FPS del juego en una esquina")]
    private bool showFps = true;
    [SerializeField, Tooltip("Definir si la consola se va a iniciar")]
    private bool start = true;
    [SerializeField, Tooltip("Número máximo de caracteres que se pueden mostrar en consola"), Range(1000, 50000)]
    private int maxLength = 15000;
    [SerializeField, Tooltip("Intervalo de actualización de FPS en segundos"), Range(0.1f, 2f)]
    private float fpsUpdateInterval = 0.5f;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI consoleTextPanel;
    [SerializeField] private GameObject consolePanel;
    [SerializeField] private GameObject maximizeButton;
    [SerializeField] private TextMeshProUGUI fpsTextPanel;
    [SerializeField] private GameObject fpsPanel;

    //Privados
    private readonly StringBuilder logBuffer = new StringBuilder();
    private readonly object logLock = new object();
    private float lastFpsUpdate;
    private float fpsAccumulator;
    private int fpsFrameCount;

    /// <summary>
    /// Inicializa la consola. Configura los listeners y establece la instancia.
    /// </summary>
    protected override void OnInstantiate()
    {
        if (!start)
        { 
            gameObject.SetActive(false);
            return;
        }

        InitializeConsole();
    }

    /// <summary>
    /// Limpia los eventos registrados al destruir la instancia.
    /// </summary>
    private void OnDisable()
    {
        if (Instance == this && logUnityLogs)
        {
            Application.logMessageReceived -= OnUnityLog;
        }
    }

    /// <summary>
    /// Actualiza la visualización de FPS en pantalla con optimización de rendimiento.
    /// </summary>
    private void Update()
    {
        if (showFps && fpsTextPanel != null)
        {
            UpdateFPSDisplay();
        }
    }

    /// <summary>
    /// Oculta la consola de depuración.
    /// </summary>
    public void Minimize()
    {
        if (maximizeButton != null) maximizeButton.SetActive(true);
        if (consolePanel != null) consolePanel.SetActive(false);
    }

    /// <summary>
    /// Muestra la consola de depuración.
    /// </summary>
    public void Maximize()
    {
        if (maximizeButton != null) maximizeButton.SetActive(false);
        if (consolePanel != null) consolePanel.SetActive(true);
    }

    /// <summary>
    /// Limpia todo el contenido de la consola.
    /// </summary>
    public void ClearConsole()
    {
        lock (logLock)
        {
            logBuffer.Clear();
            if (consoleTextPanel != null)
            {
                consoleTextPanel.text = string.Empty;
            }
        }
    }

    /// <summary>
    /// Imprime un mensaje en consola como log normal.
    /// </summary>
    public static void Log(object message)
    {
        if (message == null) return;
        
        string completeMessage = $"[{nameof(DebugConsole)}] {message}";
        Debug.Log(completeMessage);
        LogMessage(completeMessage, "white");
    }

    /// <summary>
    /// Imprime un mensaje en consola como advertencia.
    /// </summary>
    public static void LogWarning(object message)
    {
        if (message == null) return;
        
        string completeMessage = $"[{nameof(DebugConsole)}] {message}";
        Debug.LogWarning(completeMessage);
        LogMessage(completeMessage, "yellow");
    }

    /// <summary>
    /// Imprime un mensaje en consola como error.
    /// </summary>
    public static void LogError(object message)
    {
        if (message == null) return;
        
        string completeMessage = $"[{nameof(DebugConsole)}] {message}";
        Debug.LogError(completeMessage);
        LogMessage(completeMessage, "red");
    }

    /// <summary>
    /// Inicializa los componentes de la consola.
    /// </summary>
    private void InitializeConsole()
    {
        if (consolePanel != null) consolePanel.SetActive(true);
        if (fpsPanel != null) fpsPanel.SetActive(showFps);

        if (logUnityLogs)
            Application.logMessageReceived += OnUnityLog;

        // Inicializar FPS
        lastFpsUpdate = Time.time;
        fpsAccumulator = 0f;
        fpsFrameCount = 0;
    }

    /// <summary>
    /// Actualiza la visualización de FPS con mejor rendimiento.
    /// </summary>
    private void UpdateFPSDisplay()
    {
        fpsAccumulator += Time.deltaTime;
        fpsFrameCount++;

        if (Time.time - lastFpsUpdate >= fpsUpdateInterval)
        {
            int fps = Mathf.RoundToInt(fpsFrameCount / fpsAccumulator);
            fpsTextPanel.text = $"{fps} FPS";
            
            fpsAccumulator = 0f;
            fpsFrameCount = 0;
            lastFpsUpdate = Time.time;
        }
    }

    /// <summary>
    /// Añade un mensaje coloreado a la consola visual de forma thread-safe.
    /// </summary>
    private static void LogMessage(string message, string color)
    {
        if (Instance == null || Instance.consoleTextPanel == null) return;

        lock (Instance.logLock)
        {
            string timestamp = Instance.showTimestamp ? $"[{System.DateTime.Now:HH:mm:ss}] " : string.Empty;
            string formattedMessage = $"\n<color={color}>{timestamp}{message}</color>";
            
            Instance.logBuffer.Append(formattedMessage);
            Instance.consoleTextPanel.text = Instance.logBuffer.ToString();
            
            Instance.TrimLogsIfNeeded();
        }
    }

    /// <summary>
    /// Captura los mensajes del sistema de logging de Unity y los muestra según su tipo.
    /// </summary>
    private void OnUnityLog(string logString, string stackTrace, LogType type)
    {
        if (string.IsNullOrEmpty(logString)) return;

        string color = type switch
        {
            LogType.Error or LogType.Exception => "red",
            LogType.Warning => "yellow",
            LogType.Log => "white",
            LogType.Assert => "orange",
            _ => "white"
        };

        LogMessage(logString, color);
    }

    /// <summary>
    /// Recorta el texto si excede el límite de caracteres, preservando los mensajes más recientes.
    /// </summary>
    private void TrimLogsIfNeeded()
    {
        if (logBuffer.Length <= maxLength) return;

        // Encontrar el primer salto de línea después del punto de corte
        int cutPoint = logBuffer.Length - maxLength;
        int newlineIndex = logBuffer.ToString().IndexOf('\n', cutPoint);
        
        if (newlineIndex > 0)
        {
            string trimmedContent = logBuffer.ToString().Substring(newlineIndex);
            logBuffer.Clear();
            logBuffer.Append(trimmedContent);
            consoleTextPanel.text = logBuffer.ToString();
        }
    }
}
