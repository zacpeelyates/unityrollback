using System;
#if !UNITY_DOTSRUNTIME
using UnityEngine.Scripting.APIUpdating;
#endif

namespace Unity.Jobs
{
    /// <summary>
    /// When added as an assembly-level attribute, allows creating job reflection data for instances of generic jobs.
    /// </summary>
    /// <remarks>
    /// This attribute allows specific instances of generic jobs to be registered for reflection data generation.
    /// </remarks>
#if !UNITY_DOTSRUNTIME
    [MovedFrom(true, "Unity.Entities", "Unity.Entities")]
#endif
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterGenericJobTypeAttribute : Attribute
    {
        public Type ConcreteType;

        public RegisterGenericJobTypeAttribute(Type type)
        {
            ConcreteType = type;
        }
    }
}
