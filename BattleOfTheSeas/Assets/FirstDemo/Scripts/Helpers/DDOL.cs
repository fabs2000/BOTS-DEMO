using UnityEngine;

public class DDOL : MonoBehaviour
{
    //Script so objects under GObj hierarchy can persist across levels
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
