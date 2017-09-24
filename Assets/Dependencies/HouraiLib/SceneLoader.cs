using HouraiTeahouse.AssetBundles;
using UnityEngine.SceneManagement;

namespace HouraiTeahouse {

    public static class SceneLoader {
        
        public static ITask LoadScene(string path, LoadSceneMode mode = LoadSceneMode.Single) {
            if (!path.Contains(Resource.BundleSeperator.ToString()))
                return SceneManager.LoadSceneAsync(path, mode).ToTask();
            string[] parts = path.Split(Resource.BundleSeperator);
            return AssetBundleManager.LoadLevelAsync(parts[0], parts[1], mode);
        }

    }

}
