using UnityEngine;

namespace Chess
{
    [CreateAssetMenu(fileName = "Data", menuName = "Holo Chess/Player Config", order = 2)]
    public class PlayerConfigScriptable : ScriptableObject
    {
        [SerializeField] PlayerMode lightPlayer;
        public PlayerMode LightPlayer { get { return lightPlayer; } }
        [SerializeField] PlayerMode darkPlayer;
        public PlayerMode DarkPlayer { get { return darkPlayer; } }
    }
}