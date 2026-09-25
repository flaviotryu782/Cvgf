# Sistema de animações dos jogadores

## 1. Modelo 3D

Substitua a cápsula por um modelo com `Animator`. Mantenha o `CharacterController` no objeto raiz e coloque o modelo como filho:

```text
Player (CharacterController, PlayerController)
└── FootballerModel (Animator, PlayerAnimationController, Avatar)
```

Faça o mesmo com jogadores de IA e goleiros.

## 2. Animator Controller

Crie `Assets/Animations/FootballerAnimator.controller`, atribua-o ao componente Animator e configure estes parâmetros:

```text
Float: Speed
Bool: Sprint
Trigger: Kick
Trigger: Pass
Trigger: Celebrate
Trigger: Fall
Trigger: Defend
Trigger: Dive
```

Estados recomendados:

```text
Locomotion Blend Tree (Speed)
├── Idle       Speed=0
├── Run        Speed=0.62
└── Sprint     Speed=1

Kick
Pass
Celebrate
Fall
Defend
Dive
```

O Blend Tree `Locomotion` deve usar `Speed`. Crie transições Any State para Kick, Pass, Celebrate, Fall, Defend e Dive usando o Trigger correspondente, sem `Has Exit Time` para ações responsivas. Das ações, retorne para Locomotion com `Has Exit Time` ativado.

## 3. Clipes

Use clipes com estes nomes ou mapeie os clipes no Animator:

- `Idle`
- `Run`
- `Sprint`
- `Kick`
- `Pass`
- `Celebrate`
- `Fall`
- `Defend`
- `Dive`

Modelos Humanoid devem ter um Avatar válido. Verifique em Import Settings > Rig > Animation Type: Humanoid e crie o Avatar a partir do modelo.

## 4. Ligações no Inspector

No `PlayerController`, `TeamAIController` e `GoalkeeperController`, arraste `PlayerAnimationController` para `Animation Controller`. Se deixar vazio, o script procura o componente nos filhos.

## 5. Comportamento automático

- Parado: `Speed=0`.
- Correndo: `Speed=.62`.
- Sprint: `Speed=1` e `Sprint=true`.
- Chute: `Kick` ao disparar o chute.
- Passe: `Pass` ao passar.
- Comemoração: `Celebrate` depois de gol.
- Queda: chame `PlayFall()` em colisões ou faltas.
- Defendendo: chame `PlayDefend()` em bloqueios.
- Salto do goleiro: `Dive` quando alcança a bola.

## 6. Android

Evite muitos Animator Controllers diferentes. Use um controller compartilhado e troque apenas o Avatar/modelo. Desative `Apply Root Motion`, pois o movimento é controlado pelos scripts. Use clipes comprimidos e reduza a taxa de amostragem se o APK ficar grande.
