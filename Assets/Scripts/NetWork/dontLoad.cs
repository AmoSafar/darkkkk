using UnityEngine;

public class dontLoad : MonoBehaviour
{
 private void Awake()
{
    DontDestroyOnLoad(gameObject);
}

  }

