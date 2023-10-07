using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CdnGetter.Config
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class CommandLineShorthandAttribute : Attribute
    {
        public char SwitchCharacter { get; }

        public char? AltSwitchCharacter { get; set; }
        
        public CommandLineShorthandAttribute(char switchCharacter)
        {
            SwitchCharacter = switchCharacter;
        }
    }
}