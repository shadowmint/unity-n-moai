using MoonSharp.Interpreter;
using N.Package.Moai.LuaUtility;
using UnityEngine;

namespace N.Package.Moai.Example
{
    public class LuaGame : MonoBehaviour
    {
        private Script _runtime;
        private LuaGameApi _api;
        private DynValue _update;

        public void Start()
        {
            // Get builder, and match to the builder
            var builder = FindObjectOfType<LuaGameBuild>();
            var mode = builder.mode;

            // Folder name is relative to the `Resources` folder.
            _runtime = new Script {Options = {ScriptLoader = new UnityLuaScriptLoader("src", mode)}};
            _api = LuaGameApi.Initialize(_runtime);
            _api.SetGameObject(gameObject);

            var script = _runtime.LoadFile("main/MainSetup.lua");
            script.Function.Call();

            _update = _runtime.LoadFile("main/MainLoop.lua");
            _update.Function.Call();
        }

        public void Update()
        {
            _update.Function.Call();
        }
    }
}