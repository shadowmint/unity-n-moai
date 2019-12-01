using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using UnityEngine;

namespace Runtime
{
    [ExecuteInEditMode]
    public class LuaGameBuild : MonoBehaviour
    {
        public string rootFolder = "pkg-all/n-moai/N/Moai/Example";

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

            var source = new LuaSourceCodeUtility(sourcesFolder, targetFolder, ".bytes");
            var workerScript = new Script {Options = {ScriptLoader = new FileSystemScriptLoader()}};
            source.CopySourcesToDestination((fileRef) =>
            {
                if (!fileRef.DestinationPathExists)
                {
                    Directory.CreateDirectory(fileRef.DestinationPath);
                }

                if (fileRef.DestinationFileChanged || !fileRef.DestinationFileExists)
                {
                    if (fileRef.DestinationFileExists)
                    {
                        File.Delete(fileRef.DestinationFile);
                    }

                    Debug.Log($"Compile: {fileRef.SourceFile}");
                    var chunk = workerScript.LoadFile(fileRef.SourceFile);

                    using (Stream stream = new FileStream(fileRef.DestinationFile, FileMode.Create, FileAccess.Write))
                        workerScript.Dump(chunk, stream);

                    Debug.Log($"Updated: {fileRef.DestinationFile}");
                }
            });
        }
    }
}