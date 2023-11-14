using UnityEngine;

public class SelectMenu : MonoBehaviour
{
    public Canvas[] canvas;

    public void ChangeMenu(int num)
    {
        for(int i = 0; i < canvas.Length; i++)
        {
            if (canvas[i].sortingOrder>0)
                canvas[i].sortingOrder -= 1;
        }

        if (canvas[num].sortingOrder!=1)
        {
            canvas[num].sortingOrder += 1;
        }

        Debug.Log("menu" + num);
    }
}
