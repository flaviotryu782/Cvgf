# Sistema de câmera avançado

## Hierarquia

```text
Main Camera
└── CameraDirector
```

Adicione `CameraDirector` à câmera principal. Preencha `Target Camera`, `Player Target`, `Ball Target` e `Goal Target`. O `CameraFollow` antigo pode permanecer, mas será ignorado quando houver `CameraDirector`.

## Modos

- **FollowPlayer:** câmera principal acompanhando o jogador ativo.
- **ZoomForShot:** reduz o Field of View durante o chute e aplica vibração leve.
- **FocusGoal:** enquadra a bola e o gol depois de uma finalização.
- **Replay:** câmera acompanha a bola por alguns segundos após o gol.
- **Celebration:** enquadra o jogador durante a comemoração.
- **Penalty:** posição e zoom específicos para cobrança de pênalti.

## Ligação automática

`MatchManager` configura os alvos no início e ativa foco no gol/replay após um gol. `PlayerController` ativa zoom e vibração curta no chute. Para pênaltis, use os eventos dos botões para chamar:

```text
MatchManager.StartPenaltyCamera()
MatchManager.StopSpecialCamera()
```

## Vibração

A vibração de chute usa `AudioManager.Vibrate(.06f)`. A configuração de vibração do `AudioManager` deve estar ligada e o teste precisa ser feito em um dispositivo móvel.

## Ajustes recomendados

- Normal FOV: 55
- Shot FOV: 44
- Penalty FOV: 38
- Follow Offset: `(0, 9, -12)`
- Penalty Offset: `(0, 5, -9)`
- Follow Smooth: 7–10

Para evitar que a câmera atravesse o campo, adicione depois um sistema de colisão por SphereCast entre a câmera e o alvo.
