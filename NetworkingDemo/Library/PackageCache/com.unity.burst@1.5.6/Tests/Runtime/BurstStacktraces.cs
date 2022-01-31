using System;
using AOT;
using NUnit.Framework;
using System.Text.RegularExpressions;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.TestTools;

#if UNITY_2021_1_OR_NEWER
namespace BurstStacktraces
{
    [BurstCompile]
    class BurstStacktracesJobs
    {
        [BurstCompile(CompileSynchronously = true, Debug = true)]
        struct ThrowManagedExceptionDebugJob : IJob
        {
            public void Execute()
            {
                throw new ArgumentException("A");
            }
        }

        [BurstCompile(CompileSynchronously = true)]
        struct ThrowManagedExceptionJob : IJob
        {
            public void Execute()
            {
                throw new ArgumentException("A");
            }
        }

        [BurstCompile(CompileSynchronously = true, Debug = true)]
        struct ThrowNativeExceptionDebugJob : IJob
        {
            public int A;

            public void Execute()
            {
                // A is zero so this will fail with a divide by zero native hardware exception.
                A = 42 / A;
            }
        }

        [BurstCompile(CompileSynchronously = true)]
        struct ThrowNativeExceptionJob : IJob
        {
            public int A;

            public void Execute()
            {
                // A is zero so this will fail with a divide by zero native hardware exception.
                A = 42 / A;
            }
        }

        private delegate int ExceptionDelegate(int a);

        [BurstCompile(CompileSynchronously = true)]
        [MonoPInvokeCallback(typeof(ExceptionDelegate))]
        private static int ThrowManagedExceptionFunctionPointer(int a)
        {
            throw new ArgumentException("A");
        }

        [BurstCompile(CompileSynchronously = true)]
        [MonoPInvokeCallback(typeof(ExceptionDelegate))]
        private static int ThrowNativeExceptionFunctionPointer(int a)
        {
            // a is zero so this will fail with a divide by zero native hardware exception.
            return 42 / a;
        }

        [BurstCompile(CompileSynchronously = true, Debug = true)]
        [MonoPInvokeCallback(typeof(ExceptionDelegate))]
        private static int ThrowManagedExceptionDebugFunctionPointer(int a)
        {
            throw new ArgumentException("A");
        }

        [BurstCompile(CompileSynchronously = true, Debug = true)]
        [MonoPInvokeCallback(typeof(ExceptionDelegate))]
        private static int ThrowNativeExceptionDebugFunctionPointer(int a)
        {
            // a is zero so this will fail with a divide by zero native hardware exception.
            return 42 / a;
        }

        [Test]
        [UnityPlatform(RuntimePlatform.WindowsEditor)]
        [Description("Only Windows has built-in support for line numbers in stack traces")]
        public void ThrowManagedExceptionDebugWindowsFromJob()
        {
            BurstCompiler.Options.EnableBurstDebug = true;

            LogAssert.Expect(LogType.Exception, new Regex("\\[BurstStacktraces\\.cs:21\\]"));

            var jobData = new ThrowManagedExceptionDebugJob();
            jobData.Run();
        }

        [Test]
        [UnityPlatform(RuntimePlatform.WindowsEditor)]
        [Description("Only Windows has built-in support for line numbers in stack traces")]
        public void ThrowNativeExceptionDebugWindowsFromJob()
        {
            BurstCompiler.Options.EnableBurstDebug = true;

            LogAssert.Expect(LogType.Exception, new Regex("\\[BurstStacktraces\\.cs:42\\]"));

            var jobData = new ThrowNativeExceptionDebugJob { A = 0 };
            jobData.Run();
        }

        [Test]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        [Description("Requires stacktrace exception support which is only supported in the Editor")]
        public void ThrowManagedExceptionDebugFromJob()
        {
            BurstCompiler.Options.EnableBurstDebug = true;

            LogAssert.Expect(LogType.Exception, new Regex("BurstStacktraces\\.BurstStacktracesJobs\\.ThrowManagedExceptionDebugJob"));

            var jobData = new ThrowManagedExceptionDebugJob();
            jobData.Run();
        }

        [Test]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor)]
        [Description("Requires stacktrace exception support which is only supported in the Editor")]
        public void ThrowManagedExceptionFromJob()
        {
            BurstCompiler.Options.EnableBurstDebug = false;

            LogAssert.Expect(LogType.Exception, new Regex("Unity\\.Jobs\\.IJobExtensions\\.JobStruct`1<BurstStacktraces\\.BurstStacktracesJobs\\.ThrowManagedExceptionJob>\\.Execute"));

            var jobData = new ThrowManagedExceptionJob();
            jobData.Run();
        }

        [Test]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
        [Description("Requires stacktrace exception support which is only supported in the Editor")]
        public void ThrowNativeExceptionDebugFromJob()
        {
            BurstCompiler.Options.EnableBurstDebug = true;

            LogAssert.Expect(LogType.Exception, new Regex("BurstStacktraces\\.BurstStacktracesJobs\\.ThrowNativeExceptionDebugJob"));

            var jobData = new ThrowNativeExceptionDebugJob { A = 0 };
            jobData.Run();
        }

        [Test]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor)]
        [Description("Requires stacktrace exception support which is only supported in the Editor")]
        public void ThrowNativeExceptionFromJob()
        {
            BurstCompiler.Options.EnableBurstDebug = false;

            LogAssert.Expect(LogType.Exception, new Regex("Unity\\.Jobs\\.IJobExtensions\\.JobStruct`1<BurstStacktraces\\.BurstStacktracesJobs\\.ThrowNativeExceptionJob>\\.Execute"));

            var jobData = new ThrowNativeExceptionJob { A = 0 };
            jobData.Run();
        }

        [Test]
        [UnityPlatform(RuntimePlatform.WindowsEditor)]
        [Description("Only Windows has built-in support for line numbers in stack traces")]
        public void ThrowManagedExceptionDebugWindowsFromFunctionPointer()
        {
            BurstCompiler.Options.EnableBurstDebug = true;

            var funcPtr = BurstCompiler.CompileFunctionPointer<ExceptionDelegate>(ThrowManagedExceptionDebugFunctionPointer);

            try
            {
                funcPtr.Invoke(0);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("[BurstStacktraces.cs:79] BurstStacktraces.BurstStacktracesJobs.ThrowManagedExceptionDebugFunctionPointer"));
            }
        }

        [Test]
        [UnityPlatform(RuntimePlatform.WindowsEditor)]
        [Description("Only Windows has built-in support for line numbers in stack traces")]
        public void ThrowNativeExceptionDebugWindowsFromFunctionPointer()
        {
            BurstCompiler.Options.EnableBurstDebug = true;

            var funcPtr = BurstCompiler.CompileFunctionPointer<ExceptionDelegate>(ThrowNativeExceptionDebugFunctionPointer);

            try
            {
                funcPtr.Invoke(0);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("[BurstStacktraces.cs:87] BurstStacktraces.BurstStacktracesJobs.ThrowNativeExceptionDebugFunctionPointer"));
            }
        }

        [Test]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor)]
        [Description("Requires stacktrace exception support which is only supported in the Editor")]
        public void ThrowManagedExceptionDebugFromFunctionPointer()
        {
            BurstCompiler.Options.EnableBurstDebug = true;

            var funcPtr = BurstCompiler.CompileFunctionPointer<ExceptionDelegate>(ThrowManagedExceptionDebugFunctionPointer);

            try
            {
                funcPtr.Invoke(0);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("BurstStacktraces.BurstStacktracesJobs.ThrowManagedExceptionDebugFunctionPointer"));
            }
        }

        [Test]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor)]
        [Description("Requires stacktrace exception support which is only supported in the Editor")]
        public void ThrowManagedExceptionFromFunctionPointer()
        {
            BurstCompiler.Options.EnableBurstDebug = false;

            var funcPtr = BurstCompiler.CompileFunctionPointer<ExceptionDelegate>(ThrowManagedExceptionFunctionPointer);

            try
            {
                funcPtr.Invoke(0);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("BurstStacktraces.BurstStacktracesJobs.ThrowManagedExceptionFunctionPointer"));
            }
        }

        [Test]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor)]
        [Description("Requires stacktrace exception support which is only supported in the Editor")]
        public void ThrowNativeExceptionDebugFromFunctionPointer()
        {
            BurstCompiler.Options.EnableBurstDebug = true;

            var funcPtr = BurstCompiler.CompileFunctionPointer<ExceptionDelegate>(ThrowNativeExceptionDebugFunctionPointer);

            try
            {
                funcPtr.Invoke(0);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("BurstStacktraces.BurstStacktracesJobs.ThrowNativeExceptionDebugFunctionPointer"));
            }
        }

        [Test]
        [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor)]
        [Description("Requires stacktrace exception support which is only supported in the Editor")]
        public void ThrowNativeExceptionFromFunctionPointer()
        {
            BurstCompiler.Options.EnableBurstDebug = false;

            var funcPtr = BurstCompiler.CompileFunctionPointer<ExceptionDelegate>(ThrowNativeExceptionFunctionPointer);

            try
            {
                funcPtr.Invoke(0);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("BurstStacktraces.BurstStacktracesJobs.ThrowNativeExceptionFunctionPointer"));
            }
        }
    }
}
#endif
