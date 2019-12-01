using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Runtime
{
    public class LuaSourceCodeUtility
    {
        private const string MetaFileExtension = ".meta";

        private readonly string _sourcePath;
        private readonly string _destinationPath;
        private readonly string _outputExt;
        private readonly Queue<string> _pendingFolders;

        public LuaSourceCodeUtility(string sourcePath, string destinationPath, string outputExt)
        {
            _sourcePath = Path.GetFullPath(sourcePath);
            _destinationPath = destinationPath;
            _outputExt = outputExt;
            _pendingFolders = new Queue<string>();
            _pendingFolders.Enqueue(_sourcePath);
        }

        public void CopySourcesToDestination(Action<LuaSourceCodeFile> action)
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
                    output += _outputExt;
                    
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

                    action(fileRef);
                }
            }
        }
    }
}