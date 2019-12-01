using System;
using MoonSharp.Interpreter;
using N.Package.Test;
using NUnit.Framework;

namespace Editor.Tests
{
    public class LuaTests : TestCase
    {
        [Test]
        public void TestRunBasicLuaCode()
        {
            var script = new Script();
            var code = @"
                -- defines a factorial function
				function fact (n)
			        if (n == 0) then
				        return 1
			        else
				        return n*fact(n - 1)
			        end
		        end
                return fact(5)  
            ";

            var rtn = script.DoString(code);
            var value = rtn.Number;
            Assert(Math.Abs(value - (5.0 * 4.0 * 3.0 * 2.0 * 1.0)) < 0.01);
        }

        [Test]
        public void TestExposedApiCanExecuteInLua()
        {
            var script = new Script();
            var code = @"
                fooApi.AddValue(10);
                return fooApi.GetValue();                
            ";

            UserData.RegisterType<ApiProxy>();

            var proxy = new ApiProxy(100);
            var api = UserData.Create(proxy);
            script.Globals.Set("fooApi", api);

            var rtn = script.DoString(code);
            var value = rtn.Number;
            
            Assert(Math.Abs(proxy.GetValue() - value) < 0.01);
            Assert(Math.Abs(value - 110.0) < 0.01);
        }
        
        [Test]
        public void TestPersistentLuaState()
        {
            var script = new Script();
            var setup = @"
                x = 1;
            ";
            var step = @"
                x = x + 1;
                return x;
            ";

            script.DoString(setup);
            
            var rtn = script.DoString(step);
            var value = (int) rtn.Number;
            
            Assert(value == 2);
            
            rtn = script.DoString(step);
            value = (int) rtn.Number;
            
            Assert(value == 3);
        }

        public class ApiProxy
        {
            private int _value;

            [MoonSharpHidden]
            public ApiProxy(int initialValue)
            {
                _value = initialValue;
            }

            public void AddValue(int value)
            {
                _value += value;
            }

            public int GetValue()
            {
                return _value;
            }
        }
    }
}