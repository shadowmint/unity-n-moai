using System;
using System.IO;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using UnityEngine;

namespace N.Package.Moai.LuaUtility
{
    public class UnityLuaScriptLoader : ScriptLoaderBase
    {
        private readonly string _rootResourcesPath;
        private readonly UnityLuaScriptLoaderLoadMode _loadMode;

        /// <summary>
        /// Setup a new load 
        /// </summary>
        /// <param name="rootResourcesPath">The root path, relative to the parent 'Resources' folder</param>
        /// <param name="loadMode">Is the asset to load the full source, or the binary representation?</param>
        public UnityLuaScriptLoader(string rootResourcesPath, UnityLuaScriptLoaderLoadMode loadMode)
        {
            _rootResourcesPath = rootResourcesPath;
            _loadMode = loadMode;
        }

        public override bool ScriptFileExists(string file)
        {
            var fullPath = Path.Combine(_rootResourcesPath, file);
            var textAsset = Resources.Load<TextAsset>(fullPath);
            return textAsset == null;
        }

        public override object LoadFile(string file, Table globalContext)
        {
            var fullPath = Path.Combine(_rootResourcesPath, file);
            var textAsset = Resources.Load(fullPath) as TextAsset;
            if (textAsset == null) throw new FileNotFoundException(fullPath);
            switch (_loadMode)
            {
                case UnityLuaScriptLoaderLoadMode.LoadTextData:
                    return textAsset.text;
                case UnityLuaScriptLoaderLoadMode.LoadBinaryData:
                    return textAsset.bytes;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}