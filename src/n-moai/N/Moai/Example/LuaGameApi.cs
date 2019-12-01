using MoonSharp.Interpreter;
using UnityEngine;

namespace Runtime
{
    public class LuaGameApi
    {
        [MoonSharpHidden]
        public static void Initialize(Script runtime)
        {
            UserData.RegisterType<LuaGameApi>();
            var api = UserData.Create(new LuaGameApi());
            runtime.Globals.Set("api", api);
        }

        // ReSharper disable once InconsistentNaming
        public void debug(DynValue message)
        {
            if (message.Type == DataType.String)
            {
                Debug.Log(message.String);
            }
            else
            {
                Debug.Log(message);
            }
        }
    }
}