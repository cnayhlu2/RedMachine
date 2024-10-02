using UnityEngine;

namespace Assets.Scripts.Camera
{
    [CreateAssetMenu(menuName ="RedMachine/Create camera config", fileName ="CameraConfig")]
    public class CameraMoveConfig : ScriptableObject
    {
        public float MoveSpeed = 5f;
        public float StoppingDistance = .1f;
        
    }
}