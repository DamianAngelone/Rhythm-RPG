using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OsuParser;
using System;

public class metronome : MonoBehaviour
{
    public float tickLength;
    public float nextTickTime;
    public float positionInBeats = 0f;

    public event EventHandler<TickEventArgs> Tick;
    public class TickEventArgs : EventArgs
    {
        public float positionInBeats { get; set; }
    }
    public delegate void EventHandler(metronome m, EventArgs e);

    AudioSource audio_data;
    SpriteRenderer sprite_render;

    Beatmap beatmap;

    public SpriteRenderer note;
    public SpriteRenderer beat;
    public SpriteRenderer slider_bar;

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
    bool in_slider;

    //public SpriteRenderer Note;
    Stack<beat_data> frame_list;

    struct beat_data
    {
        public float beat;
        public HitObjectType type;
    }

    float fixed_beat(float beat)
    {
        float prefix = (float)Math.Truncate(beat);
        var x = beat - Math.Truncate(beat);

        if (x >= 0.0 && x < 0.125)
        {
            return (prefix + 0.00F);
        }
        else if (x >= 0.125 && x < 0.375)
        {
            return (prefix + 0.25F);
        }
        else if (x >= 0.375 && x < 0.625)
        {
            return (prefix + 0.50F);
        }
        else if (x >= 0.625 && x < 0.875)
        {
            return (prefix + 0.75F);
        }
        else
        {
            return beat;
        }
    }

    void spawn(object sender, TickEventArgs e)
    {
        if (GetComponent<SpriteRenderer>().color == Color.black)
        {
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.black;
        }

        songPosInBeats = e.positionInBeats;
        if (System.Math.Floor(songPosInBeats) > frame_list.Peek().beat)
        {
            Debug.Log("BROKEN! note count: " + songPosInBeats + ", stack_value: " + frame_list.Peek().beat);
            //end_debug_mode();
        }

        if (total_notes == 0)
        {
            Debug.Log("END OF SONG");
        }

        if (songPosInBeats == frame_list.Peek().beat - beatmap.ApproachRate)
        {
            Debug.Log("FOUND NOTE: " + frame_list.Peek().beat);

            if ((total_notes != 0) && (frame_list.Peek().beat < (songPosInBeats + beatsShownInAdvance)))
            {
                SpriteRenderer note_sprite = Instantiate(note, start, Quaternion.identity);

                note_sprite.GetComponent<Movement>().SpawnPos = start;
                note_sprite.GetComponent<Movement>().RemovePos = end;

                note_sprite.GetComponent<Movement>().BeatsShownInAdvance = beatsShownInAdvance;
                note_sprite.GetComponent<Movement>().BeatOfThisNote = frame_list.Peek().beat;
                note_sprite.GetComponent<Movement>().SongPosInBeats = songPosInBeats;

                if (frame_list.Peek().type == HitObjectType.Circle)
                {
                    note_sprite.GetComponent<SpriteRenderer>().color = Color.blue;
                }
                else if (frame_list.Peek().type == HitObjectType.Slider)
                {
                    note_sprite.GetComponent<SpriteRenderer>().color = Color.yellow;

                    if (in_slider == true)
                    {
                        in_slider = false;
                    }
                    else
                    {
                        in_slider = true;
                    }
                }
                else
                {
                    note_sprite.GetComponent<SpriteRenderer>().color = Color.green;
                }

                //nextIndex++;
            }

            frame_list.Pop();
            total_notes--;

            note_count++;
        }
        else
        {
            SpriteRenderer beat_sprite;

            if (in_slider)
            {
                beat_sprite = Instantiate(slider_bar, start, Quaternion.identity);
                beat_sprite.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
            else
            {
                beat_sprite = Instantiate(beat, start, Quaternion.identity);
            }

            beat_sprite.GetComponent<Movement>().SpawnPos = start;
            beat_sprite.GetComponent<Movement>().RemovePos = end;

            beat_sprite.GetComponent<Movement>().BeatsShownInAdvance = beatsShownInAdvance;
            beat_sprite.GetComponent<Movement>().BeatOfThisNote = frame_list.Peek().beat;
            beat_sprite.GetComponent<Movement>().SongPosInBeats = songPosInBeats;

        }
    }

    void end_debug_mode()
    {
        Debug.Log("ENDING PROGRAM");
        UnityEditor.EditorApplication.isPlaying = false;
    }

    void Start()
    {
        sprite_render = GetComponent<SpriteRenderer>();

        beatmap = new Beatmap("C:/Users/Damian/Documents/GitHub/Rhythm-RPG/Capstone/Assets/Songs/trigger_happy-normal.osu");
        audio_data = GetComponent<AudioSource>();

        frame_list = new Stack<beat_data>();
        secPerBeat = beatmap.TimingPoints[0].TimePerBeat;
        total_notes = beatmap.HitObjects.Count;
        beatsShownInAdvance = 5;
        nextIndex = 0;
        in_slider = false;

        tickLength = (secPerBeat / 1000) / 2; //tick length is time for one eigth note
        nextTickTime = (float)(AudioSettings.dspTime + tickLength);

        Tick += spawn;

        start = new Vector3(2.0F, 0.8F, 0.0F);// 3.709F);
        end = start;
        end.x = -2.0F;

        beat_data beatData = new beat_data();

        for (int i = beatmap.HitObjects.Count - 1; i >= 0; i--)
        {
            if (beatmap.HitObjects[i].GetHitObjectType() == HitObjectType.Circle) //CircleObject
            {
                float value = beatmap.HitObjects[i].StartTimeInBeats(secPerBeat);

                beatData.beat = fixed_beat(value);
                beatData.type = HitObjectType.Circle;

                frame_list.Push(beatData);
                Debug.Log("Circle Frame " + i + ": " + fixed_beat(value).ToString("F2"));
            }
            else if (beatmap.HitObjects[i].GetHitObjectType() == HitObjectType.Slider) //SliderObject
            {
                float start_value = ((SliderObject)beatmap.HitObjects[i]).StartTimeInBeats(secPerBeat);
                float end_value = start_value + ((SliderObject)beatmap.HitObjects[i]).EndTimeInBeats(secPerBeat, beatmap.SliderMultiplier);

                Debug.Log("Slider Frame " + i + ": " + fixed_beat(start_value).ToString("F2") + " - " + fixed_beat(end_value).ToString("F2"));

                beatData.beat = fixed_beat(end_value);
                beatData.type = HitObjectType.Slider;
                frame_list.Push(beatData);

                beatData.beat = fixed_beat(start_value);
                frame_list.Push(beatData);

                /* This accounts for the end of the slider, which is now it's own note */
                total_notes++;
            }
            else if (beatmap.HitObjects[i].GetHitObjectType() == HitObjectType.Spinner) //SpinnerObject
            {
                float start_value = ((SpinnerObject)beatmap.HitObjects[i]).StartTimeInBeats(secPerBeat);
                float end_value = ((SpinnerObject)beatmap.HitObjects[i]).EndTimeInBeats(secPerBeat);

                Debug.Log("Spinner Frame " + i + ": " + fixed_beat(start_value).ToString("F2") + " - " + fixed_beat(end_value).ToString("F2"));

                beatData.beat = fixed_beat(end_value);
                beatData.type = HitObjectType.Spinner;
                frame_list.Push(beatData);

                beatData.beat = fixed_beat(start_value);
                frame_list.Push(beatData);

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

    void Update()
    {
        float currentTime = (float)AudioSettings.dspTime;
        currentTime += Time.deltaTime;

        if (positionInBeats == 0)
        {
            Debug.Log("Initial tick");
            TickEventArgs args = new TickEventArgs();
            args.positionInBeats = positionInBeats;
            Tick(this, args);
        }

        while (currentTime > nextTickTime)
        {
            positionInBeats += (float)1 / 2; //tick is on eigth notes 
            TickEventArgs args = new TickEventArgs();
            args.positionInBeats = positionInBeats;
            Tick(this, args);
            nextTickTime += tickLength;
        }

        //calculate the position in seconds
        //songPosition = (float)(AudioSettings.dspTime - dsptimesong);

        //calculate the position in beats
        //songPosInBeats = (1000 * songPosition) / secPerBeat;


    }
}
