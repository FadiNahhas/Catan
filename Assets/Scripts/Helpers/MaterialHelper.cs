using UnityEngine;

namespace Helpers
{
    public static class MaterialHelper
    {
        public static Material GetDefaultMaterial()
        {
            var material = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
            {
                color = Color.white
            };
            
            return material;
        }
        
        public static Material GetMaterial(Color color)
        {
            var material = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
            {
                color = color
            };
            
            return material;
        }
    }
}