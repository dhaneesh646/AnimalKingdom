using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OffScreenIndicator : MonoBehaviour
{
    public Image LocationIndicatorPrefab;
    public Vector3 offset;
    public Transform Player;
    public Canvas OffscreenCanvas;

    private Transform target;
    private TMP_Text distancetext;
    private Image Indicator;

    public void Start()
    {
        target = this.gameObject.transform;
        Indicator = Instantiate(LocationIndicatorPrefab, transform.position, transform.rotation);
        Indicator.transform.SetParent(OffscreenCanvas.transform);
        distancetext = Indicator.GetComponentInChildren<TMP_Text>();
    }
    public void Update()
    {
        float minimumX = Indicator.GetPixelAdjustedRect().width / 2;
        float maximumX = Screen.width - minimumX;

        float minimumy = Indicator.GetPixelAdjustedRect().height / 2;
        float maximumy = Screen.width - minimumy;

        Vector2 pos = Camera.main.WorldToScreenPoint(target.position + offset);

        if (Vector3.Dot((target.position - Player.position), Player.forward) < 0)
        {
            if (pos.x < Screen.width / 2)
            {
                pos.x = maximumX;
            }
            else
            {
                pos.x = minimumX;
            }
        }
        pos.x = Mathf.Clamp(pos.x, minimumX, maximumX);
        pos.y = Mathf.Clamp(pos.y, minimumy, maximumy);

        Indicator.transform.position = pos;
        distancetext.text = ((int)Vector3.Distance(target.position, Player.position)).ToString() + "m";
    }

}
