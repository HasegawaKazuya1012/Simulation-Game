using UnityEngine;
using UnityEngine.SceneManagement; // シーン切り替えに必要

public class TitleManager : MonoBehaviour
{
    // ボタンが押されたら呼ばれる
    public void OnStartButtonClicked()
    {
        // ゲーム画面（SampleScene）へ移動する
        // ※あなたのゲーム画面のシーン名が違う場合はここを変えてください
        SceneManager.LoadScene("SampleScene"); 
    }
}