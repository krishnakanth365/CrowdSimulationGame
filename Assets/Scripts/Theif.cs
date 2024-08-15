using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Theif : MonoBehaviour
{
    public GameObject bag;
    public GameObject watch;
    public GameObject hat;

    public static Theif instance;
    public static bool hasHat = false;
    public static bool hasBag = false;
    public static bool hasWatch = false;

    public bool hatsd = false;
    public bool bagsd = false;
    public bool watchsd = false;

    private void Awake()
    {
        instance = this;
    }

    public void AssignReferences(GameObject watch, GameObject hat, GameObject bag)
    {
        this.hat = hat;
        this.watch = watch;
        this.bag = bag;
    }

    private void Start()
    {
        Randomize();
        if (hasHat)
        {
            hat.SetActive(true);
        }
        if (hasWatch)
        {
            watch.SetActive(true);
        }
        if (hasBag)
        {
            bag.SetActive(true);
        }
        UI_Hints.instance.AssignHints();
    }

    public void Randomize()
    {
        GameObject[] objects = { hat, bag, watch };
        bool[] hasObject = { hasHat, hasBag, hasWatch };

        int RandomNums = Random.Range(1, objects.Length);

        for (int i = 0; i < RandomNums; i++)
        {
            int tmp = Random.Range(0, 10);
            if (tmp > 2)
            {
                objects[i].SetActive(true);
                if (i == 0)
                {
                    hasHat = true;
                    hatsd = true;
                }
                if (i == 1)
                {
                    hasBag = true;
                    bagsd = true;
                }
                if (i == 2)
                {
                    hasWatch = true;
                    watchsd = true;
                }
            }
        }
    }
}
