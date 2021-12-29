using System;

namespace PoW_ModAPI.API
{
    public class Utility
    {
        public static T GetPrivateVariableValue<T>(object obj, Type type, string fieldName)
        {
            var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (T)field.GetValue(obj);
        }

        public static void SetPrivateVariableValue<T>(object obj, Type type, string fieldName, T newValue)
        {
            var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(obj, newValue);
        }
    }
}
