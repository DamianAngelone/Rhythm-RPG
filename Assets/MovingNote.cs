using System;

namespace MoveNote
{
    public class Note
    {
        protected float SpawnPos { get; set; }
        protected float RemovePos { get; set; }
        protected float BeatsShownInAdvance { get; set; }
        protected float beatOfThisNote { get; set; }
        protected float songPosInBeats { get; set; }

        public Note(float spawn_pos, float remove_pos, float beat_shown_in_advance, float beat_of_this_note, float song_pos_in_beats)
        {
            SpawnPos = spawn_pos;
            RemovePos = remove_pos;
            BeatsShownInAdvance = beat_shown_in_advance;
            beatOfThisNote = beat_of_this_note;
            songPosInBeats = song_pos_in_beats;
        }
    }
}
