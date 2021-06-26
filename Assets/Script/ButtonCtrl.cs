using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonCtrl : MonoBehaviour
{
    public GameObject buttonObjectPool;
    ButtonObjectPool objectPool;

    private void Start()
    {
        objectPool = buttonObjectPool.GetComponent<ButtonObjectPool>();
    }

    public void OnClicked()
    {
        switch (gameObject.name)
        {
            case "Setting":
                objectPool.buttonObjPool[0].SetActive(true);
                break;

            case "Close":
                objectPool.buttonObjPool[0].SetActive(false);
                break;

            case "Back":
                SceneManager.LoadScene(0);
                break;
        }
    }
}
