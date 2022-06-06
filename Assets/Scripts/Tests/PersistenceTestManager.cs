using System;
using System.IO;

using UnityEngine;

namespace Tests
{
    public class PersistenceTestManager : MonoBehaviour
    {
        void Awake()
        {
            string path = ConfigureNamedPath("test");
            ConfigurePath(path);
        }

#region Load
        private PersistenceTestScriptable._Data Deserialize(string json) => JsonUtility.FromJson<PersistenceTestScriptable._Data>(json);

        public PersistenceTestScriptable LoadData(string path)
        {
            string json = Load(path);
            PersistenceTestScriptable._Data data = Deserialize(json);

            var scriptable = new PersistenceTestScriptable();
            scriptable.SetData(data);
            
            return scriptable;
        }

        private void ApplyScriptable(PersistenceTestScriptable scriptable)
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

        private string path;

        public void ConfigurePath(string path) => this.path = path;

        public void OnLoad()
        {
            PersistenceTestScriptable scriptable = LoadData(path);
            ApplyScriptable(scriptable);
        }

        private string Load(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            return System.Text.Encoding.Default.GetString(bytes);
        }
#endregion

#region Save
        private string Serialize(PersistenceTestScriptable._Data data) => JsonUtility.ToJson(data);

        public string SaveData(string filename)
        {
            var managers = transform.GetComponentsInChildren<PersistenceObjectManager>() as PersistenceObjectManager[];
            PersistenceTestScriptable scriptable = new PersistenceTestScriptable();

            foreach (PersistenceObjectManager manager in managers)
            {
                scriptable.AddData(manager.GetData());
            }

            string json = Serialize(scriptable.Data);
            return Save(filename, json, true);
        }

        public void OnSave() => SaveData("test");

        private bool Exists(string path) => File.Exists(path);
        
        private void Delete(string path) => File.Delete(path);

        private void DeleteIfExists(string filename)
        {
            if (Exists(filename))
            {
                Delete(filename);
            }
        }

        private string ConfigureNamedPath(string filename) => $"{Application.persistentDataPath}/{filename}.json";

        private string ConstructPath(string filename)
        {
            var dateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return $"{Application.persistentDataPath}/{filename}-{dateTime}.json";
        }

        private string Save(string filename, string json, bool deleteExisting = false)
        {
            if (deleteExisting)
            {
                DeleteIfExists(filename);
            }

            string path = ConstructPath(filename);
            Debug.Log($"Saving to {path}");
            File.WriteAllText(path, json);

            return path;
        }
#endregion
    }
}