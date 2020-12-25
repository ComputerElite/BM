/* 
    Nice that you are here.
*/

using System.Collections.Generic;
using System.Text.Json;

namespace Qosmetics
{
    public class QosmeticsJSON
    {
        public List<QosmeticsObject> modelerQSabers { get; set; } = new List<QosmeticsObject>();
        public List<QosmeticsObject> publicQSabers { get; set; } = new List<QosmeticsObject>();
        public List<QosmeticsObject> qWalls { get; set; } = new List<QosmeticsObject>();
        public List<QosmeticsObject> qBloqs { get; set; } = new List<QosmeticsObject>();
        public List<QosmeticsObject> AllQSabers { get; set; } = new List<QosmeticsObject>();

        public string ToJSON()
        {
            return JsonSerializer.Serialize(this);
        }

        public void SetMixedSabers()
        {
            List<QosmeticsObject> finished = new List<QosmeticsObject>();

            List<QosmeticsObject> bigger = new List<QosmeticsObject>(publicQSabers);
            List<QosmeticsObject> smaller = new List<QosmeticsObject>(modelerQSabers);
            if(modelerQSabers.Count > publicQSabers.Count)
            {
                bigger = new List<QosmeticsObject>(modelerQSabers);
                smaller = new List<QosmeticsObject>(publicQSabers);
            }
            int i = 0;
            foreach(QosmeticsObject o in smaller)
            {
                finished.Add(bigger[0]);
                finished.Add(o);
                bigger.RemoveAt(0);
                i++;
            }
            foreach(QosmeticsObject o in bigger)
            {
                finished.Add(o);
            }
            AllQSabers = finished;
        }
    }

    public class QosmeticsObject
    {
        public string name { get; set; } = "N/A";
        public string author { get; set; } = "N/A";
        public string orgmessage { get; set; } = "N/A";
        public string imageURL { get; set; } = "N/A";
        public string downloadURL { get; set; } = "N/A";
    }
}