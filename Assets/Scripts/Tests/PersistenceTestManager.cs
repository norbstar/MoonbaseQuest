using System;
using System.IO;

using UnityEngine;

namespace Tests
{
    public class PersistenceTestManager : MonoBehaviour
    {
        public void OnLoad()
        {
            PersistenceTestScriptable scriptable = LoadData("test");
            Apply(scriptable);
        }

        public PersistenceTestScriptable LoadData(string filename)
        {
            string json = Load(filename);
            PersistenceTestScriptable._Data data = Deserialize(json);

            var scriptable = new PersistenceTestScriptable();
            scriptable.Data = data;
            
            // foreach (PersistenceObjectScriptable.ObjectData objectData in data.collection)
            // {
            //     GameObject gameObject = GameObject.Find(objectData.name);

            //     if (gameObject != null)
            //     {
            //         gameObject.transform.localPosition = objectData.localPosition;
            //     }
            // }

            return scriptable;
        }

        public void Apply(PersistenceTestScriptable scriptable)
        {
            foreach (PersistenceObjectScriptable.ObjectData objectData in scriptable.Data.collection)
            {
                GameObject gameObject = GameObject.Find(objectData.name);

                if (gameObject != null)
                {
                    gameObject.transform.localPosition = objectData.localPosition;
                }
            }
        }

        public void OnSave()
        {
            var managers = transform.GetComponentsInChildren<PersistenceObjectManager>() as PersistenceObjectManager[];
            PersistenceTestScriptable scriptable = new PersistenceTestScriptable();

            foreach (PersistenceObjectManager manager in managers)
            {
                scriptable.AddData(manager.GetObjectData());
            }

            string json = Serialize(scriptable.Data);
            Save("test", json, true);
        }

        public string Serialize(PersistenceTestScriptable._Data data) => JsonUtility.ToJson(data);

        public PersistenceTestScriptable._Data Deserialize(string json) => JsonUtility.FromJson<PersistenceTestScriptable._Data>(json);

        public bool Exists(string filename) => File.Exists($"{Application.persistentDataPath}/{filename}.json");

        public void Delete(string filename) => File.Delete($"{Application.persistentDataPath}/{filename}.json");

        public void DeleteIfExists(string filename)
        {
            if (Exists(filename))
            {
                Delete(filename);
            }
        }

        public string Load(string filename)
        {
            string path = $"{Application.persistentDataPath}/{filename}.json";
            byte[] bytes = File.ReadAllBytes(path);
            return System.Text.Encoding.Default.GetString(bytes);
        }
        
        public void Save(string filename, string json, bool deleteExisting = false)
        {
            if (deleteExisting)
            {
                DeleteIfExists(filename);
            }

            var dateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            string path = $"{Application.persistentDataPath}/{filename}-{dateTime}.json";
            File.WriteAllText(path, json);
        }
    }
}