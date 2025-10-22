namespace DinoRunner.Interfaces
{
    // Interface for saving and loading game data
    public interface SaveService
    {
        void SaveGame<T>(string key, T data);
        T LoadGame<T>(string key, T defaultValue = default); // Use default if key not found
        bool HasSave(string key);
        void DeleteSave(string key);
        void DeleteAllSaves(); // for resetting all data
    }
}