using System;
using System.Diagnostics;

namespace XenoCore.Engine
{

    public static class Debug
    {
        public static void Assert(bool condition, String errorMsg)
        {
            if (!condition)
                throw new Exception($"Assertion failed! {errorMsg}");
        }

        [Conditional("DEBUG")]
        public static void AssertDebug(bool condition, String errorMsg)
        {
            if (!condition)
                throw new Exception($"Assertion failed! {errorMsg}");
        }
    }
}
