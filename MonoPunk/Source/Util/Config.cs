using System;
using System.Collections.Generic;
using System.IO;

namespace MonoPunk
{
    public interface ConfigProperty
    {
        string Key { get; }

        void Set(string value);
        void Set(bool value);
        void Set(int value);
        void Set(float value);

        string GetString();
        bool GetBool();
        int GetInt();
        float GetFloat();
    }

    public static class Config
    {
        private static string fileName;
        private static readonly Dictionary<string, Property> properties = new Dictionary<string, Property>();

        private class Property : ConfigProperty
        {
            public string Key { get; }
            public object Value { get; private set; }
            public Action<string> StringCallback { get; set; }
            public Action<bool> BoolCallback { get; set; }
            public Action<int> IntCallback { get; set; }
            public Action<float> FloatCallback { get; set; }

            public Property(string key, object value)
            {
                Key = key;
                Value = value;
            }

            public void SetValueFromString(string str)
            {
                if (Value.GetType() == typeof(string))
                {
                    Value = str;
                }
                else if (Value.GetType() == typeof(bool))
                {
                    Value = bool.Parse(str);
                }
                else if (Value.GetType() == typeof(int))
                {
                    Value = int.Parse(str);
                }
                else if (Value.GetType() == typeof(float))
                {
                    Value = float.Parse(str);
                }
            }

            public void Init()
            {
                if (Value.GetType() == typeof(string))
                {
                    StringCallback?.Invoke(GetString());
                }
                else if (Value.GetType() == typeof(bool))
                {
                    BoolCallback?.Invoke((bool)Value);
                }
                else if (Value.GetType() == typeof(int))
                {
                    IntCallback?.Invoke((int)Value);
                }
                else if (Value.GetType() == typeof(float))
                {
                    FloatCallback?.Invoke((float)Value);
                }
            }

            private void SetValue(object value)
            {
                if (Value.GetType() != value.GetType())
                {
                    throw new Exception("Property '" + Key + "' type mismatch.");
                }
                Value = value;
            }

            public void Set(string value)
            {
                SetValue(value);
                StringCallback?.Invoke(value);
                Save();
            }

            public void Set(bool value)
            {
                SetValue(value);
                BoolCallback?.Invoke(value);
                Save();
            }

            public void Set(int value)
            {
                SetValue(value);
                IntCallback?.Invoke(value);
                Save();
            }

            public void Set(float value)
            {
                SetValue(value);
                FloatCallback?.Invoke(value);
                Save();
            }

            public string GetString()
            {
                return (string)Value;
            }

            public bool GetBool()
            {
                return (bool)Value;
            }

            public int GetInt()
            {
                return (int)Value;
            }

            public float GetFloat()
            {
                return (float)Value;
            }
        }

        private static Property CreatePropertyBase(string key, object defaultValue)
        {
            if (properties.ContainsKey(key))
            {
                throw new Exception("Property '" + key + "' already exists.");
            }
            var property = new Property(key, defaultValue);
            properties.Add(key, property);
            return property;
        }

        public static ConfigProperty CreateProperty(string key, string defaultValue, Action<string> callback = null)
        {
            var property = CreatePropertyBase(key, defaultValue);
            property.StringCallback = callback;
            return property;
        }

        public static ConfigProperty CreateProperty(string key, bool defaultValue, Action<bool> callback = null)
        {
            var property = CreatePropertyBase(key, defaultValue);
            property.BoolCallback = callback;
            return property;
        }

        public static ConfigProperty CreateProperty(string key, int defaultValue, Action<int> callback = null)
        {
            var property = CreatePropertyBase(key, defaultValue);
            property.IntCallback = callback;
            return property;
        }

        public static ConfigProperty CreateProperty(string key, float defaultValue, Action<float> callback = null)
        {
            var property = CreatePropertyBase(key, defaultValue);
            property.FloatCallback = callback;
            return property;
        }

        public static ConfigProperty GetProperty(string key)
        {
            Property property;
            if (!properties.TryGetValue(key, out property))
            {
                throw new Exception("Property '" + key + "' not found.");
            }
            return property;
        }

        public static void Init(string fileName = null)
        {
            Config.fileName = fileName;

            if (fileName != null)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(fileName))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            line.Trim();
                            if (line.Length > 0 && !line.StartsWith("#"))
                            {
                                string[] tokens = line.Split('=');
                                if (tokens.Length != 2)
                                {
                                    throw new Exception("Invalid line '" + line + "'.");
                                }
                                string key = tokens[0];
                                string value = tokens[1];
                                if (properties.ContainsKey(key))
                                {
                                    properties[key].SetValueFromString(value);
                                }
                            }
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    Log.Debug("Config file '" + fileName + "' not found, using default values.");
                }
                catch (Exception ex)
                {
                    Log.Error("Failed to open config file '" + fileName + "', using default values.", ex);
                }
            }

            foreach(var entry in properties)
            {
                entry.Value.Init();
            }
        }

        private static void Save()
        {
            if (fileName == null)
            {
                return;
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    foreach (var entry in properties)
                    {
                        writer.WriteLine(entry.Key + "=" + entry.Value.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Failed to write config file '" + fileName + "'.", ex);
            }
        }
    }
}
