using System.Collections;
using UnityEngine;

public class donutTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource m_Source;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerCar")
        {
            m_Source.Play();

            StartCoroutine(DestroyGameObject());

        }
    }

    IEnumerator DestroyGameObject()
    {
        yield return new WaitForSeconds(0.8f);

        Destroy(gameObject);
    }
}
