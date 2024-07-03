using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public static class MaterialHelper
    {
        private static readonly Material DefaultMaterial;
        private static readonly int Metallic = Shader.PropertyToID("_Metallic");
        private static readonly int Smoothness = Shader.PropertyToID("_Smoothness");
        
        private static readonly Dictionary<Color, Material> Materials = new();

        static MaterialHelper()
        {
            DefaultMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            DefaultMaterial.SetFloat(Metallic, 0.2f);
            DefaultMaterial.SetFloat(Smoothness, 0.2f);
        }

        public static Material GetDefaultMaterial() => GetMaterial(Color.white);
        
        public static Material GetMaterial(Color color)
        {
            if (Materials.TryGetValue(color, out var material))
                return material;
            
            // Duplicate the default material and set the color
            material = new Material(DefaultMaterial)
            {
                color = color
            };
            
            Materials.Add(color, material);
            return material;
        }
    }
}