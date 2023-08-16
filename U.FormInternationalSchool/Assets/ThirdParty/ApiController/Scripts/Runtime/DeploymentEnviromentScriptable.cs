using UnityEngine;

namespace API
{
    [CreateAssetMenu(menuName = "API/New Deployment Environment", fileName = "New API deployment")]
    public class DeploymentEnvScriptable : ScriptableObject
    {
        public DeploymentEnvironments DeploymentEnvironment;
        public string Url;
        public bool IsDebug;
    }
}