using System.Collections.Generic;
using Hex;
using UnityEngine;

namespace Helpers
{
    public static class MaterialHelper
    {
        private static readonly Dictionary<Color, MaterialPropertyBlock> MaterialPropertyBlocks = new();
        private static readonly int ResourceColor = Shader.PropertyToID("_ResourceColor");
        private static readonly int BlendRadius = Shader.PropertyToID("_BlendRadius");
        
        public static MaterialPropertyBlock GetMaterialProperties(CellType? type)
        {
            MaterialPropertyBlock mpb;
            if (type == null)
            {
                mpb = new MaterialPropertyBlock();
                mpb.SetColor(ResourceColor, Color.white);
                return mpb;
            }
            
            var color = HexHelper.GetColor(type.Value);
            
            if (MaterialPropertyBlocks.TryGetValue(color, out mpb))
                return mpb;

            if (type == CellType.Water)
            {
                var mat = new MaterialPropertyBlock();
                mat.SetColor(ResourceColor, color);
                mat.SetFloat(BlendRadius, 0f);
                MaterialPropertyBlocks.Add(color, mat);
                return mat;
            }
            
            // Duplicate the default material and set the color
            mpb = new MaterialPropertyBlock();
            mpb.SetColor(ResourceColor, color);
            
            MaterialPropertyBlocks.Add(color, mpb);
            return mpb;
        }
    }
}