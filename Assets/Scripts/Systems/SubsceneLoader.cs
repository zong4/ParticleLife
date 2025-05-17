using System.Collections;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

namespace Systems
{
    public class SubSceneLoader : MonoBehaviour
    {
        public SubScene subSceneToLoad;
        private Entity subScene;

        private void Awake()
        {
            var loadParameters = new SceneSystem.LoadParameters { Flags = SceneLoadFlags.NewInstance };

            subScene = SceneSystem.LoadSceneAsync(World.DefaultGameObjectInjectionWorld.Unmanaged,
                subSceneToLoad.SceneGUID, loadParameters);

            StartCoroutine(CheckScene());
        }

        // ReSharper disable once UnusedMember.Local
        private void UnLoadSubScene()
        {
            // Specify unload parameters, you can adjust these based on your requirements.
            const SceneSystem.UnloadParameters unloadParameters = SceneSystem.UnloadParameters.DestroyMetaEntities;

            SceneSystem.UnloadScene(World.DefaultGameObjectInjectionWorld.Unmanaged, subScene, unloadParameters);
        }

        private IEnumerator CheckScene()
        {
            while (!SceneSystem.IsSceneLoaded(World.DefaultGameObjectInjectionWorld.Unmanaged, subScene))
            {
                yield return null;
            }
        }
    }
}