using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform target;

    [SerializeField]
    private Transform topLeftAnchor;
    [SerializeField]
    private Transform bottomRightAnchor;
    private Camera cam;

    //float scaleFactor;
    float lerpSpeed;

    float verticalBound;
    float horizontalBound;

    [SerializeField]
    private bool prewarm = false;

	// Use this for initialization
	void Start ()
    {
        cam = GetComponent<Camera>();
        //scaleFactor = 1f;
        lerpSpeed = 1f;

        verticalBound = cam.orthographicSize;
        horizontalBound = verticalBound * Screen.width / Screen.height;

        if(prewarm)
        {
            CameraLerp(1);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        //cam.orthographicSize = (Screen.height / 100f) / scaleFactor;
        CameraLerp(lerpSpeed);
    }

    void CameraLerp(float lerpAmount)
    {
        if (target)
        {
            Vector3 camPos = target.position;
            camPos.x = Mathf.Clamp(camPos.x, topLeftAnchor.position.x + horizontalBound, bottomRightAnchor.position.x - horizontalBound);
            camPos.y = Mathf.Clamp(camPos.y, bottomRightAnchor.position.y + verticalBound, topLeftAnchor.position.y - verticalBound);
            transform.position = Vector3.Lerp(transform.position, camPos, lerpAmount) + new Vector3(0, 0, -10);
        }
    }
}
