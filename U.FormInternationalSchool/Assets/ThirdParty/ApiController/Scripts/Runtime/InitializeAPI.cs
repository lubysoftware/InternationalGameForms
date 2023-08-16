using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace API
{
    public class InitializeAPI : MonoBehaviour
    {
        public DeploymentEnvironments DeployEnvironment;
        public List<DeploymentEnvScriptable> DeploymentEnvironments;

        private void Awake()
        {
            var currentApiToDeploy = GetApiToDeploy();

            if (currentApiToDeploy == null)
            {
                Debug.LogWarning(
                    $"WARNING: {DeployEnvironment} is not added on {typeof(InitializeAPI)} please change deploy or add another DeploymentEnvironmentScriptable");
                return;
            }

            APIFactory.Initialize(currentApiToDeploy.Url, new RestClientAPI(), currentApiToDeploy.IsDebug);
        }

        private DeploymentEnvScriptable GetApiToDeploy()
        {
            return DeploymentEnvironments.FirstOrDefault(f => f.DeploymentEnvironment == DeployEnvironment);
        }
    }
}
