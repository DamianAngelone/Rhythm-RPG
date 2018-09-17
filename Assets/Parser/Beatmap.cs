using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OsuParser
{
    public class Beatmap
    {
        //General 
        public int? AudioLeadIn = null;

        //Metadata
        public string Title;
        public string Artist;
        public string Creator;

        //Difficulty
        public float OverallDifficulty = 5;
        public float ApproachRate = 5;
        public float SliderMultiplier = 1.4f;
        public float SliderTickRate = 1;

        //TimingPoints
        public List<TimingPoint> TimingPoints = new List<TimingPoint>();

        //HitObjects
        public List<HitObject> HitObjects = new List<HitObject>();

        public Beatmap(String beatmapFile = "")
        {
            if (beatmapFile != "")
            {
                if (File.Exists(beatmapFile))
                {
                    Parse(beatmapFile);
                }
            }
        }

        private void Parse(string beatmapFile)
        {
            StreamReader sr = new StreamReader(beatmapFile);
            string line,currentSection="";

            while ((line = sr.ReadLine()) != null)
            {
                //Skip commented or blank lines
                if (line.StartsWith("//") || line.Length == 0)
                    continue;
                //Get section tag if line matches "[Section Name"]
                if (line.StartsWith("["))
                {
                    currentSection = line;
                    continue;
                }

                //[General], [Metadata], [Difficulty] sections
                if ((currentSection == "[General]") || (currentSection == "[Metadata]") || (currentSection == "[Difficulty]") || (currentSection == "[Editor]"))
                {
                    //Split line into values
                    string[] lineSplit = line.Split(':');
                    string property = lineSplit[0].TrimEnd();
                    string value = lineSplit[1].Trim();

                    //Assign values to fields
                    FieldInfo fi = this.GetType().GetField(property);
                    if (fi != null)
                    {
                        if (fi.FieldType == typeof(float?))
                            fi.SetValue(this, (float?)Convert.ToDouble(value));
                        if (fi.FieldType == typeof(float))
                            fi.SetValue(this, (float)Convert.ToDouble(value));
                        else if ((fi.FieldType == typeof(int?)) || (fi.FieldType == typeof(int)))
                            fi.SetValue(this, Convert.ToInt32(value));
                        else if (fi.FieldType == typeof(string))
                            fi.SetValue(this, value);
                    }
                //[TimingPoints] section
                }else if (currentSection == "[TimingPoints]")
                {
                    //Split line into values
                    string[] values = line.Split(',');
                    //Create new timing point (time, time per beat, time signature, frenzy mode)
                    this.TimingPoints.Add(new TimingPoint((float)Convert.ToDouble(values[0]),(float)Convert.ToDouble(values[1]),Convert.ToInt32(values[2]),Convert.ToBoolean(Convert.ToDouble(values[7]))));
                //[HitObjects] section
                } else if (currentSection == "[HitObjects]")
                {
                    //Split line into values
                    string[] values = line.Split(',');
                    //Get hit object type
                    HitObjectType type = (HitObjectType)Convert.ToInt32(values[3]);
                    //[Circle] 
                    if ((type & HitObjectType.Circle) > 0)
                    {
                        //Add new circle hit object
                        HitObjects.Add(new CircleObject(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), (float)Convert.ToDouble(values[2])));
                    }
                    //[Slider]
                    else if((type & HitObjectType.Slider) > 0)
                    {
                        //Add new slider hit object
                        HitObjects.Add(new SliderObject(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), (float)Convert.ToDouble(values[2]), (float)Convert.ToDouble(Convert.ToInt32(values[7]))));
                    }
                    //[Spinner]
                    else if((type & HitObjectType.Spinner) > 0)
                    {
                        //Add new spinner hit object
                        HitObjects.Add(new SpinnerObject(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), (float)Convert.ToDouble(values[2]), (float)Convert.ToDouble(values[5])));
                    }
                }
            }
        }
    }
}
