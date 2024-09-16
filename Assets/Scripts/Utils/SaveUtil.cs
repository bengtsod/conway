using Controllers;
using Misc;
using UnityEngine;

namespace Utils
{
    public static class SaveUtil
    {
        public static void Load()
        {
            if (HasSavedState())
            {
                var generation = Generation.FromCode(PlayerPrefs.GetString("Save"));
                GameController.Instance.Load(generation);
            }
        }

        public static bool HasSavedState()
        {
            return PlayerPrefs.HasKey("Save");
        }

        public static void Save()
        {
            PlayerPrefs.SetString("Save", Generation.ToCode(GameController.Instance.GetLastGeneration()));
            PlayerPrefs.Save();
        }
    }
}