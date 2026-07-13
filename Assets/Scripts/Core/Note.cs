using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    public Sprite[] sprites; //array de sprites para diferentes tipos de notas, se puede ajustar segun sea necesario

    [SerializeField]
    private double targetDPSTime;
    private bool canBePressed;
    private float speed = 3f; //velocidad a la que se mueve el note, se puede ajustar segun sea necesario
    private List<KeyCode> keys = new List<KeyCode>();
    private bool exitByHit = false;

    [Header("Debug")]
    [SerializeField] private double currentDspTime;
    [SerializeField] private double lastDspTime;
    [SerializeField] private double ElapsedDpsTime => currentDspTime - lastDspTime;

    private bool gameover = false;
    public void Initialize(double targetDPSTime, int railIndex, float noteSpeed)
    {
        this.targetDPSTime = targetDPSTime;
        speed = noteSpeed;
        SetNoteSprite(railIndex);
        UpdateNotePosition();
        GameEventBus.Subscribe<GameOverEvent>(OnGameOver);
    }

    void OnDisable()
    {
        GameEventBus.Unsubscribe<GameOverEvent>(OnGameOver);
    }

    void OnGameOver(GameOverEvent _)
    {
        Debug.Log("Game Over received in Note, disabling note.");
        canBePressed = false;
        keys.Clear();
        enabled = false;
        gameover = true;
    }

    void Update()
    {
        if(gameover)
            return;
        lastDspTime = currentDspTime;
        currentDspTime = AudioSettings.dspTime;

        if(PauseController.Instance.isPaused)
        {
            targetDPSTime += ElapsedDpsTime;
            return;
        }

        if (!canBePressed)
            return;

        foreach (var key in keys)
        {
            if (Input.GetKeyDown(key))
            {
                canBePressed = false;
                exitByHit = true;
                ScoreManager.Instance.HitBeat(GetDifferenceFromTarget(), this);
                gameObject.SetActive(false);
                break;
            }
        }
    }

    void LateUpdate()
    {
        if(gameover)
            return;
        UpdateNotePosition();
    }

    void UpdateNotePosition()
    {
        float yPosition = (float)(targetDPSTime - currentDspTime) * speed;
        transform.localPosition = new Vector3(transform.localPosition.x, yPosition, transform.localPosition.z);
    }

    void SetNoteSprite(int index)
    {
        spriteRenderer.sprite = sprites[index];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Note entered trigger with tag: " + collision.tag);
        if (collision.CompareTag("Activator"))
        {
            canBePressed = true;
            keys = new(collision.GetComponent<InputKey>().keys);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Activator"))
        {
            canBePressed = false;
            keys.Clear();
            if (!exitByHit)
                ScoreManager.Instance.HitBeat(GetDifferenceFromTarget(), this); 
        }
    }

    double GetDifferenceFromTarget()
    {
        //Ejemplo, una diferencia de 0,0613333322107792 son 61ms, se puede ajustar segun sea necesario
        return Mathf.Abs((float)(AudioSettings.dspTime - targetDPSTime) * 1000); //Convertir a milisegundos
    }
}
