# Futebol 3D para Android — guia de montagem

## 1. Cenas

Crie `Assets/Scenes/Menu.unity` e `Assets/Scenes/MainGame.unity`. Em **File > Build Settings**, adicione primeiro `Menu` e depois `MainGame`.

## 2. Hierarquia exata de `MainGame`

```text
MainGame
├── GameManager (MobileInput + MatchManager)
├── Field
│   ├── Ground (Plane, collider)
│   ├── BallKickoff (Empty)
│   ├── PlayerSpawn (Empty)
│   ├── EnemySpawn_1 (Empty)
│   ├── EnemySpawn_2 (Empty)
│   ├── GoalkeeperSpawn (Empty)
│   ├── Goal_Player (trigger + GoalTrigger, scoringTeam=1)
│   └── Goal_Enemy (trigger + GoalTrigger, scoringTeam=0)
├── Ball (Sphere, tag Ball, Rigidbody + BallController)
├── Player (tag Player, CharacterController + PlayerController)
├── Enemy_1 (tag Enemy, CharacterController + TeamAIController)
├── Enemy_2 (tag Enemy, CharacterController + TeamAIController)
├── Goalkeeper (tag Goalkeeper, CharacterController + GoalkeeperController)
├── Main Camera (CameraFollow)
└── Canvas
    ├── ScoreText (TextMeshProUGUI)
    ├── TimerText (TextMeshProUGUI)
    ├── EndPanel (inactive initially)
    │   ├── ResultText (TextMeshProUGUI)
    │   └── RestartButton (Button)
    └── MobileControls
        ├── MoveJoystick (Image + VirtualJoystick)
        │   └── Handle (Image)
        ├── KickButton (Image + MobileActionButton, action Kick)
        ├── PassButton (Image + MobileActionButton, action Pass)
        └── SprintButton (Image + MobileActionButton, action Sprint)
```

`Goal_Player` fica dentro do gol do jogador e marca `scoringTeam=1`. `Goal_Enemy` fica dentro do gol adversário e marca `scoringTeam=0`.

## 3. Configuração do MatchManager

No `GameManager`, adicione `MobileInput` e `MatchManager`. Arraste no Inspector:

- `Ball`: componente BallController do objeto Ball
- `Ball Kickoff`: objeto Field/BallKickoff
- `Score Text`, `Timer Text`, `End Panel`, `Result Text`
- `Player Spawn`, `Goalkeeper Spawn`
- `Enemy Spawns`: EnemySpawn_1 e EnemySpawn_2
- `Match Duration`: 120

No botão RestartButton, use o evento `OnClick -> GameManager.MatchManager.RestartMatch()`.

## 4. Configuração da IA

No `Enemy_1` e `Enemy_2`, preencha `ball`, `ballController`, `targetGoal` apontando para o gol do jogador e `homePosition` para o spawn correspondente. Use `Role=Chaser` em um e `Role=Support` no outro.

No `Goalkeeper`, preencha `ball` e `goalCenter` com um Empty colocado sobre a linha do gol. A IA limita a movimentação ao retângulo da área.

## 5. Controles

O joystick usa arraste e funciona no Android e no Editor. Para testar no PC, PlayerController usa WASD/setas, Espaço para chute, Ctrl para passe e Shift para sprint. No Canvas, adicione `EventSystem` (Unity cria automaticamente ao adicionar UI).

## 6. Física recomendada

Ball: SphereCollider, Rigidbody com Mass=0.43, Drag=0.15, Angular Drag=0.05, Collision Detection=Continuous Dynamic, Interpolate=Interpolate. Jogadores usam CharacterController; não adicione Rigidbody neles.

## 7. Android/APK

1. Instale Android Build Support, Android SDK & NDK Tools e OpenJDK pelo Unity Hub.
2. `File > Build Settings > Android > Switch Platform`.
3. Adicione `Menu` e `MainGame` nessa ordem.
4. `Project Settings > Player > Android`: Package Name `com.flaviotryu782.futebol3d`, Orientation Landscape, Scripting Backend IL2CPP, Target Architectures ARM64.
5. Para teste, `Build` gera APK. Para Google Play, escolha `Build App Bundle`, crie um keystore em Publishing Settings e não o perca.
6. Ative depuração USB no celular ou use `Build And Run`.

## 8. Checklist antes do Build

- Tags `Ball`, `Player`, `Enemy` e `Goalkeeper` criadas.
- Todos os campos do MatchManager preenchidos.
- Cada gol tem BoxCollider com `Is Trigger` ativado.
- Ball tem Rigidbody e tag Ball.
- Cena Menu está na posição 0 e MainGame na posição 1.
- Teste primeiro no Editor e depois em um aparelho Android.
