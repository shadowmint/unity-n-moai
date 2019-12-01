using System.IO;
using N.Package.Moai.LuaUtility;
using UnityEngine;

namespace N.Package.Moai.Example
{
    [ExecuteInEditMode]
    public class LuaGameBuild : MonoBehaviour
    {
        public string rootFolder = "pkg-all/n-moai/N/Moai/Example";

        public UnityLuaScriptLoaderLoadMode mode;

        public bool doSourceFileSync = true;

        public void Update()
        {
#if UNITY_EDITOR
            if (!doSourceFileSync) return;
            doSourceFileSync = false;
            CopySourceToDist();
#endif
        }

        private void CopySourceToDist()
        {
            var assetsFolder = Application.dataPath;
            var sourcesFolder = Path.Combine(assetsFolder, rootFolder, "src");
            var targetFolder = Path.Combine(assetsFolder, rootFolder, "Resources", "src");

            if (!Directory.Exists(sourcesFolder))
            {
                Debug.LogError($"Unable to process file; Missing folder: {sourcesFolder}");
                return;
            }

            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            var source = new LuaSourceCodeUtility(sourcesFolder, targetFolder, mode);
            source.CopySourcesToDestination();
        }
    }
}