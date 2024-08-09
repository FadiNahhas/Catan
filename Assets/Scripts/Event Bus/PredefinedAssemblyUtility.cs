using System;
using System.Collections.Generic;
using System.Reflection;

namespace Event_Bus
{
    public static class PredefinedAssemblyUtility
    {
        enum AssemblyType
        {
            AssemblyCSharp,
            AssemblyCSharpEditor,
            AssemblyCSharpEditorFirstPass,
            AssemblyCSharpFirstPass
        }

        static AssemblyType? GetAssemblyType(string assembly_name) {
            return assembly_name switch {
                "Assembly-CSharp" => AssemblyType.AssemblyCSharp,
                "Assembly-CSharp-Editor" => AssemblyType.AssemblyCSharpEditor,
                "Assembly-CSharp-Editor-firstpass" => AssemblyType.AssemblyCSharpEditorFirstPass,
                "Assembly-CSharp-firstpass" => AssemblyType.AssemblyCSharpFirstPass,
                _ => null
            };
        }

        static void AddTypesFromAssembly(Type[] assembly_types, Type interface_type, ICollection<Type> results) {
            if (assembly_types == null) return;
            for (int i = 0; i < assembly_types.Length; i++) {
                Type type = assembly_types[i];
                if (type != interface_type && interface_type.IsAssignableFrom(type)) {
                    results.Add(type);
                }
            }
        }
        
        public static List<Type> GetTypes(Type interface_type) {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
            Dictionary<AssemblyType, Type[]> assemblyTypes = new Dictionary<AssemblyType, Type[]>();
            List<Type> types = new List<Type>();
            for (int i = 0; i < assemblies.Length; i++) {
                AssemblyType? assemblyType = GetAssemblyType(assemblies[i].GetName().Name);
                if (assemblyType != null) {
                    assemblyTypes.Add((AssemblyType) assemblyType, assemblies[i].GetTypes());
                }
            }
        
            assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharp, out var assemblyCSharpTypes);
            AddTypesFromAssembly(assemblyCSharpTypes, interface_type, types);

            assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharpFirstPass, out var assemblyCSharpFirstPassTypes);
            AddTypesFromAssembly(assemblyCSharpFirstPassTypes, interface_type, types);
        
            return types;
        }
    }
}