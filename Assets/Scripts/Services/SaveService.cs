using UnityEngine;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Patterns.Singleton;
using Assets.Scripts.Core; // For ServiceLocator
using System;
using System.IO; // Para manejo de archivos
using System.Text; // Para Encoding
using System.Collections.Generic;

namespace Assets.Scripts.Services
{
    public class SaveService : SingletonMonoBehaviour<SaveService>, ISaveService
    {
        [Header("Save Settings")]
        [SerializeField] private SaveGamePath savePath = SaveGamePath.PersistentDataPath;

        public enum SaveGamePath { PersistentDataPath, DataPath }
        private Encoding encoding = Encoding.UTF8;

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;
            ServiceLocator.Instance.RegisterService<ISaveService>(this);
        }

        private string GetFilePath(string key)
        {
            string directory = (savePath == SaveGamePath.DataPath) ? Application.dataPath : Application.persistentDataPath;
            return Path.Combine(directory, key + ".json"); // Siempre guardar como .json
        }

        public void SaveGame<T>(string key, T data)
        {
            string filePath = GetFilePath(key);
            try
            {
                string json = JsonUtility.ToJson(data, true);

                string directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

                File.WriteAllText(filePath, json, encoding);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveService-JSON] Error saving '{key}' to {filePath}: {ex.Message}");
            }
        }

        public T LoadGame<T>(string key, T defaultValue = default)
        {
            string filePath = GetFilePath(key);
            if (!File.Exists(filePath))
            {
                return defaultValue;
            }

            try
            {
                string json = File.ReadAllText(filePath, encoding);

                if (string.IsNullOrEmpty(json))
                {
                    return defaultValue;
                }

                T data = JsonUtility.FromJson<T>(json);
                return EqualityComparer<T>.Default.Equals(data, default) ? defaultValue : data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveService-JSON] Error loading '{key}' from {filePath}: {ex.Message}. Returning default.");
                return defaultValue;
            }
        }

        public bool HasSave(string key)
        {
            return File.Exists(GetFilePath(key));
        }

        public void DeleteSave(string key)
        {
            string filePath = GetFilePath(key);
            try
            {
                if (File.Exists(filePath)) File.Delete(filePath);
            }
            catch (Exception ex) { Debug.LogError($"Error deleting file {filePath}: {ex.Message}"); }
        }

        public void DeleteAllSaves()
        {
            string directory = (savePath == SaveGamePath.DataPath) ? Application.dataPath : Application.persistentDataPath;
            try
            {
                if (Directory.Exists(directory))
                {
                    string[] files = Directory.GetFiles(directory, "*.json"); // Solo borrar .json
                    foreach (string file in files) { File.Delete(file); }
                    Debug.Log($"[SaveService-JSON] Deleted all '.json' files in {directory}");
                }
            }
            catch (Exception ex) { Debug.LogError($"Error deleting all saves in {directory}: {ex.Message}"); }
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