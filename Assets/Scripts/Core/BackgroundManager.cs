using UnityEngine;
using UnityEngine.UI;
// === 修改區：引入 Addressables 所需命名空間 ===
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using DG.Tweening;
// === 修改區結束 ===
public class BackgroundManager : MonoBehaviour
{
    public Image backgroundImage;
    public BackgroundDatabase database;

    public float fadeDuration = 0.5f;

    // === 修改區：管理目前資源與動畫 ===
    private AsyncOperationHandle<Sprite>? currentHandle;
    private Tween currentTween;
    // === 修改區結束 ===

    public void ChangeBackground(string key)
    {
        // 若已有淡入淡出動畫，先停止
        currentTween?.Kill();

        // 啟動背景切換流程（非同步）
        LoadAndFade(key);
    }

    /// <summary>
    /// 載入新背景並透過 DOTween 控制淡出與淡入
    /// </summary>
    private async void LoadAndFade(string key)
    {
        // Step 1：淡出目前背景（透明度漸變為 0）
        currentTween = backgroundImage.DOFade(0f, fadeDuration);
        await currentTween.AsyncWaitForCompletion(); // 等待動畫完成

        // Step 2：釋放舊的背景圖資源（若有）
        if (currentHandle.HasValue)
            Addressables.Release(currentHandle.Value);

        // Step 3：載入新的背景圖
        var handle = Addressables.LoadAssetAsync<Sprite>(key);
        currentHandle = handle;
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            backgroundImage.sprite = handle.Result;
        }
        else
        {
            Debug.LogError("背景圖載入失敗：" + key);
            return; // 若載入失敗，中止流程
        }

        // Step 4：淡入新背景（透明度漸變為 1）
        currentTween = backgroundImage.DOFade(1f, fadeDuration);
    }
}
