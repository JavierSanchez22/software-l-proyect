using UnityEngine;
using System.IO;
using System.Text;
using RedRunner.Interfaces;
using RedRunner.Patterns.Singleton;
using RedRunner.Core; // For ServiceLocator
using BayatGames.SaveGameFree.Serializers; // Using BayatGames serializers
using BayatGames.SaveGameFree.Encoders; // Using BayatGames encoders


namespace RedRunner.Services
{
    public class SaveService : SingletonMonoBehaviour<SaveService>,
    
    ISaveService
    {
        [Header("Save Settings")]
        [SerializeField]
        private SaveFormat defaultSaveFormat = SaveFormat.JSON;
        [SerializeField] private bool encodeByDefault = false;
        [SerializeField] private string encodePassword = "yourSecretPassword";
        // CHANGE THIS!
        [SerializeField]
        private SaveGamePath savePath = SaveGamePath.PersistentDataPath;
        private ISaveGameSerializer serializer;
        private ISaveGameEncoder encoder;
        private Encoding encoding = Encoding.UTF8;
        // Enum matching BayatGames structure if needed, or simplify
        public enum SaveFormat { JSON, XML, Binary }
        public enum SaveGamePath { PersistentDataPath, DataPath }

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;
            // Initialize serializer based on selection
            switch (defaultSaveFormat)
            {
                case SaveFormat.JSON:
                    serializer = new
                SaveGameJsonSerializer(); break;
                case SaveFormat.XML:
                    serializer = new SaveGameXmlSerializer();
                    break;
                case SaveFormat.Binary:
                    serializer = new
                SaveGameBinarySerializer(); break;
                default: serializer = new SaveGameJsonSerializer(); break;
            }
            encoder = new SaveGameSimpleEncoder(); // Or other BayatGames
            encoder ServiceLocator.Instance.RegisterService<ISaveService>(this);
        }

        private string GetFilePath(string key)
        {
            string directory = "";
            switch (savePath)
            {
                case SaveGamePath.PersistentDataPath:
                    directory = Application.persistentDataPath; break;
                case SaveGamePath.DataPath:
                    directory = Application.dataPath;
                    break;
                default: 
                    directory = Application.persistentDataPath; break;
            }
            // Combine path safely
            return Path.Combine(directory, key + GetExtension());
        }

        private string GetExtension()
        {
            switch (defaultSaveFormat)
            {
                case SaveFormat.JSON: return ".json";
                case SaveFormat.XML: return ".xml";
                case SaveFormat.Binary: return ".bin";
                default: return ".sav";
            }
        }


        // --- ISaveService Implementation ---
        public void SaveGame<T>(string key, T data)
        {
            string filePath = GetFilePath(key);
            try
            {
                // Ensure directory exists
                string directoryPath = Path.GetDirectoryName(filePath);

                #if !UNITY_WEBGL // IO operations not directly supported in
                                WebGL in the same way
                            if (!Directory.Exists(directoryPath))
                                    Directory.CreateDirectory(directoryPath);
                #endif
                
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    serializer.Serialize<T>(data, memoryStream, encoding);
                    byte[] bytes = memoryStream.ToArray();
                    if (encodeByDefault)
                    {
                        string unencodedString = encoding.GetString(bytes);
                        string encodedString = encoder.Encode(unencodedString, encodePassword);
                        bytes = encoding.GetBytes(encodedString);
                    }
                    // Save bytes (handles WebGL via PlayerPrefs, others via
                    File IO)
                #if UNITY_WEBGL
                PlayerPrefs.SetString(key,
                System.Convert.ToBase64String(bytes)); // Store as Base64 in PlayerPrefs for
                WebGL
                PlayerPrefs.Save();
                #else
                File.WriteAllBytes(filePath, bytes);
                #endif
                }
                // Debug.Log($"[SaveService] Saved '{key}' to {filePath}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[SaveService] Error saving '{key}' to { filePath}: { ex.Message}");
            }
        }
        public T LoadGame<T>(string key, T defaultValue = default)
        {
            string filePath = GetFilePath(key);
            T result = defaultValue;
            try
            {
                byte[] bytes = null;
                
                #if UNITY_WEBGL
                if (PlayerPrefs.HasKey(key))
                {
                string base64 = PlayerPrefs.GetString(key);
                bytes = System.Convert.FromBase64String(base64);
                } else return defaultValue; // Key not found in
                PlayerPrefs
                #else
                                if (!File.Exists(filePath)) return defaultValue; // File
                                not found
                            bytes = File.ReadAllBytes(filePath);
                #endif

                if (bytes == null || bytes.Length == 0) return defaultValue;
                if (encodeByDefault)
                {
                    string encodedString = encoding.GetString(bytes);
                    string decodedString = encoder.Decode(encodedString, encodePassword);
                    bytes = encoding.GetBytes(decodedString);
                }
                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    result = serializer.Deserialize<T>(memoryStream, encoding);
                }
                // Debug.Log($"[SaveService] Loaded '{key}' from {filePath}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[SaveService] Error loading '{key}' from { filePath}: { ex.Message}. Returning default value.");
                result = defaultValue; // Return default on error
            }
            // Return default if deserialization failed (result might be null / default)
            return EqualityComparer<T>.Default.Equals(result, default) ? defaultValue : result;
        }

        public bool HasSave(string key)
        {
            string filePath = GetFilePath(key);

            #if UNITY_WEBGL
            return PlayerPrefs.HasKey(key);
            #else
                        return File.Exists(filePath);
            #endif

        }
        public void DeleteSave(string key)
        {
            string filePath = GetFilePath(key);

            #if UNITY_WEBGL
            PlayerPrefs.DeleteKey(key);
            #else

            try { if (File.Exists(filePath)) File.Delete(filePath); }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error deleting file { filePath}: { ex.Message}"); }
            #endif
        }

public void DeleteAllSaves()
        {
            string directory = "";
            switch (savePath)
            {
                case SaveGamePath.PersistentDataPath: directory = Application.persistentDataPath; 
                    break;
                case SaveGamePath.DataPath: directory = Application.dataPath;
                    break;
            }

            #if UNITY_WEBGL

            PlayerPrefs.DeleteAll(); // Deletes ALL player prefs, use
            with caution
            Debug.LogWarning("[SaveService] DeleteAllSaves on WebGL
            clears ALL PlayerPrefs.");
            #else
            
            try
            {
                if (Directory.Exists(directory))
                {
                    // Delete files matching the extension, or all files
                    if careful
                    string[] files = Directory.GetFiles(directory, "*" + GetExtension());
                    foreach (string file in files) { File.Delete(file); }
                    Debug.Log($"[SaveService] Deleted all files extension '{GetExtension()}' in { directory}");
                // Optionally delete directories too, but be very careful
                }
            }

            catch (System.Exception ex)
            {
                Debug.LogError($"Error deleting all saves in { directory}: { ex.Message}"); }
            #endif
            }

protected override void OnDestroy()
        {
            if (ServiceLocator.Instance != null && ServiceLocator.Instance.HasService<ISaveService>() && ServiceLocator.Instance.GetService<ISaveService>() == this)
            {
                ServiceLocator.Instance.UnregisterService<ISaveService>();
            }
            base.OnDestroy();
        }
    }
}
