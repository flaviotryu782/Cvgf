# Controles touch Android

## Hierarquia recomendada

```text
Canvas (Screen Space - Overlay)
└── MobileHUD (MobileSafeArea)
    ├── MoveJoystick (Image + VirtualJoystick)
    │   └── Handle (Image)
    ├── ActionButtons
    │   ├── KickButton (MobileActionButton: Kick)
    │   ├── PassButton (MobileActionButton: Pass)
    │   ├── SprintButton (MobileActionButton: Sprint)
    │   ├── TackleButton (MobileActionButton: Tackle)
    │   ├── SlideButton (MobileActionButton: Slide)
    │   ├── PressureButton (MobileActionButton: Pressure)
    │   ├── SwitchButton (MobileActionButton: SwitchPlayer)
    │   ├── LobButton (MobileActionButton: Lob)
    │   └── CurveButton (MobileActionButton: Curve)
```

Adicione um `EventSystem` à cena. O Canvas deve usar `Canvas Scaler > Scale With Screen Size`, referência `1920x1080` e `Match=0.5`.

## Joystick

Crie `MoveJoystick` como Image quadrada, adicione `VirtualJoystick`, crie `Handle` como filho e arraste-o para o campo `Handle`. Use raio de 90–120 px. O joystick aceita multitouch sem deixar o dedo dos botões interferir.

## Botões

Adicione `Image` e `MobileActionButton` a cada botão. Configure `Action Type` conforme o nome. Botões de chute, sprint e pressão funcionam enquanto o dedo estiver pressionado; o chute calcula força pelo tempo pressionado e solta quando o dedo sai da área.

## GameManager

O objeto `GameManager` da cena precisa ter `MobileInput`. Não crie uma segunda instância em outra cena sem destruir a anterior. Os scripts dos jogadores leem o joystick pelo singleton.

## Safe area e orientação

Adicione `MobileSafeArea` ao painel que contém os controles. Na primeira cena, adicione `MobileOrientationBootstrap` a um objeto persistente. Em `Project Settings > Player > Resolution and Presentation`, configure Landscape Left/Right e desative Portrait.

## Teste

No Editor, use mouse para arrastar o joystick e clique nos botões. No celular, teste pressionar joystick e botão simultaneamente. Se algum botão não responder, confirme que existe `EventSystem`, que a Image tem `Raycast Target` ligado e que `MobileInput` está na cena.
