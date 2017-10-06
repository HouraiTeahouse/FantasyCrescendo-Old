using UnityEngine;
using System.Linq;

namespace HouraiTeahouse.SmashBrew {

    /// <summary>
    /// A script to manage the dynamic loading of scenes at the start of the game.
    /// </summary>
    /// <remarks>
    /// This scripts runs after all scene and character information has been loaded by the
    /// <see cref="DataManager"/>. It will select one scene from the provided SceneDatas that meets
    /// the following requirements:
    ///  * Menus before Stages
    ///  * Highest load priority
    /// For example: if no Menus are selected, the highest priority Stage will be loaded as the base scene.
    /// However, if one Menu is available, it will be loaded instead of any of the other stages, even if it's
    /// LoadPriority is lower than every other provided Scene.
    /// </remarks>
    public class BootstrapSceneLoader : MonoBehaviour {

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() {
            var log = Log.GetLogger(this);
            DataManager.LoadTask.Then(() => {
                var scenes = DataManager.Scenes.OrderByDescending(s => s.Type).ThenByDescending(s => s.LoadPriority);
                var logStr = "Scene Considerations: ";
                foreach (var scene in scenes)
                    logStr += string.Format("\n   {0}: {1} {2}, Loadable: {3}", scene.name, scene.Type, scene.LoadPriority, scene.IsSelectable);
                log.Info(logStr);
                var startScene = scenes.FirstOrDefault(s => s.IsSelectable);
                if (startScene == null)
                    log.Error("No usable loadable scene found.");
                else {
                    log.Info("Loading {0} as the initial scene...", startScene.name);
                    startScene.Load();
                }
            });
        }

    }

}

