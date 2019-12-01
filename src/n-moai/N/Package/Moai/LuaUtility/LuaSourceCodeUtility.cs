using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using UnityEngine;

namespace N.Package.Moai.LuaUtility
{
    public class LuaSourceCodeUtility
    {
        private const string MetaFileExtension = ".meta";
        private const string TextAssetExtension = ".txt";
        private const string BinaryAssetExtension = ".bytes";

        private readonly string _sourcePath;
        private readonly string _destinationPath;
        private readonly Queue<string> _pendingFolders;
        private readonly UnityLuaScriptLoaderLoadMode _outputMode;
        private Script _compilerScript;

        public LuaSourceCodeUtility(string sourcePath, string destinationPath, UnityLuaScriptLoaderLoadMode mode)
        {
            _sourcePath = Path.GetFullPath(sourcePath);
            _destinationPath = destinationPath;
            _outputMode = mode;
            _pendingFolders = new Queue<string>();
            _pendingFolders.Enqueue(_sourcePath);
        }

        private Script GetScriptingHostForCompile()
        {
            _compilerScript = _compilerScript ?? new Script {Options = {ScriptLoader = new FileSystemScriptLoader()}};
            return _compilerScript;
        }

        private string GetExtension()
        {
            switch (_outputMode)
            {
                case UnityLuaScriptLoaderLoadMode.LoadTextData:
                    return TextAssetExtension;
                case UnityLuaScriptLoaderLoadMode.LoadBinaryData:
                    return BinaryAssetExtension;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void CopySourcesToDestination(Action<LuaSourceCodeFile> action = null)
        {
            while (_pendingFolders.Any())
            {
                var nextSourceFolder = _pendingFolders.Dequeue();
                var directoryInfo = new DirectoryInfo(nextSourceFolder);

                var subdirectories = directoryInfo.EnumerateDirectories();
                foreach (var sub in subdirectories)
                {
                    _pendingFolders.Enqueue(sub.FullName);
                }

                var files = directoryInfo.EnumerateFiles();
                foreach (var file in files)
                {
                    if (file.Extension.ToLowerInvariant() == MetaFileExtension) continue;
                    var output = Path.Combine(_destinationPath, file.FullName.Substring(_sourcePath.Length + 1));

                    // Set asset type
                    output += GetExtension();

                    var fileRef = new LuaSourceCodeFile()
                    {
                        SourceFile = file.FullName,
                        DestinationFile = output,
                        DestinationPath = Path.GetDirectoryName(output),
                    };

                    // Does the folder exist?
                    fileRef.DestinationPathExists = Directory.Exists(fileRef.DestinationPath);

                    // Check if the file has changed
                    fileRef.DestinationFileExists = fileRef.DestinationPathExists && File.Exists(fileRef.DestinationFile);
                    fileRef.DestinationFileChanged = fileRef.DestinationFileExists &&
                                                     File.GetLastWriteTime(fileRef.SourceFile) > File.GetLastWriteTime(fileRef.DestinationFile);

                    action = action ?? ProcessSingleFile;
                    action(fileRef);
                }
            }
        }

        private void ProcessSingleFile(LuaSourceCodeFile fileRef)
        {
            if (!fileRef.DestinationPathExists)
            {
                Directory.CreateDirectory(fileRef.DestinationPath);
            }

            if (!fileRef.DestinationFileChanged && fileRef.DestinationFileExists) return;
            if (fileRef.DestinationFileExists)
            {
                File.Delete(fileRef.DestinationFile);
            }

            switch (_outputMode)
            {
                case UnityLuaScriptLoaderLoadMode.LoadTextData:
                    CopySourceFiles(fileRef);
                    break;
                case UnityLuaScriptLoaderLoadMode.LoadBinaryData:
                    CompileScriptAndSaveBinaryChunks(fileRef);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CopySourceFiles(LuaSourceCodeFile fileRef)
        {
            File.Copy(fileRef.SourceFile, fileRef.DestinationFile);
            Debug.Log($"Copy: {fileRef.SourceFile} -> {fileRef.DestinationFile}");
        }

        private void CompileScriptAndSaveBinaryChunks(LuaSourceCodeFile fileRef)
        {
            var workerScript = GetScriptingHostForCompile();
            var chunk = workerScript.LoadFile(fileRef.SourceFile);

            using (Stream stream = new FileStream(fileRef.DestinationFile, FileMode.Create, FileAccess.Write))
            {
                workerScript.Dump(chunk, stream);
            }

            Debug.Log($"Compile: {fileRef.SourceFile} -> {fileRef.DestinationFile}");
        }
    }
}