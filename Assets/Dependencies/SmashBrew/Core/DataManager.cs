using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using HouraiTeahouse.AssetBundles;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.SmashBrew {

    public class GameDataCollection<T> : ReadOnlyCollection<T> where T : IGameData {
        
        public GameDataCollection(IList<T> list) : base(list){
        }

        public uint GroupHash {
            get {
                uint sum = 0;
                unchecked {
                    for (var i = 0; i < Count; i++)
                        if (this[i].IsSelectable)
                            sum += this[i].Id;
                }
                return sum;
            }
        }

        public string GroupHash64String {
            get {
                return Convert.ToBase64String(BitConverter.GetBytes(GroupHash)).Replace("=", "");
            }
        }


    }

    /// <summary> A manager of all of the game data loaded into the game. </summary>
    public sealed class DataManager : MonoBehaviour {

        static List<SceneData> _scenes;
        static List<CharacterData> _characterList;
        static Dictionary<uint, CharacterData> _characters;
        static ITask _loadTask;

        public static ITask LoadTask {
            get { return _loadTask ?? (LoadTask = new Task()); }
            private set { _loadTask = value; }
        }

        public static bool IsReady {
            get { return LoadTask != null && LoadTask.State != TaskState.Pending; }
        }

        [RuntimeInitializeOnLoadMethod]
        static void Initialize() {
            _characterList = new List<CharacterData>();
            _characters = new Dictionary<uint, CharacterData>();
            _scenes = new List<SceneData>();

            AssetBundleManager.AddHandler<CharacterData>(AddCharacter);
            AssetBundleManager.AddHandler<SceneData>(AddScene);

            Characters = new GameDataCollection<CharacterData>(_characterList);
            Scenes = new GameDataCollection<SceneData>(_scenes);

            SceneManager.sceneLoaded += SceneLoad;

            var bundlePath = BundleUtility.StoragePath;
            Debug.LogFormat("Storage Path: {0}", bundlePath);

            LoadTask.Then(() => {
                Debug.LogFormat("Character Group Hash: {0}", Characters.GroupHash64String);
                Debug.LogFormat("Scene Group Hash: {0}", Scenes.GroupHash64String);
            });

#if UNITY_EDITOR
            if (AssetBundleManager.SimulateAssetBundleInEditor) {
                LoadFromEditor<CharacterData>(AddCharacter);
                LoadFromEditor<SceneData>(AddScene);
                LoadTask.Resolve();
            }
            else
#endif
            {
                var dataFilePath = Path.Combine(bundlePath, "data");
                if (!File.Exists(dataFilePath)) {
                    Debug.LogFormat("Cannot find {0} copying StreamingAssets...", dataFilePath, bundlePath);
                    AssetBundleManager.CopyDirectory(Application.streamingAssetsPath, bundlePath);
                }

                var file = File.ReadAllText(dataFilePath);
                var whitelist = new List<string>();
                var blacklist = new List<string>();
                foreach (var entry in file.Split('\n')) {
                    if (entry.StartsWith("-")) {
                        blacklist.Add(entry.Substring(1).Trim());
                        Debug.LogFormat("Registered bundle blacklist: {0}", entry.Substring(1).Trim());
                    }
                    else {
                        whitelist.Add(entry.Trim());
                        Debug.LogFormat("Registered bundle whitelist: {0}", entry.Trim());
                    }
                }

                var task = AssetBundleManager.LoadLocalBundles(whitelist, blacklist);
                task.Then(() => {
                    LoadTask.Resolve();
                });
                task.Done();
            }
        }

        /// <summary> 
        /// All Characters that are included with the Game's build. The Data Manager will automatically load all
        /// CharacterData instances from Resources. 
        /// </summary>
        public static GameDataCollection<CharacterData> Characters { get; private set; }

        /// <summary> 
        /// All Scenes and their metadata included with the game's build. The DataManager will automatically load all
        /// SceneData instances from Resources. 
        /// </summary>
        public static GameDataCollection<SceneData> Scenes { get; private set; }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            var networkManager = SmashNetworkManager.Instance;
            if (networkManager == null)
                return;
            Mediator.Global.CreateUnityContext(this).Subscribe<NetworkClientStarted>(args => {
                return LoadTask.Then(() => {
                    return Task.All(Characters.SelectMany(character => GetPrefabs(character))
                        .Distinct()
                        .Select(resource => resource.LoadAsync())
                    ).Then(prefabs => {
                        foreach(var prefab in prefabs.Where(p => p != null))
                            ClientScene.RegisterPrefab(prefab);
                        foreach (var pref in ClientScene.prefabs)
                            Debug.LogFormat("Registered Network Prefab: {0} ({1})", pref.Value.name, pref.Key);
                    });
                });
            });
        }
        
        static IEnumerable<Resource<GameObject>> GetPrefabs(CharacterData character) {
            yield return character.Prefab;
            foreach(var prefab in character.ExtraPrefabs)
                yield return prefab;
        }

#if UNITY_EDITOR
        static void LoadFromEditor<T>(Action<T> loadFunc) where T : Object {
            var guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T).Name));
            foreach (string guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                    loadFunc(asset);
            }
        }
#endif

        public static void AddCharacter(CharacterData data) {
            if (_characters.ContainsKey(data.Id)) {
                Debug.LogWarning("Attempted to load {0} while already loaded.", data);
                return;
            }
            _characters[data.Id] = data;
            _characterList.Add(data);
            Debug.LogFormat("Registered {0} ({1}) as a valid character.", data.name, data.Id);
        }

        public static void AddScene(SceneData data) {
            if (_scenes.Contains(data)) {
                Debug.LogWarning("Attempted to load {0} while already loaded.", data);
                return;
            }
            _scenes.Add(data);
            Debug.LogFormat("Registered {0} as a valid scene.", data.name);
        }

        static void SceneLoad(Scene newScene, LoadSceneMode mode) {
            Debug.Log("Unloading managed data assets");
            foreach (SceneData scene in _scenes)
                scene.Unload();
            foreach (CharacterData character in _characters.Values)
                character.Unload();
        }

        /// <summary>
        /// Looks up a character by it's ID. Returns null if no character is found.
        /// </summary>
        /// <param name="id">the id of the character to lookup</param>
        /// <returns>The matching character</returns>
        public static CharacterData GetCharacter(uint id) {
            if (_characters .ContainsKey(id))
                return _characters[id];
            return null;
        }

    }

}
