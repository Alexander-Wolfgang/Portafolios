using UnityEngine;

public class GameManagerSkybox : MonoBehaviour
{
    [Header("Skybox por niveles")]
    public Material skyboxNivel1a8;
    public Material skyboxNivel9a16;
    public Material skyboxNivel17a23;
    public Material skyboxBoss24;

    public void AplicarSkyboxPorNivel(int nivel)
    {
        if (nivel <= 8)
        {
            RenderSettings.skybox = skyboxNivel1a8;
        }
        else if (nivel <= 16)
        {
            RenderSettings.skybox = skyboxNivel9a16;
        }
        else if (nivel <= 23)
        {
            RenderSettings.skybox = skyboxNivel17a23;
        }
        else
        {
            RenderSettings.skybox = skyboxBoss24;
        }

        DynamicGI.UpdateEnvironment();
    }
}