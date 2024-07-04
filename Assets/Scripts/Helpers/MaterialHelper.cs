using System.Collections.Generic;
using Hex;
using UnityEngine;

namespace Helpers
{
    public static class MaterialHelper
    {
        private static readonly Material DefaultMaterial;
        
        private static readonly Dictionary<Color, Material> Materials = new();
        private static readonly int ResourceColor = Shader.PropertyToID("_ResourceColor");

        static MaterialHelper()
        {
            DefaultMaterial = Resources.Load<Material>("Materials/m_Tile");
        }

        public static Material GetDefaultMaterial() => GetMaterial(null);
        
        public static Material GetMaterial(CellType? type)
        {
            if (type == null)
            {
                var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"))
                {
                    color = Color.white
                };
                return mat;
            }
            
            var color = HexHelper.GetColor(type.Value);
            
            if (Materials.TryGetValue(color, out var material))
                return material;

            if (type == CellType.Water)
            {
                var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"))
                {
                    color = color
                };
                Materials.Add(color, mat);
                return mat;
            }
            
            // Duplicate the default material and set the color
            material = new Material(DefaultMaterial);
            material.SetColor(ResourceColor, color);
            
            Materials.Add(color, material);
            return material;
        }
    }
}