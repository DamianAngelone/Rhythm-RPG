using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OsuParser;
using MoveNote;

public class Test : MonoBehaviour {

    AudioSource audio_data;
    SpriteRenderer sprite_render;

    Beatmap beatmap;

    int total_notes;
    int note_count = 0;

    float nextIndex;
    float dsptimesong;
    float secPerBeat;
    float songPosition;
    float songPosInBeats;
    float beatsShownInAdvance;

    public SpriteRenderer Note;
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

        for (int i = beatmap.HitObjects.Count - 1; i >= 0; i--)
        {
            if (typeof(CircleObject).IsAssignableFrom(beatmap.HitObjects[i].GetType())) //CircleObject
            {
                float value = beatmap.HitObjects[i].StartTimeInBeats(secPerBeat);
                frame_list.Push((int)value);
                Debug.Log("Circle Frame " + i + ": " + value);
            }
            else if (typeof(SliderObject).IsAssignableFrom(beatmap.HitObjects[i].GetType())) //SliderObject
            {
                float start_value = ((SliderObject)beatmap.HitObjects[i]).StartTimeInBeats(secPerBeat);
                float end_value = start_value + ((SliderObject)beatmap.HitObjects[i]).EndTimeInBeats(secPerBeat, beatmap.SliderMultiplier);

                Debug.Log("Slider Frame " + i + ": " + start_value + " - " + end_value);

                frame_list.Push((int)end_value);
                frame_list.Push((int)start_value);

                /* This accounts for the end of the slider, which is now it's own note */
                total_notes++;
            }
            else if (typeof(SpinnerObject).IsAssignableFrom(beatmap.HitObjects[i].GetType())) //SpinnerObject
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
    }

    void Update ()
    {
        Debug.Log("Next Note: " + frame_list.Peek());

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
                Instantiate(Note, new Vector3(0.704F, 0.209F, -9.5F), Quaternion.identity);

                Note NoteTest = new Note(0.704F, -0.799F, beatsShownInAdvance, frame_list.Peek(), songPosInBeats);

                //initialize the fields of the music note

                //nextIndex++;
            }

            frame_list.Pop();
            total_notes--;

            if (sprite_render.color == Color.red)
            {
                sprite_render.color = Color.white;
            }
            else
            {
                sprite_render.color = Color.red;
            }

            note_count++;
        }

        if (Input.GetKeyDown("space"))
        {
            Debug.Log("START OF SONG");
            audio_data.Play(0);
            note_count = 0;
        }
    }
}
