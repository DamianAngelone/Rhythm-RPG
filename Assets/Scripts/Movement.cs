using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //Note note;
    public Vector3 SpawnPos;
    public Vector3 RemovePos;
    public float BeatsShownInAdvance;
    public float BeatOfThisNote;
    public float SongPosInBeats;

    float lerpTime = 1f;
    float currentLerpTime;

    // Use this for initialization
    void Start ()
    {
        // N/A
    }

    // Update is called once per frame
    void Update()
    {
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > lerpTime)
        {
            currentLerpTime = lerpTime;
        }

        //lerp!
        float perc = currentLerpTime / lerpTime;

        transform.position = Vector2.Lerp(SpawnPos, RemovePos, perc);// (BeatsShownInAdvance - (BeatOfThisNote - SongPosInBeats)) / BeatsShownInAdvance);

        if (transform.position == RemovePos)
        {
            Debug.Log("DELETE NOTE");
            Destroy(transform.root.gameObject);
        }
    }
}
