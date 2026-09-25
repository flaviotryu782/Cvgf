using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneBootstrap
{
    [MenuItem("Cvgf/Bootstrap Unity Scene Setup")]
    public static void BootstrapScenes()
    {
        EnsureFolder("Assets/Scenes");
        EnsureFolder("Assets/Scripts");

        CreateMenuSceneIfMissing();
        CreateMainGameSceneIfMissing();

        Debug.Log("Cvgf scene bootstrap completed. Open the Menu scene and press Play.");
    }

    private static void EnsureFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
    }

    private static void CreateMenuSceneIfMissing()
    {
        const string scenePath = "Assets/Scenes/Menu.unity";
        if (File.Exists(scenePath))
            return;

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var root = new GameObject("Menu");
        var canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var eventSystem = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));

        canvas.transform.SetParent(root.transform, false);
        eventSystem.transform.SetParent(root.transform, false);

        var canvasComp = canvas.GetComponent<Canvas>();
        canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        var title = new GameObject("TitleText", typeof(RectTransform));
        title.transform.SetParent(canvas.transform, false);
        title.AddComponent<UnityEngine.UI.Text>();
        var titleText = title.GetComponent<UnityEngine.UI.Text>();
        titleText.text = "Futebol 3D";
        titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        titleText.fontSize = 64;
        titleText.alignment = TextAnchor.MiddleCenter;
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.75f);
        titleRect.anchorMax = new Vector2(0.5f, 0.75f);
        titleRect.sizeDelta = new Vector2(600f, 120f);
        titleRect.anchoredPosition = Vector2.zero;

        var startButton = new GameObject("StartButton", typeof(RectTransform));
        startButton.transform.SetParent(canvas.transform, false);
        var startImage = startButton.AddComponent<UnityEngine.UI.Image>();
        startImage.color = new Color(0.2f, 0.7f, 0.3f, 1f);
        var startText = new GameObject("Text", typeof(RectTransform));
        startText.transform.SetParent(startButton.transform, false);
        var startTextComp = startText.AddComponent<UnityEngine.UI.Text>();
        startTextComp.text = "Iniciar Partida";
        startTextComp.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        startTextComp.fontSize = 36;
        startTextComp.alignment = TextAnchor.MiddleCenter;
        startTextComp.color = Color.white;

        var startRect = startButton.GetComponent<RectTransform>();
        startRect.anchorMin = new Vector2(0.5f, 0.45f);
        startRect.anchorMax = new Vector2(0.5f, 0.45f);
        startRect.sizeDelta = new Vector2(300f, 90f);
        startRect.anchoredPosition = Vector2.zero;

        var startButtonComp = startButton.AddComponent<UnityEngine.UI.Button>();
        var startButtonTarget = startButtonComp.targetGraphic = startImage;
        startButtonComp.transition = Selectable.Transition.ColorTint;
        startButtonComp.colors = ColorBlock.defaultColorBlock;

        var startTextRect = startText.GetComponent<RectTransform>();
        startTextRect.anchorMin = Vector2.zero;
        startTextRect.anchorMax = Vector2.one;
        startTextRect.offsetMin = Vector2.zero;
        startTextRect.offsetMax = Vector2.zero;

        var quitButton = new GameObject("QuitButton", typeof(RectTransform));
        quitButton.transform.SetParent(canvas.transform, false);
        var quitImage = quitButton.AddComponent<UnityEngine.UI.Image>();
        quitImage.color = new Color(0.7f, 0.2f, 0.2f, 1f);
        var quitText = new GameObject("Text", typeof(RectTransform));
        quitText.transform.SetParent(quitButton.transform, false);
        var quitTextComp = quitText.AddComponent<UnityEngine.UI.Text>();
        quitTextComp.text = "Sair";
        quitTextComp.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        quitTextComp.fontSize = 32;
        quitTextComp.alignment = TextAnchor.MiddleCenter;
        quitTextComp.color = Color.white;

        var quitRect = quitButton.GetComponent<RectTransform>();
        quitRect.anchorMin = new Vector2(0.5f, 0.25f);
        quitRect.anchorMax = new Vector2(0.5f, 0.25f);
        quitRect.sizeDelta = new Vector2(220f, 80f);
        quitRect.anchoredPosition = Vector2.zero;

        quitButton.AddComponent<UnityEngine.UI.Button>();
        var quitTextRect = quitText.GetComponent<RectTransform>();
        quitTextRect.anchorMin = Vector2.zero;
        quitTextRect.anchorMax = Vector2.one;
        quitTextRect.offsetMin = Vector2.zero;
        quitTextRect.offsetMax = Vector2.zero;

        root.AddComponent<MenuManager>();
        root.AddComponent<AudioManager>();

        EditorSceneManager.SaveScene(scene, scenePath);
    }

    private static void CreateMainGameSceneIfMissing()
    {
        const string scenePath = "Assets/Scenes/MainGame.unity";
        if (File.Exists(scenePath))
            return;

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var gameManager = new GameObject("GameManager");
        gameManager.AddComponent<MobileInput>();
        gameManager.AddComponent<MatchManager>();

        var field = new GameObject("Field");
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.SetParent(field.transform, false);
        ground.transform.localScale = new Vector3(3f, 1f, 5f);

        CreateEmptyChild(field, "BallKickoff");
        CreateEmptyChild(field, "PlayerSpawn");
        CreateEmptyChild(field, "EnemySpawn_1");
        CreateEmptyChild(field, "EnemySpawn_2");
        CreateEmptyChild(field, "GoalkeeperSpawn");

        var goalPlayer = CreateGoal("Goal_Player", 1);
        goalPlayer.transform.SetParent(field.transform, false);
        goalPlayer.transform.position = new Vector3(0f, 0f, 18f);

        var goalEnemy = CreateGoal("Goal_Enemy", 0);
        goalEnemy.transform.SetParent(field.transform, false);
        goalEnemy.transform.position = new Vector3(0f, 0f, -18f);

        var ballObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ballObject.name = "Ball";
        ballObject.tag = "Ball";
        ballObject.transform.position = new Vector3(0f, 0.5f, 0f);
        ballObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        ballObject.AddComponent<Rigidbody>();
        ballObject.AddComponent<BallController>();

        var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = new Vector3(-6f, 1f, 0f);
        player.AddComponent<CharacterController>();
        player.AddComponent<PlayerController>();

        var playerSocket = new GameObject("BallSocket");
        playerSocket.transform.SetParent(player.transform, false);
        playerSocket.transform.localPosition = new Vector3(0f, 0.2f, 0.8f);

        var enemy1 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        enemy1.name = "Enemy_1";
        enemy1.tag = "Enemy";
        enemy1.transform.position = new Vector3(7f, 1f, -8f);
        enemy1.AddComponent<CharacterController>();
        enemy1.AddComponent<TeamAIController>();

        var enemy2 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        enemy2.name = "Enemy_2";
        enemy2.tag = "Enemy";
        enemy2.transform.position = new Vector3(7f, 1f, 8f);
        enemy2.AddComponent<CharacterController>();
        enemy2.AddComponent<TeamAIController>();

        var goalkeeper = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        goalkeeper.name = "Goalkeeper";
        goalkeeper.tag = "Goalkeeper";
        goalkeeper.transform.position = new Vector3(0f, 1f, -16f);
        goalkeeper.AddComponent<CharacterController>();
        goalkeeper.AddComponent<GoalkeeperController>();

        var camera = new GameObject("Main Camera");
        camera.tag = "MainCamera";
        camera.transform.position = new Vector3(0f, 15f, -18f);
        camera.transform.rotation = Quaternion.Euler(30f, 0f, 0f);
        camera.AddComponent<Camera>();
        camera.AddComponent<CameraFollow>();

        var canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvasComp = canvas.GetComponent<Canvas>();
        canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        var scoreText = CreateTextChild(canvas, "ScoreText", "0 - 0", new Vector2(0.5f, 0.95f), new Vector2(250f, 60f), 32);
        var timerText = CreateTextChild(canvas, "TimerText", "02:00", new Vector2(0.5f, 0.12f), new Vector2(250f, 60f), 28);
        var endPanel = new GameObject("EndPanel", typeof(RectTransform), typeof(UnityEngine.UI.Image));
        endPanel.transform.SetParent(canvas.transform, false);
        var endRect = endPanel.GetComponent<RectTransform>();
        endRect.anchorMin = new Vector2(0.5f, 0.5f);
        endRect.anchorMax = new Vector2(0.5f, 0.5f);
        endRect.sizeDelta = new Vector2(400f, 200f);
        endRect.anchoredPosition = Vector2.zero;
        endPanel.GetComponent<UnityEngine.UI.Image>().color = new Color(0f, 0f, 0f, 0.7f);
        var resultText = CreateTextChild(endPanel, "ResultText", "EMPATE", new Vector2(0.5f, 0.65f), new Vector2(300f, 80f), 30);
        var restartButton = new GameObject("RestartButton", typeof(RectTransform), typeof(UnityEngine.UI.Image), typeof(UnityEngine.UI.Button));
        restartButton.transform.SetParent(endPanel.transform, false);
        var buttonRect = restartButton.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.2f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.2f);
        buttonRect.sizeDelta = new Vector2(180f, 60f);
        buttonRect.anchoredPosition = Vector2.zero;
        var restartLabel = CreateTextChild(restartButton, "Text", "Reiniciar", new Vector2(0.5f, 0.5f), new Vector2(160f, 40f), 22);

        var playerController = player.GetComponent<PlayerController>();
        var ballController = ballObject.GetComponent<BallController>();

        playerController.SetBallReferences(ballObject.transform, playerSocket.transform, ballController);

        var ballKickoff = new GameObject("BallKickoff");
        ballKickoff.transform.SetParent(field.transform, false);
        ballKickoff.transform.position = Vector3.zero;

        gameManager.transform.SetParent(scene.GetRootGameObjects()[0].transform, false);
        field.transform.SetParent(scene.GetRootGameObjects()[0].transform, false);
        ballObject.transform.SetParent(scene.GetRootGameObjects()[0].transform, false);
        player.transform.SetParent(scene.GetRootGameObjects()[0].transform, false);
        enemy1.transform.SetParent(scene.GetRootGameObjects()[0].transform, false);
        enemy2.transform.SetParent(scene.GetRootGameObjects()[0].transform, false);
        goalkeeper.transform.SetParent(scene.GetRootGameObjects()[0].transform, false);
        camera.transform.SetParent(scene.GetRootGameObjects()[0].transform, false);

        var matchManager = gameManager.GetComponent<MatchManager>();
        matchManager.SetBall(ballObject.GetComponent<BallController>());
        matchManager.SetKickoffPoint(ballKickoff.transform);

        EditorSceneManager.SaveScene(scene, scenePath);
    }

    private static GameObject CreateGoal(string name, int scoringTeam)
    {
        var goal = new GameObject(name);
        var trigger = goal.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(8f, 3f, 1f);
        var goalTrigger = goal.AddComponent<GoalTrigger>();
        goalTrigger.SetScoringTeam(scoringTeam);
        return goal;
    }

    private static GameObject CreateEmptyChild(Transform parent, string name)
    {
        var child = new GameObject(name);
        child.transform.SetParent(parent, false);
        return child;
    }

    private static GameObject CreateTextChild(GameObject parent, string name, string text, Vector2 anchor, Vector2 size, int fontSize)
    {
        var textObject = new GameObject(name, typeof(RectTransform));
        textObject.transform.SetParent(parent.transform, false);
        var rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;

        var textComp = textObject.AddComponent<UnityEngine.UI.Text>();
        textComp.text = text;
        textComp.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComp.fontSize = fontSize;
        textComp.alignment = TextAnchor.MiddleCenter;
        textComp.color = Color.white;
        return textObject;
    }
}
