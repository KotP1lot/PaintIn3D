using UnityEngine;
using UnityEngine.UI;

public class Brush : MonoBehaviour
{
    [SerializeField] Palete palete;
    [SerializeField] Scrollbar scroll;
    [SerializeField] Camera cam;
    [Space]
    public Color paintColor;
    public float radius = 1;
    private void Start()
    {
        palete.OnColorChanged += ChangeColor;
        scroll.onValueChanged.AddListener(ChangeRadius);
    }
    void Update()
    {
        if (!Input.GetMouseButton(0)) return;
        Vector3 position = Input.mousePosition;
        Ray ray = cam.ScreenPointToRay(position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            Debug.Log(hit.point);
            //Paintable p = hit.collider.GetComponent<Paintable>();
            //if (p != null)
            //{
            //    PaintManager.instance.paint(p, hit.point, radius, paintColor);
            //}
        }
    }
    public void ChangeColor(Color color) => paintColor = color;
    public void ChangeRadius(float size) => radius = size;
}
