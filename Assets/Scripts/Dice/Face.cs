using UnityEngine;

namespace Dice
{
    public class Face : MonoBehaviour
    {
        [SerializeField, Range(1, 6)] private int faceValue;
        public int FaceValue => faceValue;
    }
}