using UnityEngine;

public class Reticle : MonoBehaviour
{
    [SerializeField] float maxDistance;

    [Header("References")]
    Transform player;
    Camera cam;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>().transform;

        cam = FindFirstObjectByType<Camera>();
    }
    private void Update()
    {
        // Prevents reticle movement if game is over or paused
        if (GameManager.instance.isGameOver || GameManager.instance.isPaused)
        {
            return;
        }

        //Getting position of mouse
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        //Ensuring that the mouse can only go so far away from the player
        Vector3 mouseDir = mousePos - player.position;

        if (mouseDir.magnitude > maxDistance)
        {
            mouseDir = Vector3.ClampMagnitude(mouseDir, maxDistance);
        }

        transform.position = player.position + mouseDir;
    }
}
