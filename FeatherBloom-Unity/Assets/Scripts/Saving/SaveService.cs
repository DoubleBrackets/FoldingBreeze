using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Saving
{
    public class SaveService
    {
        private const string SaveFileName = "FoldingBreezeSaveData.json";
        private const string SaveBackupFileName = "FoldingBreezeSaveData-Backup.json";

        private string _saveFilePath;
        private string _backupSaveFilePath;

        private SaveModel _saveModel;

        private bool _isAboutToSave;
        public SaveModel SaveModel => _saveModel;

        public string LastHandFanConnectedSerialPortName
        {
            get => _saveModel.LastHandFanConnectedSerialPortName;
            set
            {
                _saveModel.LastHandFanConnectedSerialPortName = value;
                Save();
            }
        }

        public string LastBoxFanConnectedSerialPortName
        {
            get => _saveModel.LastBoxFanConnectedSerialPortName;
            set
            {
                _saveModel.LastBoxFanConnectedSerialPortName = value;
                Save();
            }
        }

        public int HighScore
        {
            get => _saveModel.HighScore;
            set
            {
                _saveModel.HighScore = value;
                Save();
            }
        }

        public Quaternion DefaultZeroedOrientation
        {
            get => _saveModel.DefaultZeroedOrientation;
            set
            {
                _saveModel.DefaultZeroedOrientation = value;
                Save();
            }
        }

        public SaveService()
        {
            string persistentDataPath = Application.persistentDataPath;

            _saveFilePath = Path.Combine(persistentDataPath, SaveFileName);
            _backupSaveFilePath = Path.Combine(persistentDataPath, SaveBackupFileName);

            _saveModel = TryLoadSaveModel();
        }

        private SaveModel TryLoadSaveModel()
        {
            try
            {
                return JsonUtility.FromJson<SaveModel>(File.ReadAllText(_saveFilePath));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogWarning("Failed to load save file, loading backup instead.");

                return TryLoadBackupSaveModel();
            }
        }

        private SaveModel TryLoadBackupSaveModel()
        {
            try
            {
                return JsonUtility.FromJson<SaveModel>(File.ReadAllText(_backupSaveFilePath));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new SaveModel();
            }
        }

        private void Save()
        {
            SaveModelDelayed().Forget();
        }

        /// <summary>
        ///     Saves after a frame, to avoid many writes on the same frame (mainly when the UI loads)
        /// </summary>
        private async UniTaskVoid SaveModelDelayed()
        {
            if (_isAboutToSave)
            {
                return;
            }

            _isAboutToSave = true;

            await UniTask.DelayFrame(1);

            DoSaveImmediately();

            _isAboutToSave = false;
        }

        private void DoSaveImmediately()
        {
            Debug.Log("Saving save file...");
            try
            {
                // Backup old save file
                if (File.Exists(_saveFilePath))
                {
                    File.Copy(_saveFilePath, _backupSaveFilePath, true);
                }

                // Write new save file
                File.WriteAllText(_saveFilePath, JsonUtility.ToJson(_saveModel, true));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}