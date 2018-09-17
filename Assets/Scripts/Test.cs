using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OsuParser;
//using MoveNote;

public class Test : MonoBehaviour {

    AudioSource audio_data;
    SpriteRenderer sprite_render;

    Beatmap beatmap;

    public SpriteRenderer note;
    int total_notes;
    int note_count = 0;

    float nextIndex;
    float dsptimesong;
    float secPerBeat;
    float songPosition;
    float songPosInBeats;
    float beatsShownInAdvance;

    Vector3 start;
    Vector3 end;

    //public SpriteRenderer Note;
    Stack<int> frame_list;

    void end_debug_mode ()
    {
        Debug.Log("ENDING PROGRAM");
        UnityEditor.EditorApplication.isPlaying = false;
    }

    void Start ()
    {
        sprite_render = GetComponent<SpriteRenderer>();

        beatmap = new Beatmap("C:/Users/Damian/Documents/GitHub/Rhythm-RPG/Capstone/Assets/Songs/trigger_happy-normal.osu");
        audio_data = GetComponent<AudioSource>();

        frame_list = new Stack<int>();
        secPerBeat = beatmap.TimingPoints[0].TimePerBeat;
        total_notes = beatmap.HitObjects.Count;
        beatsShownInAdvance = 5;
        nextIndex = 0;

        start = new Vector3(2.0F, 0.8F, 0.0F);// 3.709F);
        end = start;
        end.x = -2.0F;

        for (int i = beatmap.HitObjects.Count - 1; i >= 0; i--)
        {
            if (beatmap.HitObjects[i].GetHitObjectType() == HitObjectType.Circle) //CircleObject
            {
                float value = beatmap.HitObjects[i].StartTimeInBeats(secPerBeat);
                frame_list.Push((int)value);
                Debug.Log("Circle Frame " + i + ": " + value);
            }
            else if (beatmap.HitObjects[i].GetHitObjectType() == HitObjectType.Slider) //SliderObject
            {
                float start_value = ((SliderObject)beatmap.HitObjects[i]).StartTimeInBeats(secPerBeat);
                float end_value = start_value + ((SliderObject)beatmap.HitObjects[i]).EndTimeInBeats(secPerBeat, beatmap.SliderMultiplier);

                Debug.Log("Slider Frame " + i + ": " + start_value + " - " + end_value);

                frame_list.Push((int)end_value);
                frame_list.Push((int)start_value);

                /* This accounts for the end of the slider, which is now it's own note */
                total_notes++;
            }
            else if (beatmap.HitObjects[i].GetHitObjectType() == HitObjectType.Spinner) //SpinnerObject
            {
                float start_value = ((SpinnerObject)beatmap.HitObjects[i]).StartTimeInBeats(secPerBeat);
                float end_value = ((SpinnerObject)beatmap.HitObjects[i]).EndTimeInBeats(secPerBeat);

                Debug.Log("Spinner Frame " + i + ": " + start_value + " - " + end_value);

                frame_list.Push((int)end_value);
                frame_list.Push((int)start_value);

                /* TEMP -- This accounts for the end of the spinner, which is now it's own note */
                total_notes++;
            }
            else //Error
            {
                Debug.Log("INVALID HIT OBJECT");
                end_debug_mode();
            }
        }

        dsptimesong = (float)AudioSettings.dspTime;

        Debug.Log("START OF SONG");
        audio_data.Play(0);

        //for (int i = 0; i < total_notes; i++)
        //{
        //    Debug.Log(frame_list.Peek());
        //    frame_list.Pop();
        //}

        //end_debug_mode();
    }

    void Update ()
    {
        //calculate the position in seconds
        songPosition = (float)(AudioSettings.dspTime - dsptimesong);

        //calculate the position in beats
        songPosInBeats = (1000 * songPosition) / secPerBeat;

        if (System.Math.Floor(songPosInBeats) > frame_list.Peek())
        {
            Debug.Log("BROKEN! note count: " + songPosInBeats + ", stack_value: " + frame_list.Peek());
            end_debug_mode();
        }

        if (total_notes == 0)
        {
            Debug.Log("END OF SONG");
        }

        if (System.Math.Floor(songPosInBeats) >= frame_list.Peek())
        {
            Debug.Log("FOUND NOTE: " + frame_list.Peek());

            if ((total_notes != 0) && (frame_list.Peek() < (songPosInBeats + beatsShownInAdvance)))
            {
                SpriteRenderer note_sprite = Instantiate(note, start, Quaternion.identity);

                note_sprite.GetComponent<Movement>().SpawnPos = start;
                note_sprite.GetComponent<Movement>().RemovePos = end;

                note_sprite.GetComponent<Movement>().BeatsShownInAdvance = beatsShownInAdvance;
                note_sprite.GetComponent<Movement>().BeatOfThisNote = frame_list.Peek();
                note_sprite.GetComponent<Movement>().SongPosInBeats = songPosInBeats;

                //nextIndex++;
            }

            frame_list.Pop();
            total_notes--;

            //if (sprite_render.color == Color.red)
            //{
            //    sprite_render.color = Color.white;
            //}
            //else
            //{
            //    sprite_render.color = Color.red;
            //}

            note_count++;
        }
    }
}
