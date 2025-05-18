using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    [SerializeField] private bool activeAtStart = false;
    private void Awake()
    {
        Debug.Log("Awake Invoked! Don't destroy on load activated for the gameobject : "+gameObject.name);
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(activeAtStart);
    }
}
