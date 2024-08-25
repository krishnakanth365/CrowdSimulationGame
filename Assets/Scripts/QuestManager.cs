using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{

    public static QuestManager instance;
    public static bool interacted = false;

    private void Awake()
    {
        instance = this;
    }
}
