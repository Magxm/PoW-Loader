using System;
using System.IO;
using System.Reflection;

namespace PoW_EnglishPatch
{
    class DevHelper
    {
        public static void AllMethodsToFile(string path, Type type)
        {
            string data = "";
            MethodInfo[] methodInfos = type.GetMethods();
            foreach (MethodInfo mi in methodInfos)
            {
                var parameters = mi.GetParameters();
                data += mi.Name + "( ";
                foreach (var p in parameters)
                {
                    data += p.ParameterType.Name;
                    if (p != parameters[parameters.Length - 1])
                    {
                        data += ",";
                    }
                }
                data += " ) \n";
            }

            File.WriteAllText(path, data);
        }
    }
}
