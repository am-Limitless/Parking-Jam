using System.Collections;
using UnityEngine;

public class donutTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource m_Source;

    public ParticleSystem collectEffect;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerCar")
        {
            m_Source.Play();
            collectEffect.Emit(150);
            StartCoroutine(DestroyGameObject());

        }
    }

    IEnumerator DestroyGameObject()
    {
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }
}
