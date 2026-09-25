# IA avançada e controles mobile

## IA

Adicione `AdvancedTacticalManager` a cada equipe e preencha `Ball`, `Own Goal`, `Opponent Goal` e `Defensive Line`. Ele ajusta a linha defensiva, compactação e estilo conforme o placar.

Estilos disponíveis:

- Balanced;
- HighPress;
- CounterAttack;
- Defensive.

Use `ManMarkingAI` para marcação individual e deixe o `TeamAIController` cuidar da ocupação zonal. `GoalkeeperAdvancedAI` prevê a trajetória e reage com atraso configurável. `RefereeAI` acompanha a bola e verifica contatos próximos. `SubstitutionManager` troca jogadores lesionados pelo banco.

Para substitutions, adicione o método `ReplacePlayer(int index, PlayerController replacement)` ao `FootballTeamManager` conforme o prefab de banco usado na cena.

## Mobile

Adicione `MobileGameplayActions` ao jogador controlável e preencha `Ball`, `Ball Socket` e `Player Animation Controller`.

Crie botões com `MobileGameplayButton`:

```text
ProtectButton = Protect
DribbleButton = Dribble
AutoSwitchButton = AutoSwitch
```

O joystick existente continua controlando o movimento. Para passe direcional, adicione um segundo `VirtualJoystick` e use o vetor dele no sistema de passe do jogador; a sensibilidade deve ficar entre 0.7 e 1.5.

## Configuração personalizada

Crie um painel de controles e salve a posição/escala dos botões com `PlayerPrefs`. O sistema atual já suporta multitouch; mantenha um `EventSystem` e um único `MobileInput` na cena.
