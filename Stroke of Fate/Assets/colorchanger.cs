using UnityEngine;

public class colorchanger : MonoBehaviour
{
    public Color[] colors;
    public float changeTime = 2f;

    void Start()
    {
        InvokeRepeating(nameof(ChangeColor), 0f, changeTime);
    }

    void ChangeColor()
    {
        int randomIndex = Random.Range(0, colors.Length);

        GetComponent<SpriteRenderer>().color = colors[randomIndex];
    }
}

