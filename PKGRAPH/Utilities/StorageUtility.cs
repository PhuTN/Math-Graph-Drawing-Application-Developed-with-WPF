using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PKGRAPH.Models;
using Newtonsoft.Json;

namespace PKGRAPH.Utilities
{


    public static class StorageUtility
    {
        private const string AppFolderName = "FunctionToGraph";
        private const string GraphModelsFileName = "graph_models.json";

        private static string AppDataPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        private static DirectoryInfo _appDirectoryInfo;

        static StorageUtility()
        {
            CheckApplicationFolder();
        }

        private static void CheckApplicationFolder()
        {
            string path = Path.Combine(AppDataPath, AppFolderName);
            _appDirectoryInfo = new DirectoryInfo(path);

            if (!_appDirectoryInfo.Exists)
            {
                _appDirectoryInfo.Create();
            }
        }

        public static async void SaveGraphModelsAsync(IEnumerable<GraphModel> graphModels, string savedFile) 
        {
            //FileInfo file = new FileInfo(savedFile);
            //if (!file.Exists)
            //{
            //    file.Create();
            //}

            string jsonNotation = JsonConvert.SerializeObject(graphModels, Formatting.Indented);
            

            File.WriteAllText(savedFile, jsonNotation);
        }

        public static async Task<IEnumerable<GraphModel>> ReadGraphModelsAsync(string fullFileName)
        {
           

            if (!File.Exists(fullFileName))
            {
                return new List<GraphModel>();
            }

            string jsonNotation = File.ReadAllText(fullFileName);
            IEnumerable<GraphModel> graphModels = JsonConvert.DeserializeObject<IEnumerable<GraphModel>>(jsonNotation);

            return graphModels ?? new List<GraphModel>();
        }

    }
}