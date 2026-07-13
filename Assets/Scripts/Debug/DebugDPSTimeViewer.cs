using UnityEngine;


public class DebugDPSTimeViewer : MonoBehaviour
{
    [SerializeField] private bool SHOW_DSP_TIME = true; 
    void OnGUI()
    {
        if (!SHOW_DSP_TIME) 
            return;
        //Mostrar el tiempo actual del DSP en la esquina superior izquierda de la pantalla
        GUI.Label(new Rect(10, 10, 200, 20), "DSP Time: " + AudioSettings.dspTime.ToString("F2"));
    }
}