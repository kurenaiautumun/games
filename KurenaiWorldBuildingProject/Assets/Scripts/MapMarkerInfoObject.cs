using UnityEngine;

public class MapMarkerInfoObject : MonoBehaviour
{
    public string mapFilePath;
    public Vector2 position;

    // Scale related
    [HideInInspector] public float scaleAnimMult;
    private Vector3 startingScale;
    private Vector3 maxScale;

    private void Start()
    {
        startingScale = transform.localScale;
        maxScale = transform.localScale * 1.5f;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(startingScale, maxScale, scaleAnimMult);
    }
}
