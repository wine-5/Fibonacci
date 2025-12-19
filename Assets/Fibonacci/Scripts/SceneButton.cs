using UnityEngine;
using Fibonacci.Scene;

public class SceneButton : MonoBehaviour
{
    public string sceneName;

    public void OnClick()
    {
        SceneController.Instance.LoadScene("InGame");
        Debug.Log("押された");
    }
}
