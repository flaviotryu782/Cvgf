# Impedimento, desarme, pressão e faltas

## Hierarquia

```text
Rules
├── GameplayRulesManager
├── OffsideSystem
│   └── OffsideLine (LineRenderer)
└── FoulSystem

Player/Defender
└── DefensiveActionController

Canvas
├── OffsidePanel
├── FoulPanel
├── PressureButton
├── TackleButton
└── SlideButton
```

## Impedimento

No `OffsideSystem`, preencha `Ball`, `Attacking Goal`, `Attackers` e `Defenders`. O sistema calcula a linha do segundo último defensor e atualiza o `LineRenderer`. O método `CheckAtPass(receiver)` deve ser chamado no instante do passe; se retornar `false`, interrompa a jogada e conceda tiro livre indireto.

A linha é visual e serve também para depuração. Para regra oficial, faça a validação no momento exato do passe, não apenas no Update.

## Desarme e pressão

Adicione `DefensiveActionController` aos defensores, com `Ball`, `PlayerAnimationController` e `FoulSystem`.

- `E` ou `TackleButton`: desarme em pé;
- `Q` ou `SlideButton`: carrinho;
- `PressureButton`: mantém pressão/sprint e aproxima o defensor.

O desarme aplica impulso à bola. O carrinho tem alcance, direção, cooldown e chance maior de falta. Não coloque Rigidbody no jogador que usa CharacterController.

## Faltas

`FoulSystem` diferencia contato normal e carrinho. Carrinhos têm maior chance de falta, podem receber cartão amarelo e, dentro do raio configurado da área, iniciam a cobrança de pênalti existente.

Preencha `Penalty Manager`, `Penalty Area Center` e `Penalty Area Radius`. Ligue `FoulCalled` a UI ou ao árbitro se desejar mostrar o motivo da falta.

## Botões Android

Adicione `DefensiveActionButton` aos botões e escolha:

```text
TackleButton = Tackle
SlideButton = Slide
PressureButton = Pressure
```

## Teste no Editor

```text
E = desarme
Q = carrinho
WASD = movimento
Shift = pressão/sprint
```

O sistema não substitui decisões arbitrais profissionais: a chance de falta é configurável para gameplay e deve ser ajustada após testes.
