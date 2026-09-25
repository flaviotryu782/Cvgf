# Pênaltis, faltas e orientação horizontal

## Sistema de pênaltis

Crie um objeto `PenaltySystem` com `PenaltyShootoutManager`, `FoulSystem` e `LandscapeOrientation`.

Preencha:

- `Ball`: objeto com BallController;
- `Penalty Ball Spot`: Empty sobre a marca da penalidade;
- `Target Goal`: gol que será atacado; o eixo `forward` deve apontar para dentro do gol;
- `Goalkeeper`: goleiro adversário;
- `Camera Director`;
- `Power Slider`: Slider opcional;
- `Penalty Panel`: painel dos controles;
- `Result Text`: texto de resultado.

Chame `PenaltyShootoutManager.StartShootout()` quando houver pênalti ou quando uma partida eliminatória terminar empatada.

## Controles mobile

Crie no `PenaltyPanel`:

```text
PenaltyPanel
├── AimLeft
├── AimRight
├── AimUp
├── AimDown
├── KickButton
└── PowerSlider
```

Adicione `PenaltyAimButton` aos botões e configure `Button Type`. Segure os botões de direção para ajustar a mira. Segure `KickButton` para carregar força e solte para chutar.

No Editor, use setas para mirar e segure/solte Espaço para cobrar.

## Faltas

Adicione `FoulSystem` ao objeto `PenaltySystem`. Você pode chamar `CheckContact(velocidadeRelativa)` quando dois jogadores colidirem. O exemplo usa uma chance configurável e inicia o modo de pênalti quando ocorre contato forte. Para produção, substitua a chance por regras de área, posse e último defensor.

## Disputa

O sistema executa até cinco cobranças por time e encerra antes se a diferença for inalcançável. Se continuar empatado, a estrutura pode ser estendida para cobranças alternadas. O goleiro tem probabilidade de defesa influenciada pela direção do chute.

## Tela horizontal obrigatória

Adicione `LandscapeOrientation` na primeira cena carregada, normalmente `Menu`. No Android, configure também em `Project Settings > Player > Resolution and Presentation`:

- Default Orientation: `Landscape Left`;
- Auto Rotation: desativada, ou apenas Landscape Left/Right ativadas;
- Portrait e Portrait Upside Down: desativadas.

No iOS/Android, teste em aparelho real porque alguns launchers podem aplicar uma rotação durante a abertura.
