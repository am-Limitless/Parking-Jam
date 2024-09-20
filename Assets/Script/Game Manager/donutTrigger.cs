using System.Collections;
using UnityEngine;

public class donutTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource m_Source;

    public ParticleSystem collectEffect;

    public bool isCollect = false;

    [SerializeField] private GameObject collectMessage;
    [SerializeField] private GameObject parkMessage;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerCar")
        {
            collectMessage.SetActive(false);
            parkMessage.SetActive(true);
            isCollect = true;
            m_Source.Play();
            collectEffect.Emit(150);
            StartCoroutine(DestroyGameObject());
        }
    }

    IEnumerator DestroyGameObject()
    {
        yield return new WaitForSeconds(0.4f);

        Destroy(gameObject);
    }
}
