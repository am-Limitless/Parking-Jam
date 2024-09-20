using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject controlPannel;

    public void ControlHelp()
    {
        controlPannel.SetActive(true);
    }

    public void OffControlMenu()
    {
        controlPannel.SetActive(false);
    }
}
