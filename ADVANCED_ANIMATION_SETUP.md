# Sistema de animações avançadas

## Parâmetros do Animator

Crie estes parâmetros no Animator Controller:

```text
Float: Speed
Bool: Sprint
Integer: CelebrationStyle
Trigger: KickRight, KickLeft
Trigger: PassRight, PassLeft
Trigger: Header
Trigger: Dribble
Trigger: Tackle
Trigger: Slide
Trigger: Foul
Trigger: Fall
Trigger: Injured
Trigger: Celebrate
Trigger: Dive
Trigger: Save
Trigger: Complain
```

## Clipes recomendados

```text
Idle, Run, Sprint
BallControl_Right, BallControl_Left
Kick_Ground_Right, Kick_Ground_Left
Kick_Power_Right, Kick_Power_Left
Pass_Right, Pass_Left
Header
Dribble
Tackle_Standing
SlideTackle
FoulReaction
Fall
Injury
Celebrate_ArmsUp, Celebrate_Jump, Celebrate_Slide, Celebrate_Point
Goalkeeper_Dive, Goalkeeper_Save_Penalty
Complain_Referee
```

Use um Blend Tree `Locomotion` com `Speed` e transições Any State para as ações. As comemorações podem usar `CelebrationStyle` para selecionar subestados. Desative `Apply Root Motion`, porque o CharacterController movimenta o personagem.

## Integração

- `PlayerAnimationController` centraliza todos os estados;
- `DefensiveActionController` chama `Tackle` e `Slide`;
- `GoalkeeperController` chama `Dive` e `Save`;
- `MatchManager` dispara comemorações individuais após gol;
- `SignalFoul`, `SignalComplaint` e `SignalInjury` podem ser chamados pelo árbitro, FoulSystem ou UI;
- `AnimationEventRelay` permite tocar passos e sons nos frames exatos dos clipes.

Para usar pé direito/esquerdo, chame `PlayKick(Foot.Left)` ou `PlayPass(Foot.Left)` conforme a posição do jogador. O jogador controlável pode ser expandido para escolher o pé pela direção relativa da bola.

Adicione `AnimationEventRelay` ao modelo e crie eventos `Footstep` nos clipes de corrida. Use clipes Humanoid com Avatar válido.
