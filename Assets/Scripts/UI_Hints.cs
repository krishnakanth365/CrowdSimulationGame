using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Hints : MonoBehaviour
{
    [SerializeField] TMP_Text[] textHintObjs = new TMP_Text[3];
    [SerializeField] GameObject[] textHintObjsHolder = new GameObject[3];
    public static UI_Hints instance;
    public static int count = 0;

    private void Awake()
    {
        instance = this;
    }
    public void AssignHints()
    {
        if (Theif.instance.bagsd)
        {
            textHintObjs[0].text = "Theif has a BAG";
        }
        else
        {
            textHintObjs[0].text = "Theif DOESNT have a BAG";
        }

        if (Theif.instance.watchsd)
        {
            textHintObjs[1].text = "Theif wears a WATCH";
        }
        else
        {
            textHintObjs[1].text = "Theif DOESNT wear a WATCH";
        }

        if (Theif.instance.hatsd)
        {
            textHintObjs[2].text = "Theif has a HAT";
        }
        else
        {
            textHintObjs[2].text = "Theif DOESNT have a HAT";
        }
    }

    public void RevealHint()
    {
        if (count >= 3)
        {
            return;
        }
        textHintObjsHolder[count].gameObject.SetActive(true);
        count++;
    }
}
