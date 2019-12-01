using MoonSharp.Interpreter;
using UnityEngine;

namespace Runtime
{
    public class LuaGame : MonoBehaviour
    {
        private Script _runtime;

        public void Start()
        {
            // Folder name is relative to the `Resources` folder.
            _runtime = new Script {Options = {ScriptLoader = new UnityLuaScriptLoader("src", UnityLuaScriptLoaderLoadMode.LoadBinaryData)}};
            LuaGameApi.Initialize(_runtime);

            var script = _runtime.LoadFile("main/MainSetup.lua");
            script.Function.Call();
        }

        public void Update()
        {
        }
    }
}