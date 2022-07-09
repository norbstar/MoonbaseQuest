using UnityEngine;

namespace Chess
{
    public class SettingsManager : CachedObject<SettingsManager>
    {
        public static class Settings
        {
            public enum Mode
            {
                Remote,
                Direct
            }

            public enum Skill
            {
                Easy,
                Skilled,
                Expert
            }

            public enum Scene
            {
                Lounge
            }
        }

        [SerializeField] Settings.Mode interactionMode;

        [SerializeField] Set set = Set.Light;

        [SerializeField] bool playFirst = true;

        [SerializeField] Settings.Skill skill = Settings.Skill.Easy;

        [SerializeField] Settings.Scene scene;

        public Settings.Mode InteractionMode
        {
            get
            {
                return interactionMode;
            }

            set
            {
                interactionMode = value;
            }
        }

        public Set Set
        {
            get
            {
                return set;
            }

            set
            {
                set = value;
            }
        }

        public bool PlayFirst
        {
            get
            {
                return playFirst;
            }

            set
            {
                playFirst = value;
            }
        }

        public Settings.Skill Skill
        {
            get
            {
                return skill;
            }

            set
            {
                skill = value;
            }
        }

        public Settings.Scene Scene
        {
            get
            {
                return scene;
            }

            set
            {
                scene = value;
            }
        }

        public void OnInteractionModeChange(int index)
        {
            interactionMode = (Settings.Mode) index;

            // switch (index)
            // {
            //     case (int) Settings.Mode.Remote:
            //         interactionMode = Settings.Mode.Remote;
            //         break;

            //     case (int) Settings.Mode.Direct:
            //         interactionMode = Settings.Mode.Direct;
            //         break;
            // }
        }

        public void OnPlayAsChange(int index) => set = (Set) index;

        public void OnPlayOrderFirstChange(int index) => playFirst = (index == 0);

        public void OnOppositionSkillLevelChange(int index) => skill = (Settings.Skill) index;

        public void OnSceneChange(int index)
        {
            scene = (Settings.Scene) index;
            
            // switch (index)
            // {
            //     case (int) Settings.Scene.Lounge:
            //         scene = Settings.Scene.Lounge;
            //         break;
            // }
        }
    }
}