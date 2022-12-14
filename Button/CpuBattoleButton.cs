using DG.Tweening;  
using UnityEngine;  
using UnityEngine.EventSystems;  
using UnityEngine.SceneManagement;

public class CpuBattoleButton : MonoBehaviour,  
    IPointerClickHandler,  
    IPointerDownHandler,  
    IPointerUpHandler  
{
    public System.Action onClickCallback;  

    [SerializeField] private CanvasGroup _canvasGroup;  

    public void OnPointerClick(PointerEventData eventData)  
    {
        PlayerPrefs.SetInt ("image_id", 1);
        SceneManager.LoadScene (SceneManager.GetActiveScene().name);
        Debug.Log("処理実行");
        onClickCallback?.Invoke();  
    }

    public void OnPointerDown(PointerEventData eventData)  
    {
        transform.DOScale(0.95f, 0.24f).SetEase(Ease.OutCubic);  
        _canvasGroup.DOFade(0.8f, 0.24f).SetEase(Ease.OutCubic);  
    }

    public void OnPointerUp(PointerEventData eventData)  
    {
        transform.DOScale(1f, 0.24f).SetEase(Ease.OutCubic);  
        _canvasGroup.DOFade(1f, 0.24f).SetEase(Ease.OutCubic);  
    }
}