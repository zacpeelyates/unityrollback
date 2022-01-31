# C#/.NET Language Support

Burst is working on a subset of .NET that doesn't allow the usage of any managed objects/reference types in your code (class in C#).

The following sections gives more details about the constructs actually supported by Burst.

# Supported .NET types

## Primitive types

Burst supports the following primitive types:

- `bool`
- `sbyte`/`byte`
- `short`/`ushort`
- `int`/`uint`
- `long`/`ulong`
- `float`
- `double`

Burst does not support the following types:

- `char` (this will be supported in a future release)
- `string` as this is a managed type
- `decimal`

## Vector types

Burst is able to translate vector types from `Unity.Mathematics` to native SIMD vector types with first class support for optimizations:

- `bool2`/`bool3`/`bool4`
- `uint2`/`uint3`/`uint4`
- `int2`/`int3`/`int4`
- `float2`/`float3`/`float4`

> Note that for performance reasons, the 4 wide types (`float4`, `int4`...) should be preferred

## Enum types

Burst supports all enums including enums with a specific storage type (e.g `public enum MyEnum : short`)

> Burst doesn't currently support `Enum` methods (e.g `Enum.HasFlag`)

## Struct types

Burst supports regular structs with any field with supported types.

Burst supports fixed array fields.

Regarding the layout, `LayoutKind.Sequential` and `LayoutKind.Explicit` are both supported

> The `StructLayout.Pack` packing size is not supported

The `System.IntPtr` and `UIntPtr` are supported natively as an intrinsic struct directly representing pointers.

## Pointer types

Burst supports any pointer types to any Burst supported types

## Generic types

Burst supports generic types used with structs. 
Specifically, it supports full instantiation of generic calls for generic types with interface constraints (e.g when a struct with a generic parameter requiring to implement an interface)

> Note that there are restrictions when using [Generic Jobs](OptimizationGuidelines.md#generic-jobs).

## Array types

Managed arrays are not supported by Burst. You should use instead a native container, `NativeArray<T>` for instance.

Burst supports reading from readonly managed arrays loaded only from static readonly fields:

```c#
[BurstCompile]
public struct MyJob : IJob {
    private static readonly int[] _preComputeTable = new int[] { 1, 2, 3, 4 };

    public int Index { get; set; }

    public void Execute()
    {
        int x = _preComputeTable[0];
        int z = _preComputeTable[Index];
    }
}
```

Accessing a static readonly managed array has with the following restrictions:

- You are not allowed to pass this static managed array around (e.g method argument), you have to use it directly.
- Elements of readonly static managed arrays should not be modified by a C# code external to jobs, as the Burst compiler makes a readonly copy of the data at compilation time.
- Array of structs are also supported on the condition that the struct constructor doesn't have any control flow (e.g `if`/`else`) and/or does not throw an exception.
- You cannot assign to static readonly array fields more than once in a static constructor.
- You cannot use explicitly laid out structs in static readonly array types.
- You cannot use multi-dimensional arrays, these are unsupported by Burst.

Burst will produce the error `BC1361` for any of these static constructors that we cannot support.

## ValueTuple type

Burst supports the `ValueTuple` type for any use where the type cannot enter or exit an entry-point boundary. Developers can use it from code that Burst compiles both the call site and calling function:

```c#
(int, float) ReturnAGreatTuple() => (42, 13.0f);

int DoSomething() => ReturnAGreatTuple().Item1;
```

The `ValueTuple` type has these restrictions because even though it is an unmanaged type, it has a `LayoutKind.Auto` layout, and so the actual in-memory layout of the struct is not something that can be consistently relied upon across a managed boundary.
