using UnityEngine;

public class colorchange: MonoBehaviour
{
    public Color[] colors;

    void Start()
    {
        ChangeColor();
    }

    void ChangeColor()
    {
        int randomIndex = Random.Range(0, colors.Length);

        GetComponent<SpriteRenderer>().color = colors[randomIndex];
    }
}