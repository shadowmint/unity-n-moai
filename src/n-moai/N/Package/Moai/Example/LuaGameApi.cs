using MoonSharp.Interpreter;
using UnityEngine;

namespace N.Package.Moai.Example
{
    public class LuaGameApi
    {
        private GameObject _gameObject;

        [MoonSharpHidden]
        public static LuaGameApi Initialize(Script runtime)
        {
            UserData.RegisterType<LuaGameApi>();
            var api = new LuaGameApi();
            var apiRef = UserData.Create(api);
            runtime.Globals.Set("api", apiRef);
            return api;
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

        // ReSharper disable once InconsistentNaming
        public float getDelta()
        {
            return Time.deltaTime;
        }

        // ReSharper disable once InconsistentNaming
        public void rotate(DynValue amount)
        {
            if (amount.Type == DataType.Number)
            {
                var rotationAmount = (float) amount.Number;
                _gameObject.transform.RotateAround(_gameObject.transform.position, Vector3.up, rotationAmount);
            }
        }

        [MoonSharpHidden]
        public void SetGameObject(GameObject gameObject)
        {
            _gameObject = gameObject;
        }
    }
}