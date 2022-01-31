using System;
using System.Runtime.InteropServices;
using AOT;
using NUnit.Framework;
using Unity.Burst;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture, BurstCompile]
public class FunctionPointerTests
{
    [BurstCompile(new[] { BurstCompilerOptions.DoNotEagerCompile }, CompileSynchronously = true)]
    private static T StaticFunctionNoArgsGenericReturnType<T>()
    {
        return default;
    }

    private delegate int DelegateNoArgsIntReturnType();

    [Test]
    public void TestCompileFunctionPointerNoArgsGenericReturnType()
    {
        Assert.Throws<InvalidOperationException>(
            () => BurstCompiler.CompileFunctionPointer<DelegateNoArgsIntReturnType>(StaticFunctionNoArgsGenericReturnType<int>),
            "The method `Int32 StaticFunctionNoArgsGenericReturnType[Int32]()` must be a non-generic method");
    }

#if UNITY_2019_4_OR_NEWER
    private unsafe delegate void ExceptionDelegate(int* a);

    [BurstCompile(CompileSynchronously = true)]
    [MonoPInvokeCallback(typeof(ExceptionDelegate))]
    private static unsafe void DereferenceNull(int* a)
    {
        *a = 42;
    }

    [Test]
    [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
    [Description("Requires ENABLE_UNITY_COLLECTIONS_CHECKS which is currently only enabled in the Editor")]
    public unsafe void TestDereferenceNull()
    {
        var funcPtr = BurstCompiler.CompileFunctionPointer<ExceptionDelegate>(DereferenceNull);
        Assert.Throws<InvalidOperationException>(
            () => funcPtr.Invoke(null),
            "NullReferenceException: Object reference not set to an instance of an object");
    }

    [BurstCompile(CompileSynchronously = true)]
    [MonoPInvokeCallback(typeof(ExceptionDelegate))]
    private static unsafe void DivideByZero(int* a)
    {
        *a = 42 / *a;
    }

    [Test]
    [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
    [Description("Requires ENABLE_UNITY_COLLECTIONS_CHECKS which is currently only enabled in the Editor")]
    public unsafe void TestDivideByZero()
    {
        var funcPtr = BurstCompiler.CompileFunctionPointer<ExceptionDelegate>(DivideByZero);
        var i = stackalloc int[1];
        *i = 0;
        Assert.Throws<InvalidOperationException>(
            () => funcPtr.Invoke(i),
            "DivideByZeroException: Attempted to divide by zero");
    }

    private unsafe delegate void ParentDelegate(IntPtr ptr, int* a);

    [BurstCompile(CompileSynchronously = true)]
    [MonoPInvokeCallback(typeof(ParentDelegate))]
    private static unsafe void Parent(IntPtr ptr, int* a)
    {
        new FunctionPointer<ExceptionDelegate>(ptr).Invoke(a);

        // Set a to null which should never be hit because the above will throw.
        a = null;
    }

    [Test]
    [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
    [Description("Requires ENABLE_UNITY_COLLECTIONS_CHECKS which is currently only enabled in the Editor")]
    public unsafe void TestSubFunctionPointerFails()
    {
        var parentFuncPtr = BurstCompiler.CompileFunctionPointer<ParentDelegate>(Parent);
        var funcPtr = BurstCompiler.CompileFunctionPointer<ExceptionDelegate>(DivideByZero);
        var i = stackalloc int[1];
        *i = 0;
        Assert.Throws<InvalidOperationException>(
            () => parentFuncPtr.Invoke(funcPtr.Value, i),
            "DivideByZeroException: Attempted to divide by zero");
        Assert.AreNotEqual((IntPtr)i, null);
    }

    [Test]
    [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
    [Description("Requires ENABLE_UNITY_COLLECTIONS_CHECKS which is currently only enabled in the Editor")]
    public unsafe void TestSubFunctionPointerIsNullFails()
    {
        var parentFuncPtr = BurstCompiler.CompileFunctionPointer<ParentDelegate>(Parent);
        var funcPtr = new FunctionPointer<ExceptionDelegate>((IntPtr)0);
        var i = stackalloc int[1];
        *i = 0;
        Assert.Throws<InvalidOperationException>(
            () => parentFuncPtr.Invoke(funcPtr.Value, i),
            "DivideByZeroException: Attempted to divide by zero");
        Assert.AreNotEqual((IntPtr)i, null);
    }

    private static unsafe void ManagedDivideByZero(int* a)
    {
        *a = 42 / *a;
    }

    [Test]
    [UnityPlatform(RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor)]
    [Description("Requires ENABLE_UNITY_COLLECTIONS_CHECKS which is currently only enabled in the Editor")]
    public unsafe void TestManagedSubFunctionPointerFails()
    {
        var parentFuncPtr = BurstCompiler.CompileFunctionPointer<ParentDelegate>(Parent);
        ExceptionDelegate del = ManagedDivideByZero;
        var funcPtr = new FunctionPointer<ExceptionDelegate>(Marshal.GetFunctionPointerForDelegate(del));
        var i = stackalloc int[1];
        *i = 0;
        Assert.Throws<DivideByZeroException>(
            () => parentFuncPtr.Invoke(funcPtr.Value, i),
            "Attempted to divide by zero");
        Assert.AreNotEqual((IntPtr)i, null);
    }
#endif

    [Test]
    public void TestDelegateWithCustomAttributeThatIsNotUnmanagedFunctionPointerAttribute()
    {
        var fp = BurstCompiler.CompileFunctionPointer<TestDelegateWithCustomAttributeThatIsNotUnmanagedFunctionPointerAttributeDelegate>(TestDelegateWithCustomAttributeThatIsNotUnmanagedFunctionPointerAttributeHelper);

        var result = fp.Invoke(42);

        Assert.AreEqual(43, result);
    }

    [BurstCompile(CompileSynchronously = true)]
    private static int TestDelegateWithCustomAttributeThatIsNotUnmanagedFunctionPointerAttributeHelper(int x) => x + 1;

    [MyCustomAttribute("Foo")]
    private delegate int TestDelegateWithCustomAttributeThatIsNotUnmanagedFunctionPointerAttributeDelegate(int x);

    private sealed class MyCustomAttributeAttribute : Attribute
    {
        public MyCustomAttributeAttribute(string param) { }
    }
}
