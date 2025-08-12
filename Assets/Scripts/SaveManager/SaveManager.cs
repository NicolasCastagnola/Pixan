using UnityEngine;
using System.IO;
using System;
using System.Linq;
using Object = UnityEngine.Object;

namespace SaveHandler
{
    [Serializable]
    public class OptionsObject //Structure diferents options
    {
        
        public float volume;    
        public float brightness;
        public float saturation;

        public OptionsObject()
        {
            volume = 0.4f;
            brightness = 0.5f;
            saturation = 0.5f;
        }

        public OptionsObject Initialize(float _volume, float _brightness, float _saturation)
        {                                                     
            volume = Mathf.Clamp01(_volume);           
            brightness = Mathf.Clamp01(_brightness);
            saturation = Mathf.Clamp01(_saturation);

            return this;
        }
    }

    public class SaveManager<T>
    {
        public readonly string path;

        /// <param name="JsonPath">Json Name on defaultPath. Example "Options"</param>
        public SaveManager(string JsonPath = "Default")
        {
            path = Application.persistentDataPath +"/"+ JsonPath+".json";
        }

        /// <summary>
        /// Load Json from path.
        /// </summary>
        /// /// /// <param name="OnSave">Action that is used to Save the data.</param>
        public void Load(Action<T> OnSave)
        {
            try
            {
                File.ReadAllText(path);
            }
            catch
            {
                New();
            }

            OnSave.Invoke(JsonUtility.FromJson<T>(File.ReadAllText(path)));
        }

        /// <summary>
        /// Saves information on path.
        /// </summary>
        /// /// <param name="OnLoad">Func that is used to get the data.</param>
        public void Save(Func<T> OnLoad)
        {
            File.WriteAllText(path, JsonUtility.ToJson(OnLoad.Invoke(), true));
        }

        /// <summary>
        /// Creates Json on Path.
        /// </summary>
        public void New()
        {
            string json = JsonUtility.ToJson((T)Activator.CreateInstance(typeof(T)));
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Deletes Json on Path.
        /// </summary>
        public void Delete()
        {
            File.Delete(path);
        }
    }
}