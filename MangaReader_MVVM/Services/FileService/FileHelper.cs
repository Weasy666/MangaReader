using Microsoft.Toolkit.Uwp.Services.OneDrive;
using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MangaReader_MVVM.Services.FileService
{
    public static class FileHelper
    {
        /// <summary>Returns if a file is found in the specified storage strategy</summary>
        /// <param name="key">Path of the file in storage</param>
        /// <param name="location">Location storage strategy</param>
        /// <returns>Boolean: true if found, false if not found</returns>
        public static async Task<bool> FileExistsAsync(string key, StorageStrategies location = StorageStrategies.Local) => (await GetIfFileExistsAsync(key, location)) != null;

        public static async Task<bool> FileExistsAsync(string key, StorageFolder folder) => (await GetIfFileExistsAsync(key, folder)) != null;

        /// <summary>Deletes a file in the specified storage strategy</summary>
        /// <param name="key">Path of the file in storage</param>
        /// <param name="location">Location storage strategy</param>
        public static async Task<bool> DeleteFileAsync(string key, StorageStrategies location = StorageStrategies.Local)
        {
            if (location == StorageStrategies.OneDrive)
            {
                var appRootFolder = await OneDriveService.Instance.AppRootFolderAsync();
                var remoteFile = await appRootFolder.GetFileAsync(key);
                if (remoteFile != null)
                    await remoteFile.DeleteAsync();
            }
            else
            {
                var file = await GetIfFileExistsAsync(key, location);
                if (file != null)
                    await file.DeleteAsync();
            }

            return !(await FileExistsAsync(key, location));
        }

        /// <summary>Reads and deserializes a file into specified type T</summary>
        /// <typeparam name="T">Specified type into which to deserialize file content</typeparam>
        /// <param name="key">Path to the file in storage</param>
        /// <param name="location">Location storage strategy</param>
        /// <returns>Specified type T</returns>
        public static async Task<T> ReadFileAsync<T>(string key, StorageStrategies location = StorageStrategies.Local)
        {
            try
            {
                // fetch file
                var file = await GetIfFileExistsAsync(key, location);
                if (file == null)
                    return default(T);
                // read content
                var readValue = await FileIO.ReadTextAsync(file);
                // convert to obj
                var _Result = Deserialize<T>(readValue);
                return _Result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>Serializes an object and write to file in specified storage strategy</summary>
        /// <typeparam name="T">Specified type of object to serialize</typeparam>
        /// <param name="key">Path to the file in storage</param>
        /// <param name="value">Instance of object to be serialized and written</param>
        /// <param name="location">Location storage strategy</param>
        public static async Task<bool> WriteFileAsync<T>(string key, T value, StorageStrategies location = StorageStrategies.Local,
            CreationCollisionOption option = CreationCollisionOption.ReplaceExisting)
        {
            // create file
            var file = await CreateFileAsync(key, location, option);
            // convert to string
            var serializedValue = Serialize(value);
            // save string to file
            await FileIO.WriteTextAsync(file, serializedValue);
            if (location == StorageStrategies.OneDrive)
            {
                var appRootFolder = await OneDriveService.Instance.AppRootFolderAsync();

                using (var localStream = await file.OpenReadAsync())
                {
                    var fileCreated = await appRootFolder.CreateFileAsync(key, option, localStream);
                }
            }
            return await FileExistsAsync(key, location);
        }

        private static async Task<StorageFile> CreateFileAsync(string key, StorageStrategies location = StorageStrategies.Local,
            CreationCollisionOption option = CreationCollisionOption.OpenIfExists)
        {
            switch (location)
            {
                case StorageStrategies.Local:
                    return await ApplicationData.Current.LocalFolder.CreateFileAsync(key, option);
                case StorageStrategies.Roaming:
                    return await ApplicationData.Current.RoamingFolder.CreateFileAsync(key, option);
                case StorageStrategies.OneDrive:
                    return await GetOneDriveFile(key, option);
                case StorageStrategies.Temporary:
                    return await ApplicationData.Current.TemporaryFolder.CreateFileAsync(key, option);
                default:
                    throw new NotSupportedException(location.ToString());
            }
        }

        private static async Task<StorageFile> GetIfFileExistsAsync(string key, StorageFolder folder)
        {
            StorageFile retval;
            try
            {
                retval = await folder.GetFileAsync(key);
            }
            catch (System.IO.FileNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("GetIfFileExistsAsync:FileNotFoundException");
                return null;
            }
            return retval;
        }

        /// <summary>Returns a file if it is found in the specified storage strategy</summary>
        /// <param name="key">Path of the file in storage</param>
        /// <param name="location">Location storage strategy</param>
        /// <returns>StorageFile</returns>
        private static async Task<StorageFile> GetIfFileExistsAsync(string key,
            StorageStrategies location = StorageStrategies.Local)
        {
            StorageFile retval;
            try
            {
                switch (location)
                {
                    case StorageStrategies.Local:
                        retval = await ApplicationData.Current.LocalFolder.TryGetItemAsync(key) as StorageFile;
                        break;
                    case StorageStrategies.OneDrive:                        
                        retval = await GetOneDriveFile(key);
                        break;
                    case StorageStrategies.Roaming:
                        retval = await ApplicationData.Current.RoamingFolder.TryGetItemAsync(key) as StorageFile;
                        break;
                    case StorageStrategies.Temporary:
                        retval = await ApplicationData.Current.TemporaryFolder.TryGetItemAsync(key) as StorageFile;
                        break;                    
                    default:
                        throw new NotSupportedException(location.ToString());
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("GetIfFileExistsAsync:FileNotFoundException");
                return null;
            }
            return retval;
        }

        private static async Task<StorageFile> GetOneDriveFile(string key, CreationCollisionOption option = CreationCollisionOption.ReplaceExisting)
        {
            var appRootFolder = await OneDriveService.Instance.AppRootFolderAsync();
            var remoteFile = await appRootFolder.GetFileAsync(key);
            DateTimeOffset? remoteFileTime = remoteFile.DateModified;
            StorageFile localFile;
            if (remoteFileTime.Value.LocalDateTime.AddSeconds(-3) >= SettingsServices.SettingsService.Instance.LastSynced) {
                localFile = await CreateFileAsync(key, StorageStrategies.Temporary, option);
                if (remoteFile != null)
                {
                    using (var remoteStream = await remoteFile.OpenAsync())
                    {
                        byte[] buffer = new byte[remoteStream.Size];
                        var localBuffer = await remoteStream.ReadAsync(buffer.AsBuffer(), (uint)remoteStream.Size, InputStreamOptions.ReadAhead);

                        using (var localStream = await localFile.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            await localStream.WriteAsync(localBuffer);
                            await localStream.FlushAsync();
                        }
                    }
                }
            }
            else
            {
                localFile = await CreateFileAsync(key, StorageStrategies.Local);
            }
            return localFile;
        }

        private static string Serialize<T>(T item) => JsonConvert.SerializeObject(item, Formatting.None, new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            TypeNameHandling = TypeNameHandling.Objects,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
        });

        private static T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json);
    }
}
