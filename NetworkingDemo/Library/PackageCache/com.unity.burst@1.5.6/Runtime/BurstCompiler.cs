using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
#if !UNITY_DOTSPLAYER && !NET_DOTS
using UnityEngine.Scripting;
using System.Linq;
#endif
using System.Text;

namespace Unity.Burst
{
    /// <summary>
    /// The burst compiler runtime frontend.
    /// </summary>
    ///
    public static class BurstCompiler
    {
        /// <summary>
        /// Check if the LoadAdditionalLibrary API is supported by the current version of Unity
        /// </summary>
        /// <returns>True if the LoadAdditionalLibrary API can be used by the current version of Unity</returns>
        public static bool IsLoadAdditionalLibrarySupported()
        {
            return IsApiAvailable("LoadBurstLibrary");
        }

#if !UNITY_DOTSPLAYER && !NET_DOTS
#if UNITY_EDITOR
        static unsafe BurstCompiler()
        {
            // Store pointers to Log and Compile callback methods.
            // For more info about why we need to do this, see comments in CallbackStubManager.
            string GetFunctionPointer<TDelegate>(TDelegate callback)
            {
                GCHandle.Alloc(callback); // Ensure delegate is never garbage-collected.
                var callbackFunctionPointer = Marshal.GetFunctionPointerForDelegate(callback);
                return "0x" + callbackFunctionPointer.ToInt64().ToString("X16");
            }

            EagerCompileCompileCallbackFunctionPointer = GetFunctionPointer<CompileCallbackDelegate>(EagerCompileCompileCallback);
            EagerCompileLogCallbackFunctionPointer = GetFunctionPointer<LogCallbackDelegate>(EagerCompileLogCallback);
#if UNITY_2020_1_OR_NEWER
            ProgressCallbackFunctionPointer = GetFunctionPointer<ProgressCallbackDelegate>(ProgressCallback);
            ProfileBeginCallbackFunctionPointer = GetFunctionPointer<ProfileBeginCallbackDelegate>(ProfileBeginCallback);
            ProfileEndCallbackFunctionPointer = GetFunctionPointer<ProfileEndCallbackDelegate>(ProfileEndCallback);
#endif
        }
#endif

#if BURST_INTERNAL
        [ThreadStatic]
        public static Func<object, IntPtr> InternalCompiler;
#endif

        /// <summary>
        /// Internal variable setup by BurstCompilerOptions.
        /// </summary>
#if BURST_INTERNAL

        [ThreadStatic] // As we are changing this boolean via BurstCompilerOptions in btests and we are running multithread tests
                       // we would change a global and it would generate random errors, so specifically for btests, we are using a TLS.
        public
#else
        internal
#endif
        static bool _IsEnabled;

        /// <summary>
        /// Gets a value indicating whether Burst is enabled.
        /// </summary>
#if UNITY_EDITOR || BURST_INTERNAL
        public static bool IsEnabled => _IsEnabled;
#else
        public static bool IsEnabled => _IsEnabled && BurstCompilerHelper.IsBurstGenerated;
#endif

        /// <summary>
        /// Gets the global options for the burst compiler.
        /// </summary>
        public static readonly BurstCompilerOptions Options = new BurstCompilerOptions(true);

#if UNITY_2019_3_OR_NEWER
        /// <summary>
        /// Sets the execution mode for all jobs spawned from now on.
        /// </summary>
        /// <param name="mode">Specifiy the required execution mode</param>
        public static void SetExecutionMode(BurstExecutionEnvironment mode)
        {
            Burst.LowLevel.BurstCompilerService.SetCurrentExecutionMode((uint)mode);
        }
        /// <summary>
        /// Retrieve the current execution mode that is configured.
        /// </summary>
        /// <returns>Currently configured execution mode</returns>
        public static BurstExecutionEnvironment GetExecutionMode()
        {
            return (BurstExecutionEnvironment)Burst.LowLevel.BurstCompilerService.GetCurrentExecutionMode();
        }
#endif

        /// <summary>
        /// Compile the following delegate with burst and return a new delegate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="delegateMethod"></param>
        /// <returns></returns>
        /// <remarks>NOT AVAILABLE, unsafe to use</remarks>
        internal static unsafe T CompileDelegate<T>(T delegateMethod) where T : class
        {
            // We have added support for runtime CompileDelegate in 2018.2+
            void* function = Compile(delegateMethod, false);
            object res = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer((IntPtr)function, delegateMethod.GetType());
            return (T)res;
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private static void VerifyDelegateIsNotMulticast<T>(T delegateMethod) where T : class
        {
            var delegateKind = delegateMethod as Delegate;
            if (delegateKind.GetInvocationList().Length > 1)
            {
                throw new InvalidOperationException($"Burst does not support multicast delegates, please use a regular delegate for `{delegateMethod}'");
            }
        }

        private static bool IsMatchingMethod(MethodInfo burstMethod, MethodInfo otherMethod)
        {
            if (burstMethod.ReturnType != otherMethod.ReturnType)
            {
                return false;
            }

            var burstParameters = burstMethod.GetParameters();
            var otherParameters = otherMethod.GetParameters();

            if (burstParameters.Length != otherParameters.Length)
            {
                return false;
            }

            for (var i = 0; i < burstParameters.Length; i++)
            {
                if (burstParameters[i].ParameterType != otherParameters[i].ParameterType)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compiles a static method from a runtime method handle.
        /// </summary>
        /// <param name="handle">A runtime method handle.</param>
        /// <returns>A raw pointer to the compiled method, or null if burst is disabled.</returns>
        public static unsafe void* CompileUnsafeStaticMethod(RuntimeMethodHandle handle)
        {
            if (!IsEnabled)
            {
                return null;
            }

            if (handle.Value == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            var burstMethod = (MethodInfo)MethodBase.GetMethodFromHandle(handle);

            MethodInfo managedMethod = null;

            foreach (var method in burstMethod.DeclaringType.GetMethods())
            {
                // Only check methods with the correct name.
                if (method.Name != (burstMethod.Name + "$BurstManaged"))
                {
                    continue;
                }

                if (IsMatchingMethod(burstMethod, method))
                {
                    managedMethod = method;
                    break;
                }
            }

            if (managedMethod == null)
            {
                throw new NullReferenceException($"Could not find matching method for '{burstMethod}'");
            }

            Type delegateType = null;

            foreach (var nestedType in burstMethod.DeclaringType.GetNestedTypes())
            {
                // We are looking for a signature like $"{burstMethod.Name}_(8 hex digits)$$PostfixBurstDelegate".
                // It is enough to just check the prefix and the suffix are there, because we check the signature
                // just after anyway.
                if (!nestedType.Name.StartsWith(burstMethod.Name + "_"))
                {
                    continue;
                }

                if (!nestedType.Name.EndsWith("$PostfixBurstDelegate"))
                {
                    continue;
                }

                // Check that the signature of the invoke matches the signature of our method.
                if (IsMatchingMethod(burstMethod, nestedType.GetMethod("Invoke")))
                {
                    delegateType = nestedType;
                    break;
                }
            }

            if (delegateType == null)
            {
                throw new NullReferenceException($"Could not find the delegate type '{delegateType}'");
            }

            var managedFallbackDelegate = Delegate.CreateDelegate(delegateType, managedMethod);

            try
            {
                return Compile(new FakeDelegate(burstMethod), burstMethod, isFunctionPointer: true, managedFallbackDelegateObj: managedFallbackDelegate);
            }
            catch (UnityEngine.UnityException exception)
            {
                if (exception.Message.Contains("CompileAsyncDelegateMethod can only be called from the main thread"))
                {
                    GCHandle.Alloc(managedFallbackDelegate);
                    return (void*)Marshal.GetFunctionPointerForDelegate(managedFallbackDelegate);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Compile the following delegate into a function pointer with burst, invokable from a Burst Job or from regular C#.
        /// </summary>
        /// <typeparam name="T">Type of the delegate of the function pointer</typeparam>
        /// <param name="delegateMethod">The delegate to compile</param>
        /// <returns>A function pointer invokable from a Burst Job or from regular C#</returns>
        public static unsafe FunctionPointer<T> CompileFunctionPointer<T>(T delegateMethod) where T : class
        {
            VerifyDelegateIsNotMulticast<T>(delegateMethod);
            // We have added support for runtime CompileDelegate in 2018.2+
            void* function = Compile(delegateMethod, true);
            return new FunctionPointer<T>(new IntPtr(function));
        }

        [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
        internal class StaticTypeReinitAttribute : Attribute
        {
            public readonly Type reinitType;

            public StaticTypeReinitAttribute(Type toReinit)
            {
                reinitType = toReinit;
            }
        }

        private static unsafe void* Compile(object delegateObj, bool isFunctionPointer)
        {
            if (!(delegateObj is Delegate)) throw new ArgumentException("object instance must be a System.Delegate", nameof(delegateObj));
            var delegateMethod = (Delegate)delegateObj;
            return Compile(delegateMethod, delegateMethod.Method, isFunctionPointer);
        }

        private static unsafe void* Compile(object delegateObj, MethodInfo methodInfo, bool isFunctionPointer, object managedFallbackDelegateObj = null)
        {
            if (delegateObj == null) throw new ArgumentNullException(nameof(delegateObj));

            if (!methodInfo.IsStatic)
            {
                throw new InvalidOperationException($"The method `{methodInfo}` must be static. Instance methods are not supported");
            }
            if (methodInfo.IsGenericMethod)
            {
                throw new InvalidOperationException($"The method `{methodInfo}` must be a non-generic method");
            }

#if ENABLE_IL2CPP
            if (isFunctionPointer &&
                methodInfo.GetCustomAttributes().All(s => s.GetType().Name != "MonoPInvokeCallbackAttribute"))
            {
                UnityEngine.Debug.Log($"The method `{methodInfo}` must have `MonoPInvokeCallback` attribute to be compatible with IL2CPP!");
            }
#endif

            void* function;

#if BURST_INTERNAL
            // Internally in Burst tests, we callback the C# method instead
            function = (void*)InternalCompiler(delegateObj);
#else
            var isILPostProcessing = managedFallbackDelegateObj != null;

            if (!isILPostProcessing)
            {
                managedFallbackDelegateObj = delegateObj;
            }

            var delegateMethod = delegateObj as Delegate;
            var managedFallbackDelegateMethod = managedFallbackDelegateObj as Delegate;

#if UNITY_EDITOR
            string defaultOptions;

            // In case Burst is disabled entirely from the command line
            if (BurstCompilerOptions.ForceDisableBurstCompilation)
            {
                GCHandle.Alloc(managedFallbackDelegateMethod);
                function = (void*)Marshal.GetFunctionPointerForDelegate(managedFallbackDelegateMethod);
                return function;
            }

            if (isFunctionPointer)
            {
                defaultOptions = "--" + BurstCompilerOptions.OptionJitIsForFunctionPointer + "\n";
                // Make sure that the delegate will never be collected
                GCHandle.Alloc(managedFallbackDelegateMethod);
                var managedFunctionPointer = Marshal.GetFunctionPointerForDelegate(managedFallbackDelegateMethod);
                defaultOptions += "--" + BurstCompilerOptions.OptionJitManagedFunctionPointer + "0x" + managedFunctionPointer.ToInt64().ToString("X16");
            }
            else
            {
                defaultOptions = "--" + BurstCompilerOptions.OptionJitEnableSynchronousCompilation;
            }

            string extraOptions;
            // The attribute is directly on the method, so we recover the underlying method here
            if (Options.TryGetOptions(methodInfo, true, out extraOptions, isForILPostProcessing: isILPostProcessing))
            {
                if (!string.IsNullOrWhiteSpace(extraOptions))
                {
                    defaultOptions += "\n" + extraOptions;
                }

                var delegateMethodId = Unity.Burst.LowLevel.BurstCompilerService.CompileAsyncDelegateMethod(delegateObj, defaultOptions);
                function = Unity.Burst.LowLevel.BurstCompilerService.GetAsyncCompiledAsyncDelegateMethod(delegateMethodId);
            }
#else
            // The attribute is directly on the method, so we recover the underlying method here
            if (BurstCompilerOptions.HasBurstCompileAttribute(methodInfo))
            {
                if (Options.EnableBurstCompilation && BurstCompilerHelper.IsBurstGenerated)
                {
                    var delegateMethodId = Unity.Burst.LowLevel.BurstCompilerService.CompileAsyncDelegateMethod(delegateObj, string.Empty);
                    function = Unity.Burst.LowLevel.BurstCompilerService.GetAsyncCompiledAsyncDelegateMethod(delegateMethodId);
                }
                else
                {
                    // Make sure that the delegate will never be collected
                    GCHandle.Alloc(managedFallbackDelegateMethod);
                    // If we are in a standalone player, and burst is disabled and we are actually
                    // trying to load a function pointer, in that case we need to support it
                    // so we are then going to use the managed function directly
                    // NOTE: When running under IL2CPP, this could lead to a `System.NotSupportedException : To marshal a managed method, please add an attribute named 'MonoPInvokeCallback' to the method definition.`
                    // so in that case, the method needs to have `MonoPInvokeCallback`
                    // but that's a requirement for IL2CPP, not an issue with burst
                    function = (void*)Marshal.GetFunctionPointerForDelegate(managedFallbackDelegateMethod);
                }
            }
#endif
            else
            {
                throw new InvalidOperationException($"Burst cannot compile the function pointer `{methodInfo}` because the `[BurstCompile]` attribute is missing");
            }
#endif
            // Should not happen but in that case, we are still trying to generated an error
            // It can be null if we are trying to compile a function in a standalone player
            // and the function was not compiled. In that case, we need to output an error
            if (function == null)
            {
                throw new InvalidOperationException($"Burst failed to compile the function pointer `{methodInfo}`");
            }

            // When burst compilation is disabled, we are still returning a valid stub function pointer (the a pointer to the managed function)
            // so that CompileFunctionPointer actually returns a delegate in all cases
            return function;
        }

        /// <summary>
        /// Lets the compiler service know we are shutting down, called by the event EditorApplication.quitting
        /// </summary>
        internal static void Shutdown()
        {
#if UNITY_EDITOR
            SendCommandToCompiler(BurstCompilerOptions.CompilerCommandShutdown);
#endif
        }

#if UNITY_EDITOR
        internal static void DomainReload()
        {
            SendCommandToCompiler(BurstCompilerOptions.CompilerCommandDomainReload);
        }

        internal static string VersionNotify(string version)
        {
            return SendCommandToCompiler(BurstCompilerOptions.CompilerCommandVersionNotification, version);
        }

        internal static void UpdateAssemblerFolders(List<string> folders)
        {
            SendCommandToCompiler(BurstCompilerOptions.CompilerCommandUpdateAssemblyFolders, $"{string.Join(";", folders)}");
        }
#endif

        /// <summary>
        /// Cancel any compilation being processed by the JIT Compiler in the background.
        /// </summary>
        internal static void Cancel()
        {
#if UNITY_EDITOR
            SendCommandToCompiler(BurstCompilerOptions.CompilerCommandCancel);
#endif
        }

        internal static void Enable()
        {
#if UNITY_EDITOR
            SendCommandToCompiler(BurstCompilerOptions.CompilerCommandEnableCompiler);
#endif
        }

        internal static void Disable()
        {
#if UNITY_EDITOR
            SendCommandToCompiler(BurstCompilerOptions.CompilerCommandDisableCompiler);
#endif
        }

        internal static void TriggerUnsafeStaticMethodRecompilation()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var reinitAttributes = asm.GetCustomAttributes().Where(
                    x => x.GetType().FullName == "Unity.Burst.BurstCompiler+StaticTypeReinitAttribute"
                    );
                foreach (var attribute in reinitAttributes)
                {
                    var ourAttribute = attribute as StaticTypeReinitAttribute;
                    var type = ourAttribute.reinitType;
                    var method = type.GetMethod("Constructor",BindingFlags.Static|BindingFlags.Public);
                    method.Invoke(null, new object[] { });
                }
            }
        }

        internal static void TriggerRecompilation()
        {
#if UNITY_EDITOR
            SendCommandToCompiler(BurstCompilerOptions.CompilerCommandTriggerRecompilation, Options.GetOptions(true));
#endif
        }

        internal static void UnloadAdditionalLibraries()
        {
            SendCommandToCompiler(BurstCompilerOptions.CompilerCommandUnloadBurstNatives);
        }

        internal static bool IsApiAvailable(string apiName)
        {
            return SendCommandToCompiler(BurstCompilerOptions.CompilerCommandIsNativeApiAvailable, apiName) == "True";
        }

        internal static void EagerCompileMethods(List<EagerCompilationRequest> requests)
        {
#if UNITY_EDITOR
            // The order of these arguments MUST match the corresponding code in JitCompilerService.EagerCompileMethods.
            const string parameterSeparator = "***";
            const string requestParametersSeparator = "+++";
            const string methodSeparator = "```";

            var builder = new StringBuilder();

            builder.Append(EagerCompileCompileCallbackFunctionPointer);
            builder.Append(parameterSeparator);
            builder.Append(EagerCompileLogCallbackFunctionPointer);
            builder.Append(parameterSeparator);

            foreach (var request in requests)
            {
                builder.Append(request.EncodedMethod);
                builder.Append(requestParametersSeparator);
                builder.Append(request.Options);
                builder.Append(methodSeparator);
            }

            builder.Append(parameterSeparator);

            SendCommandToCompiler(BurstCompilerOptions.CompilerCommandEagerCompileMethods, builder.ToString());
#endif
        }

#if UNITY_EDITOR
        private unsafe delegate void CompileCallbackDelegate(void* userdata, NativeDumpFlags dumpFlags, void* dataPtr);

        private static unsafe void EagerCompileCompileCallback(void* userData, NativeDumpFlags dumpFlags, void* dataPtr) { }

        private static readonly string EagerCompileCompileCallbackFunctionPointer;

        private unsafe delegate void LogCallbackDelegate(void* userData, int logType, byte* message, byte* fileName, int lineNumber);

        private static unsafe void EagerCompileLogCallback(void* userData, int logType, byte* message, byte* fileName, int lineNumber)
        {
            if (EagerCompilationLoggingEnabled)
            {
                BurstRuntime.Log(message, logType, fileName, lineNumber);
            }
        }

        internal static bool EagerCompilationLoggingEnabled = false;

        private static readonly string EagerCompileLogCallbackFunctionPointer;
#endif

        internal static void WaitUntilCompilationFinished()
        {
#if UNITY_EDITOR
            SendCommandToCompiler(BurstCompilerOptions.CompilerCommandWaitUntilCompilationFinished);
#endif
        }

        internal static void ClearEagerCompilationQueues()
        {
#if UNITY_EDITOR
            SendCommandToCompiler(BurstCompilerOptions.CompilerCommandClearEagerCompilationQueues);
#endif
        }

        internal static void CancelEagerCompilation()
        {
#if UNITY_EDITOR
            SendCommandToCompiler(BurstCompilerOptions.CompilerCommandCancelEagerCompilation);
#endif
        }

        internal static void SetProgressCallback()
        {
#if UNITY_EDITOR && UNITY_2020_1_OR_NEWER
            SendCommandToCompiler(BurstCompilerOptions.CompilerCommandSetProgressCallback, ProgressCallbackFunctionPointer);
#endif
        }

#if UNITY_EDITOR && UNITY_2020_1_OR_NEWER
        private delegate void ProgressCallbackDelegate(int current, int total);

        private static readonly string ProgressCallbackFunctionPointer;

        private static void ProgressCallback(int current, int total)
        {
            OnProgress?.Invoke(current, total);
        }

        internal static event Action<int, int> OnProgress;
#endif

        internal static void RequestClearJitCache()
        {
#if UNITY_EDITOR && UNITY_2020_1_OR_NEWER
            SendCommandToCompiler(BurstCompilerOptions.CompilerCommandRequestClearJitCache);
#endif
        }

        internal static void SetProfilerCallbacks()
        {
#if UNITY_EDITOR && UNITY_2020_1_OR_NEWER
            SendCommandToCompiler(
                BurstCompilerOptions.CompilerCommandSetProfileCallbacks,
                ProfileBeginCallbackFunctionPointer + ";" + ProfileEndCallbackFunctionPointer);
#endif
        }

#if UNITY_EDITOR && UNITY_2020_1_OR_NEWER
        internal delegate void ProfileBeginCallbackDelegate(string markerName, string metadataName, string metadataValue);
        internal delegate void ProfileEndCallbackDelegate(string markerName);

        private static readonly string ProfileBeginCallbackFunctionPointer;
        private static readonly string ProfileEndCallbackFunctionPointer;

        private static void ProfileBeginCallback(string markerName, string metadataName, string metadataValue) => OnProfileBegin?.Invoke(markerName, metadataName, metadataValue);
        private static void ProfileEndCallback(string markerName) => OnProfileEnd?.Invoke(markerName);

        internal static event ProfileBeginCallbackDelegate OnProfileBegin;
        internal static event ProfileEndCallbackDelegate OnProfileEnd;
#endif

        internal static void Reset()
        {
#if UNITY_EDITOR
            SendCommandToCompiler(BurstCompilerOptions.CompilerCommandReset);
#endif
        }

        private static string SendCommandToCompiler(string commandName, string commandArgs = null)
        {
            if (commandName == null) throw new ArgumentNullException(nameof(commandName));

            var compilerOptions = commandName;
            if (commandArgs != null)
            {
                compilerOptions += " " + commandArgs;
            }

            var results = Unity.Burst.LowLevel.BurstCompilerService.GetDisassembly(DummyMethodInfo, compilerOptions);
            if (!string.IsNullOrEmpty(results))
                return results.TrimStart('\n');
            return "";
        }

        private static readonly MethodInfo DummyMethodInfo = typeof(BurstCompiler).GetMethod(nameof(DummyMethod), BindingFlags.Static | BindingFlags.NonPublic);

        /// <summary>
        /// Dummy empty method for being able to send a command to the compiler
        /// </summary>
        private static void DummyMethod() { }

#if !UNITY_EDITOR && !BURST_INTERNAL
        /// <summary>
        /// Internal class to detect at standalone player time if AOT settings were enabling burst.
        /// </summary>
        [BurstCompile]
        internal static class BurstCompilerHelper
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate bool IsBurstEnabledDelegate();
            private static readonly IsBurstEnabledDelegate IsBurstEnabledImpl = new IsBurstEnabledDelegate(IsBurstEnabled);

            [BurstCompile]
            [AOT.MonoPInvokeCallback(typeof(IsBurstEnabledDelegate))]
            private static bool IsBurstEnabled()
            {
                bool result = true;
                DiscardedMethod(ref result);
                return result;
            }

            [BurstDiscard]
            private static void DiscardedMethod(ref bool value)
            {
                value = false;
            }

            private static unsafe bool IsCompiledByBurst(Delegate del)
            {
                var delegateMethodId = Unity.Burst.LowLevel.BurstCompilerService.CompileAsyncDelegateMethod(del, string.Empty);
                // We don't try to run the method, having a pointer is already enough to tell us that burst was active for AOT settings
                return Unity.Burst.LowLevel.BurstCompilerService.GetAsyncCompiledAsyncDelegateMethod(delegateMethodId) != (void*)0;
            }

            /// <summary>
            /// Gets a boolean indicating whether burst was enabled for standalone player, used only at runtime.
            /// </summary>
            public static readonly bool IsBurstGenerated = IsCompiledByBurst(IsBurstEnabledImpl);
        }
#endif // !UNITY_EDITOR && !BURST_INTERNAL

        /// <summary>
        /// Fake delegate class to make BurstCompilerService.CompileAsyncDelegateMethod happy
        /// so that it can access the underlying static method via the property get_Method.
        /// So this class is not a delegate.
        /// </summary>
        private class FakeDelegate
        {
            public FakeDelegate(MethodInfo method)
            {
                Method = method;
            }

            [Preserve]
            public MethodInfo Method { get; }
        }

#else    // UNITY_DOTSPLAYER || NET_DOTS

        /// <summary>
        /// Compile the following delegate into a function pointer with burst, invokable from a Burst Job or from regular C#.
        /// </summary>
        /// <typeparam name="T">Type of the delegate of the function pointer</typeparam>
        /// <param name="delegateMethod">The delegate to compile</param>
        /// <returns>A function pointer invokable from a Burst Job or from regular C#</returns>
        public static unsafe FunctionPointer<T> CompileFunctionPointer<T>(T delegateMethod) where T : System.Delegate
        {
            // Make sure that the delegate will never be collected
            GCHandle.Alloc(delegateMethod);
            return new FunctionPointer<T>(Marshal.GetFunctionPointerForDelegate(delegateMethod));
        }

        internal static bool IsApiAvailable(string apiName)
        {
            return false;
        }
#endif
    }
}