using UnityEngine;
using UnityEngine.UI;

public class LoadUITest : MonoBehaviour
{
    [SerializeField] private Image _loadTestImage;
    private void OnEnable()
    {
        string path = "Assets/Alt_Resouce/IsometricDiamond.png";
        PracticeResourceManager.Inst.AddressableLoadSprite_Callback(path, (sprite) =>
        { _loadTestImage.sprite = sprite; });
    }
}
