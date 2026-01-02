using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class UniversalDragDrop2D : MonoBehaviour
{
    [Header("Настройки")]
    public bool returnToStartIfNoDrop = true;  // Вернуть на место, если не сброшен на зону
    public float dropRadius = 0.5f;            // Радиус проверки зоны (OverlapCircle)

    private Vector3 originalPosition;
    private Vector2 dragOffset;
    private bool isDragging = false;
    private Camera mainCamera;
    private Collider2D ownCollider;

    // Статический для одного перетаскивания за раз
    private static UniversalDragDrop2D currentDragged;

    private void Start()
    {
        originalPosition = transform.position;
        mainCamera = Camera.main;
        ownCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        Vector2 pointerScreenPos = GetPointerScreenPosition();

        // Начало перетаскивания
        if (IsPointerDown() && !isDragging)
        {
            Vector2 pointerWorldPos = mainCamera.ScreenToWorldPoint(pointerScreenPos);
            Collider2D hit = Physics2D.OverlapPoint(pointerWorldPos);
            if (hit == ownCollider)
            {
                StartDragging(pointerWorldPos);
            }
        }

        // Перетаскивание
        if (isDragging)
        {
            Vector2 pointerWorldPos = mainCamera.ScreenToWorldPoint(pointerScreenPos);
            transform.position = pointerWorldPos + dragOffset;
        }

        // Конец перетаскивания
        if (IsPointerUp() && isDragging)
        {
            EndDragging(pointerScreenPos);
        }
    }

    private Vector2 GetPointerScreenPosition()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).position;
        return Input.mousePosition;
    }

    private bool IsPointerDown()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).phase == TouchPhase.Began;
        return Input.GetMouseButtonDown(0);
    }

    private bool IsPointerUp()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled;
        return Input.GetMouseButtonUp(0);
    }

    private void StartDragging(Vector2 pointerWorldPos)
    {
        if (currentDragged != null && currentDragged != this)
            return;  // Блокируем множественное перетаскивание

        currentDragged = this;
        isDragging = true;
        dragOffset = new Vector2 (transform.position.x, transform.position.y) - pointerWorldPos;

        // Если есть Rigidbody2D — делаем kinematic
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.isKinematic = true;

        Debug.Log("Начато перетаскивание: " + name);
    }

    private void EndDragging(Vector2 pointerScreenPos)
    {
        isDragging = false;
        currentDragged = null;

        // Восстанавливаем Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.isKinematic = false;

        // Проверяем сброс на зону
        Vector2 pointerWorldPos = mainCamera.ScreenToWorldPoint(pointerScreenPos);
        Collider2D hitZone = Physics2D.OverlapCircle(pointerWorldPos, dropRadius);

        if (hitZone != null && hitZone.CompareTag("DropZone"))
        {
            // Snap к центру зоны
            transform.position = hitZone.transform.position;
            Debug.Log("Успешный сброс на зону: " + hitZone.name);


        }
        else if (returnToStartIfNoDrop)
        {

        }

        // Сброс позиции для следующего раза (если snap)
        originalPosition = transform.position;
    }

    // Публичный метод для сброса позиции извне
    public void ResetPosition()
    {
        transform.position = originalPosition;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "colba"){
            collision.gameObject.GetComponent<AudioSource>().Play();
            Destroy(this.gameObject);
        }
    }
}