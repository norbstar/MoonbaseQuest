#if UNITY_EDITOR
using UnityEngine;

using Chess.Scriptable;

namespace Chess
{
    public class GameSessionManagerOld : MonoBehaviour
    {
        [SerializeField] GameSessionScriptable scriptable;

        void Awake()
        {
            scriptable = new GameSessionScriptable();
        }

        public void DeleteAssetIfExists(string filename)
        {
            if (AssetExists("session"))
            {
                DeleteAsset("session");
            }
        }

        public bool AssetExists(string filename) => scriptable.AssetExists(filename);

        public object LoadAsset(string filename, System.Type type) => scriptable.LoadAsset(filename, type);
        
        public void SaveAsset(string filename, bool deleteExisting = false) => scriptable.SaveAsset(filename, deleteExisting);

        public void DeleteAsset(string filename) => scriptable.DeleteAsset(filename);

        // public void Clear() => scriptable.Clear();

        public void SubmitSnapshot(GameSessionScriptable.BoardLayout map) => scriptable.SetLayout(map);

        // public void SubmitMove(GameSessionScriptable.Move move) => scriptable.AddMove(move);
    }
}
#endif