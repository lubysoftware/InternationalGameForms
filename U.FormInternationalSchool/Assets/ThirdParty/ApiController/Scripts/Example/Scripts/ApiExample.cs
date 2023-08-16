using UnityEngine;

namespace API.Example
{
    public class ApiExample : MonoBehaviour
    {
        private void Start()
        {
            APIFactory.GetApi<FetchApiController>().Get(print, error => Debug.LogError(error.message));
        }
    }
}