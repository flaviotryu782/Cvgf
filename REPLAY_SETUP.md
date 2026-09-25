# Replay real de gols

## Hierarquia

```text
ReplaySystem
├── ReplayRecorder
└── ReplayCamera

Canvas
└── ReplayPanel
    ├── SkipButton (ReplayControlButton: Skip)
    ├── CameraButton (ReplayControlButton: NextAngle)
    ├── SaveBestButton (ReplayControlButton: SaveBest)
    └── ReplayLabel
```

No `ReplayRecorder`, arraste `Ball`, todos os jogadores em `Tracked Players`, a câmera em `Replay Camera` e um Empty do campo em `Camera Focus`. Use `Seconds To Keep=10`, `Sample Rate=20` e `Replay Speed=.45`.

O sistema grava continuamente posições, rotações e velocidade da bola. Ao marcar gol, o `MatchManager` pausa a partida, reproduz o lance em câmera lenta por vários segundos e só depois reposiciona as equipes.

## Ângulos e botões

`NextCameraAngle()` alterna entre três posições. O botão `Skip` chama `SkipReplay()`. `SaveBest` guarda metadados do melhor lance no `PlayerPrefs`; a gravação atual fica em memória para não aumentar o APK e evitar gravações excessivas no armazenamento.

## Observação sobre salvar replay

O replay salvo entre sessões registra a duração e o número de quadros. Para persistir a jogada completa, serialize `Frame` em arquivo JSON/binário e salve em `Application.persistentDataPath`; isso é recomendado apenas para o melhor lance, não para cada gol, porque o arquivo pode ficar grande.

## Integração

No `MatchManager`, preencha `Replay Recorder` e `Replay Panel`. Se não preencher o recorder, ele será encontrado automaticamente na cena. O replay exige que `Tracked Players` esteja configurado; sem jogadores rastreados, apenas a bola será exibida.
