# Áudio, narração e vibração

## AudioManager

Crie um objeto `AudioManager` na cena `Menu` com quatro AudioSources:

```text
AudioManager
├── MusicSource (loop)
├── AmbienceSource (loop)
├── EffectsSource
└── CommentarySource
```

Adicione o componente `AudioManager`, arraste as fontes e configure:

- `Menu Music`: música do menu;
- `Match Music`: música da partida;
- `Kick Sound`: chute;
- `Pass Sound`: passe;
- `Goal Sound`: gol;
- `Whistle Sound`: apito;
- `Save Sound`: defesa do goleiro;
- `Celebration Sound`: comemoração;
- `Crowd Loop`: torcida em loop;
- `Commentary Clips`: falas de narração.

O objeto permanece entre cenas com `DontDestroyOnLoad`.

## Eventos conectados

- Chute do jogador e da IA: `PlayKick()`;
- Passe: `PlayPass()`;
- Gol: gol, comemoração e narração;
- Início/fim: apito;
- Defesa do goleiro: som de defesa e vibração;
- Android: vibração em gol e defesa.

## Arquivos de áudio

Use formatos leves, preferencialmente `.ogg` ou `.wav` curto. Para música e torcida, importe com `Load Type=Streaming` quando apropriado. Para efeitos curtos, use `Decompress On Load` e Mono.

## Vibração

`Vibration Enabled` pode ser ligado/desligado pelo menu chamando `MenuManager.ToggleVibration(bool)`. A vibração só ocorre em dispositivo Android/iOS; no Editor não produz efeito.

## Cuidados

Use áudio apenas com licença adequada. Não inclua músicas comerciais ou narrações protegidas sem autorização. Se o áudio não tocar, confira se os clips e AudioSources foram arrastados no Inspector e se o volume do projeto não está mudo.
