using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum SceneName
{
    LoadScene,
    SelectScene,
    TitleScene,
    PlayScene,
    TutorialScene,
    TitleRoom,
    Tutrial1,
    Tutrial2,
    Stage1,
    Stage2,
    None,
}
public enum AddToScene
{
    ChildScene,
    TitleRoom,
}
public class SceneControll : MonoBehaviour
{
    //クラス外から次のシーン名を指定
    public SceneName NextScene { get; set; }

    public SceneName CurrentScene { get; set; }

    public List<string> AddToScene = new List<string>();

    private Dictionary<SceneName, string> MainScene = new Dictionary<SceneName, string>();

    void Start()
    {
        //Dictionaryの初期化を行う⇛Dictionary管理でタイピングミスを無くす
        MainScene.Add(SceneName.LoadScene, "LoadScene");
        MainScene.Add(SceneName.TitleScene, "TitleScene");
        MainScene.Add(SceneName.PlayScene, "PlayScene");
        MainScene.Add(SceneName.SelectScene, "SelectScene");
        MainScene.Add(SceneName.TutorialScene, "TutorialScene");

        //シーンの入れ替えをLoad等をすべてこのスクリプトで管理するため,シーンをまたいで活用
        DontDestroyOnLoad(this);

        //初期化
        CurrentScene = SceneName.LoadScene;
        //すべての設定が終わったらタイトルへ移動
        NextScene = SceneName.TitleScene;
        //SceneChange(NextScene);
    }

    void Update()
    {
        //常にシーン遷移を行うか判断
        SceneChange(NextScene);
    }
    //シーンを遷移する際に行う処理
    void SceneChange(SceneName scene)
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            //Application.Quit();
        }
        //今のシーンと次のシーンが同じでなければ次のシーンをロードする
        if ((SceneManager.GetActiveScene().name != MainScene[scene]))
        {
            SceneManager.LoadScene(MainScene[scene], LoadSceneMode.Single);
            //シーンが切り替わったら現在のシーンを更新
            CurrentScene = scene;

            Debug.Log("シーン切替時：次のシーンだお：" + scene);
        }
        if (AddToScene.Count != 0)
        {
            //新しいシーンを読み込んだ際に追加シーンを読み込む
            StartCoroutine(AddScene());
            //すべて読みこんだらAddSceneリストをクリア
            AddToScene.Clear();
        }
    }
    //指定したシーンの開放
    public void RemoveScene(SceneName scene)
    {

    }

    IEnumerator AddScene()
    {
        foreach (var addScene in AddToScene)
        {
            //シーンを非同期で追加する
            SceneManager.LoadScene(addScene.ToString(), LoadSceneMode.Additive);
        }
        //シーン名を指定する(現在のシーンをアクティブにしたい)
        Scene scene = SceneManager.GetSceneByName(CurrentScene.ToString());
        //読み込んでる際には行わないように少し待つ
        //while (!scene.isLoaded)
        //{
        yield return null;
        //}
        //指定したシーン名をアクティブにする
        SceneManager.SetActiveScene(scene);
    }

}

