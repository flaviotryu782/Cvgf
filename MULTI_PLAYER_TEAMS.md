# Times com vários jogadores

## Hierarquia recomendada

```text
Teams
├── PlayerTeam (FootballTeamManager, TeamSide=Player)
│   ├── Player_Controlled (PlayerController + TeamAIController)
│   ├── Player_Attacker (PlayerController + TeamAIController)
│   ├── Player_Midfielder (PlayerController + TeamAIController)
│   ├── Player_Defender (PlayerController + TeamAIController)
│   └── Player_Goalkeeper (GoalkeeperController)
└── OpponentTeam (FootballTeamManager, TeamSide=Opponent)
    ├── Opponent_Attacker (TeamAIController)
    ├── Opponent_Midfielder (TeamAIController)
    ├── Opponent_Defender (TeamAIController)
    └── Opponent_Goalkeeper (GoalkeeperController)
```

## Configuração dos arrays

No `PlayerTeam`, `Outfield Players` deve conter exatamente, nesta ordem:

1. `Player_Controlled`
2. `Player_Attacker`
3. `Player_Midfielder`
4. `Player_Defender`

Em `AI Players`, coloque os três jogadores de linha que terão IA. Em `Spawn Points`, use quatro pontos de linha e, como quinto ponto, o spawn do goleiro. O jogador no índice zero é o controlado; os demais são ativados automaticamente como IA.

No `OpponentTeam`, `Outfield Players` contém atacante, meio-campista e defensor. Todos devem ter `TeamAIController` e `PlayerController` desativado. O quarto spawn é o goleiro.

## Funções de cada jogador

- **Atacante:** persegue a bola e finaliza perto do gol.
- **Meio-campista:** apoia a jogada e ocupa posição intermediária.
- **Defensor:** protege sua posição e pressiona quando a bola se aproxima.
- **Goleiro:** usa `GoalkeeperController` e fica limitado à área.

No `TeamAIController`, selecione o `Role` correspondente e configure `Home Position`, `Ball`, `Ball Controller`, `Target Goal` e `Ball Socket`.

## Troca de jogador

A troca ocorre automaticamente pelo próximo jogador de linha. No teclado use `Tab`. Para botão Android, adicione um botão com evento `OnClick` chamando `MobileInput.RequestSwitch()`; o `FootballTeamManager` do jogador recebe esse pedido.

## MatchManager

Remova o uso dos antigos arrays individuais de inimigos e goleiro. No `MatchManager`, arraste `PlayerTeam` em `Player Team` e `OpponentTeam` em `Opponent Team`. O reinício e o reset após gol reposicionam todos os jogadores das duas equipes.

## Física

Mantenha `CharacterController` nos jogadores e não adicione Rigidbody neles. Cada jogador de linha precisa de `PlayerController` e `TeamAIController`; o manager alterna qual componente fica ativo. O goleiro usa somente `GoalkeeperController`.
