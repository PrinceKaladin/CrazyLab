using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class level2 : MonoBehaviour
{
    AudioSource c;
    public AudioClip x;

    void Start()
    {
        c = GetComponent<AudioSource>();
    }

    void Update()
    {
        CheckLevelComplete();
        if (!InputDown()) return;

        Vector2 pos = Camera.main.ScreenToWorldPoint(GetInputPosition());
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

        if (hit.collider == null) return;

        if (hit.collider.CompareTag("Finish"))
        {
            if (x != null) c.PlayOneShot(x);
            Destroy(hit.collider.gameObject);

            CheckLevelComplete();
        }
       
    }

    bool InputDown()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).phase == TouchPhase.Began;

        return Input.GetMouseButtonDown(0);
    }

    Vector2 GetInputPosition()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).position;

        return Input.mousePosition;
    }

    void CheckLevelComplete()
    {
        if (GameObject.Find("pop") == null)
        {
            SceneManager.LoadScene(4);
        }

        
    }
}
