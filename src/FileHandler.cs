using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Xml.Serialization;
using Windows.Storage.AccessCache;
using Windows.Storage.Streams;

namespace RS3QuestFilter.src
{
    public enum ContextType
    {
        Local,
        LocalCache,
        LocalShared,
        Roaming,
        Temp,
        User
    }

    public static class FileHandler
    {
        private static StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;
        private static StorageFolder LocalCacheFolder = ApplicationData.Current.LocalCacheFolder;
        private static StorageFolder LocalSharedFolder = ApplicationData.Current.SharedLocalFolder;
        private static StorageFolder RoamingFolder = ApplicationData.Current.RoamingFolder;
        private static StorageFolder TempFolder = ApplicationData.Current.TemporaryFolder;

        private static ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        private static ApplicationDataContainer RoamingSettings = ApplicationData.Current.RoamingSettings;

        private static bool IsSetup = false;

        private static async Task init()
        {
            if (!IsSetup)
            {
                await GenerateFolder(ContextType.Local);
                
                //foreach (string s in LocalSettings.Values.Keys)
                //    LocalSettings.Values.Remove(s);
                
                GenerateSettings(ContextType.Local);
            }
            IsSetup = true;
        }

        private static async Task GenerateFolder(ContextType contextType)
        {
            StorageFolder context = GetContextType(contextType);

            await context.CreateFolderAsync("RS3 Quests", CreationCollisionOption.OpenIfExists);

        }

        public static void GenerateSettings(ContextType contextType)
        {
            ApplicationDataContainer context;
            switch (contextType)
            {
                case ContextType.Local:
                    context = LocalSettings;
                    break;
                case ContextType.Roaming:
                    context = RoamingSettings;
                    break;
                default:
                    throw new InvalidOperationException("Unable to generate settings: Invalid context.");
            }
            context.Values.TryAdd("UserSavePath", "");
            context.Values.TryAdd("UserSaveFolder", "");
            context.Values.TryAdd("UserSaveFilename", "");

        }

        private static async Task<StorageFolder?> PickUserFolder()
        {
            Windows.Storage.Pickers.FolderPicker folderPicker = new();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            
            if (folder == null) return null;

            RememberFolder(folder, "UserSaveFolder");
            return folder;
        }

        private static StorageFolder GetContextType(ContextType type)
        {
            StorageFolder context = null;
            switch (type)
            {
                case ContextType.Local:
                    context = LocalFolder;
                    break;
                case ContextType.LocalCache:
                    context = LocalCacheFolder;
                    break;
                case ContextType.LocalShared:
                    context = LocalSharedFolder;
                    break;
                case ContextType.Roaming:
                    context = RoamingFolder;
                    break;
                default:
                    context = LocalFolder;
                    break;
            }
            return context;
        
        }

        private static async Task SaveData()
        {

            StorageFile quests = await LocalFolder.CreateFileAsync("QuestLog.xml", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(quests, MyIO.SerialiseToXML(App.ViewModel.VMQuests.QuestLog));


            App.ViewModel.VMPlayer.PlayerData.PrepareSerialisable();
            StorageFile player = await LocalFolder.CreateFileAsync("PlayerData.xml", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(player, MyIO.SerialiseToXML(App.ViewModel.VMPlayer.PlayerData));

        }

        private static T GetSetting<T>(string key, ContextType contextType)
        {
            ApplicationDataContainer settings;
            switch (contextType)
            {
                case ContextType.Local:
                    settings = LocalSettings;
                    break;
                case ContextType.Roaming:
                    settings = RoamingSettings;
                    break;
                default:
                    throw new InvalidOperationException($"Unknown context {contextType}. Expected either Local or Roaming.");
            }

            if (settings.Values.ContainsKey(key))
                return (T)Convert.ChangeType(settings.Values[key], typeof(T));
            
            throw new KeyNotFoundException("Key not found in settings: " + key);
        }

        public static void RememberFolder(StorageFolder folder, string tokenName) => StorageApplicationPermissions.FutureAccessList.AddOrReplace(tokenName, folder);
        

        public static void RememberFile(StorageFile file, string tokenName) => StorageApplicationPermissions.FutureAccessList.AddOrReplace(tokenName, file);


        private static string GetFileFolderPath(StorageFile file)
        {
            if (file == null)
                return null;

            return file.Path.Substring(0, file.Path.Length - file.Name.Length);
        }

        public static async Task<StorageFile> GetFileForToken(string token)
        {
            if (!StorageApplicationPermissions.FutureAccessList.ContainsItem(token)) return null;
            return await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
        }        

        private static async Task<StorageFile?> ExportUserFile()
        {
            Windows.Storage.Pickers.FileSavePicker savePicker = new()
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "PlayerData"
            };
            
            savePicker.FileTypeChoices.Add("Player Data File", new List<string>() { ".xml" });
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                RememberFile(file, "UserSaveFile");
                
                UpdateLocalSetting("UserSavePath", file.Path);
                UpdateLocalSetting("UserSaveFolder", GetFileFolderPath(file));
                UpdateLocalSetting("UserSaveFilename", file.Name);

                App.ViewModel.VMPlayer.PlayerData.PrepareSerialisable();
                string data = MyIO.SerialiseToXML(App.ViewModel.VMPlayer.PlayerData);
                try
                {
                    await FileIO.WriteTextAsync(file, data);
                }
                catch (Exception ex)
                {
                    await MainPage.ShowAlert(ex.Message);
                    throw;
                }
                finally
                {
                    Debug.WriteLine($"File ({file.Name}) saved successfully at: '{file.Path}'");
                }
                return file;
            }
            else
            {
                return null;
            }
        }

        private static async Task<Player> ImportUserFile()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.List,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add(".xml");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                if (!file.Path.Equals(GetSetting<string>("UserSavePath", ContextType.Local)))
                {
                    RememberFile(file, "UserSaveFilename");
                    UpdateOrAddSetting("UserSavePath", file.Path, ContextType.Local);
                    UpdateOrAddSetting("UserSaveFilename", file.Name, ContextType.Local);
                    UpdateOrAddSetting("UserSaveFolder", GetFileFolderPath(file), ContextType.Local);
                }

                Player player = null;
                try
                {
                    player = await MyIO.DeserialiseFromXML<Player>(file);
                }
                catch (Exception e)
                {
                    await MainPage.ShowAlert(e.Message);
                    throw;
                }

                return player ?? null;
            }   
            else
            {
                return null;
            }
        }

        private static async Task<T> LoadData<T>(ContextType contextType) where T : class
        {
            StorageFolder context = GetContextType(contextType);

            switch (typeof(T))
            {
                case Type type when type == typeof(QuestLog):
                    {
                        StorageFile? questFile = null;
                        if (context != null)
                            questFile = await context.GetFileAsync("QuestLog.xml");

                        if (questFile != null)
                        {
                            T log = null;
                            try
                            {
                                log = MyIO.DeserialiseFromXML<QuestLog>(questFile.Path.ToString()) as T;
                            }
                            catch (Exception e)
                            {
                                await MainPage.ShowAlert(e.Message);
                            }
                            if (log != null)
                            {
                                return log;
                            }
                        }
                    }
                    break;

                case Type type when type == typeof(Player):

                    StorageFile? file = null;

                    if (LocalSettings.Values.ContainsKey("UserSavePath"))
                    {
                        if (!LocalSettings.Values["UserSavePath"].Equals(""))
                        {
                            file = await StorageFile.GetFileFromPathAsync((string)LocalSettings.Values["UserSavePath"]);
                        }
                        else
                        {
                            file = await LocalFolder.GetFileAsync("PlayerData.xml");
                        }
                    }
                    else
                    {
                        file = await LocalFolder.GetFileAsync("PlayerData.xml");
                    }

                    if (file != null)
                    {
                        T player = null;
                        try
                        {
                            player = MyIO.DeserialiseFromXML<Player>(file.Path.ToString()) as T;
                        }
                        catch (Exception e)
                        {
                            await MainPage.ShowAlert(e.Message);
                        }

                        if (player != null)
                        {
                            UpdateLocalSetting("UserSaveFolder", GetFileFolderPath(file));
                            UpdateLocalSetting("UserSaveFilename", file.Name);
                            UpdateLocalSetting("UserSavePath", file.Path);
                            (player as Player).PrepareSkills();
                            return player;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
            }
            return null;
        }

        private static async void AddAppFile(string filename, StorageFile file, ContextType contextType) 
        {
            StorageFolder? context = null;
            switch (contextType)
            {
                case ContextType.Local:
                    context = LocalFolder;
                    break;
                case ContextType.LocalCache:
                    context = LocalCacheFolder;
                    break;
                case ContextType.LocalShared:
                    context = LocalSharedFolder;
                    break;
                case ContextType.Roaming:
                    context = RoamingFolder;
                    break;
                case ContextType.Temp:
                    context = TempFolder;
                    break;
            }

            _ = await context?.CreateFileAsync(filename, CreationCollisionOption.FailIfExists);

        }

        private static void UpdateSetting(string setting, ValueType value, ContextType contextType)
        {
            switch (contextType)
            {
                case ContextType.Local:
                    {
                        if (LocalSettings.Values.ContainsKey(setting))
                            LocalSettings.Values[setting] = value;
                        else
                            throw new KeyNotFoundException($"Key '{setting}' not found in LocalSettings");
                        break;
                    }
                case ContextType.Roaming:
                    {
                        if (RoamingSettings.Values.ContainsKey(setting))
                            RoamingSettings.Values[setting] = value;
                        else
                            throw new KeyNotFoundException($"Key '{setting}' not found in RoamingSettings");
                        break;
                    }
            }
        }
        private static void UpdateSetting(string setting, string value, ContextType contextType)
        {
            switch (contextType)
            {
                case ContextType.Local:
                    {
                        if (LocalSettings.Values.ContainsKey(setting))
                            LocalSettings.Values[setting] = value;
                        else
                            throw new KeyNotFoundException($"Key '{setting}' not found in LocalSettings");
                        break;
                    }
                case ContextType.Roaming:
                    {
                        if (RoamingSettings.Values.ContainsKey(setting))
                            RoamingSettings.Values[setting] = value;
                        else
                            throw new KeyNotFoundException($"Key '{setting}' not found in RoamingSettings");
                        break;
                    }
            }
        }

        private static void UpdateOrAddSetting(string setting, ValueType value, ContextType contextType)
        {
            var context = (contextType is ContextType.Local) ? LocalSettings : ((contextType is ContextType.Roaming) ? RoamingSettings : null);
            if (context is null) throw new InvalidOperationException("Context is not Local or Roaming...");
            try
            {
                UpdateSetting(setting, value, contextType);
            }
            catch (KeyNotFoundException)
            {
                context.Values.TryAdd(setting, value);
            }
            return;
        }
        private static void UpdateOrAddSetting(string setting, string value, ContextType contextType)
        {
            var context = (contextType is ContextType.Local) ? LocalSettings : ((contextType is ContextType.Roaming) ? RoamingSettings : null);
            if (context is null) throw new InvalidOperationException("Context is not Local or Roaming...");
            try
            {
                UpdateSetting(setting, value, contextType);
            }
            catch (KeyNotFoundException)
            {
                context.Values.TryAdd(setting, value);
            }
        }

        public static void UpdateLocalSetting(string setting, ValueType value) => UpdateSetting(setting, value, ContextType.Local);
        public static void UpdateRoamingSetting(string setting, ValueType value) => UpdateSetting(setting, value, ContextType.Roaming);
        public static void UpdateLocalSetting(string setting, string value) => UpdateSetting(setting, value, ContextType.Local);
        public static void UpdateRoamingSetting(string setting, string value) => UpdateSetting(setting, value, ContextType.Roaming);

        public static async Task<QuestLog> GetQuestLog()
        {
           return await LoadData<QuestLog>(ContextType.Local);
        }

        public static async Task<Player> GetPlayerData()
        {
            return await LoadData<Player>(ContextType.Local);
        }

        public static async Task SaveAll() => await SaveData();
        
        public static async Task Export() => await ExportUserFile();
        
        public static async Task<Player> Import() => await ImportUserFile();

        public static async Task Init() => await init();

        public static string GetPlayerDataPath() => GetSetting<string>("UserSavePath", ContextType.Local);
    }
}