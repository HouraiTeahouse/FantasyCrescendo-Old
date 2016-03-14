using System;

namespace HouraiTeahouse {
    [AttributeUsage(AttributeTargets.Class)]
    public class RequiredCharacterComponentAttribute : Attribute {
        public RequiredCharacterComponentAttribute(bool runtime = false) {
            Runtime = runtime;
        }

        public bool Runtime { get; private set; }
    }
}