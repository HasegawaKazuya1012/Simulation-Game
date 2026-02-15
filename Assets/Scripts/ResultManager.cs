using UnityEngine;
using UnityEngine.SceneManagement; // シーン切り替えに必須

public class ResultManager : MonoBehaviour
{
    // タイトルへ戻るボタンが押されたら動く
    public void OnBackToTitleClicked()
    {
        // 【重要】もしゲームオーバー時に時間を止めていたら、ここで解除する
        Time.timeScale = 1.0f;

        // "Title" という名前のシーンを読み込む
        // ※作成したタイトル画面のシーン名と完全に同じにしてください
        SceneManager.LoadScene("Title");
    }
}