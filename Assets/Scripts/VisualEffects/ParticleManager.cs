using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private GameObject hitParticle;
    [SerializeField] private float customScale = 1f;

    public void OnEnable()
    {
        GameEventBus.Subscribe<HitEvent>(OnHitEvent);
    }

    public void OnDisable()
    {
        GameEventBus.Unsubscribe<HitEvent>(OnHitEvent);
    }

    #if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            // Simula un evento de golpe perfecto para pruebas
            Note dummyNote = new GameObject("DummyNote").AddComponent<Note>();
            dummyNote.transform.position = Vector3.zero; // Posición de prueba
            OnHitEvent(new HitEvent(HitType.Perfect, dummyNote));
            Destroy(dummyNote.gameObject); // Limpia el objeto de prueba
        }
    }
    #endif

    public void OnHitEvent(HitEvent hitEvent)
    {
        GameSettings settings = GameSettings.Instance;

        if(!settings.UsingGameFeel)
            return;

        if(hitEvent.hitType == HitType.Perfect)
        {
            if(hitParticle != null)
            {
                GameObject particle = Instantiate(hitParticle, hitEvent.note.transform.position + settings.ParticleOffset, hitEvent.note.transform.rotation);
                ParticleSystem p = particle.transform.GetChild(0).GetComponent<ParticleSystem>();
                p.Play();
                //Al detenerse la particula, se destruye el objeto padre para evitar acumulacion de objetos en la escena
                Destroy(particle, 1.5f);
                particle.transform.localScale *= customScale;
            }
        }
    }

}