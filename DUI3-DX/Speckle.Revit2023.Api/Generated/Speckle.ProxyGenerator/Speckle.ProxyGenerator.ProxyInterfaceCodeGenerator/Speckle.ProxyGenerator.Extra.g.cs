﻿//----------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by https://github.com/specklesystems/ProxyGenerator
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//----------------------------------------------------------------------------------------

#nullable enable
using System;

namespace Speckle.ProxyGenerator
{
    [AttributeUsage(AttributeTargets.Interface)]
    internal sealed class ProxyAttribute : Attribute
    {
        public Type Type { get; }
        public bool ProxyBaseClasses { get; }
        public ProxyClassAccessibility Accessibility { get; }
        public string[]? MembersToIgnore { get; }

        public ProxyAttribute(Type type) : this(type, false, ProxyClassAccessibility.Public)
        {
        }

        public ProxyAttribute(Type type, bool proxyBaseClasses) : this(type, proxyBaseClasses, ProxyClassAccessibility.Public)
        {
        }

       	public ProxyAttribute(Type type, ProxyClassAccessibility accessibility) : this(type, false, accessibility)
        {
        }

        public ProxyAttribute(Type type, bool proxyBaseClasses, ProxyClassAccessibility accessibility) : this(type, proxyBaseClasses, accessibility, null)
        {
        }

        public ProxyAttribute(Type type, string[]? membersToIgnore) : this(type, false, ProxyClassAccessibility.Public, null)
        {
        }

        public ProxyAttribute(Type type, bool proxyBaseClasses, ProxyClassAccessibility accessibility, string[]? membersToIgnore)
        {
            Type = type;
            ProxyBaseClasses = proxyBaseClasses;
            Accessibility = accessibility;
            MembersToIgnore = membersToIgnore;
        }
    }

    [Flags]
    internal enum ProxyClassAccessibility
    {
        Public = 0,

        Internal = 1
    }
#nullable restore
}