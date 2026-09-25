# Atualização: controle avançado da bola

Os scripts agora incluem domínio, drible, controle junto ao pé, chute rasteiro, chute alto, curva, passe para alvos, carga do chute e colisão física melhorada.

## Ball

No objeto `Ball`, configure Rigidbody:

- Mass: `0.43`
- Drag: `0.08`
- Angular Drag: `0.05`
- Use Gravity: ligado
- Interpolate: `Interpolate`
- Collision Detection: `Continuous Dynamic`
- SphereCollider com Physics Material: Dynamic Friction `0.35`, Static Friction `0.35`, Bounciness `0.25`, Friction Combine `Multiply`, Bounce Combine `Minimum`.

## Jogador e domínio

Crie um Empty filho de `Player` chamado `BallSocket`, na posição aproximada `(0, 0.15, 0.65)`, à frente dos pés. Arraste-o para o campo `Ball Socket` do PlayerController. Quando a bola entra no raio, ela passa a acompanhar o socket com uma pequena oscilação, permitindo drible e domínio.

## Tipos de chute

- `KickButton`: chute rasteiro; segure mais tempo para aumentar a força.
- `LobButton`: chute alto; segure para controlar a força.
- `CurveButton`: chute com torque lateral.
- `PassButton`: passe para o melhor companheiro, calculado por distância e direção do jogador.

No Canvas, adicione `MobileActionButton` a cada botão e selecione seu `Action Type`.

## Passe para companheiros

Crie os filhos `TeammateTarget_1`, `TeammateTarget_2` ou arraste diretamente os jogadores aliados para o array `Pass Targets` do PlayerController. O sistema escolhe o alvo mais útil considerando distância e direção para frente.

## IA

Em cada `TeamAIController`, preencha `Ball`, `Ball Controller`, `Target Goal`, `Home Position` e `Ball Socket`. Um jogador pode usar `Role=Chaser` e outro `Role=Support`. A IA domina a bola quando chega perto e finaliza ao se aproximar do gol.

## UI opcional de carga

A força já funciona pelo tempo de pressionamento. Para mostrar visualmente a carga, adicione um `Slider` e atualize-o em um script UI usando o tempo desde `BeginKick`; a mecânica não depende do Slider.

## Controles no Editor

- WASD/setas: mover
- Shift: sprint
- Espaço: chute rasteiro
- Ctrl: passe

## Avisos importantes

Não use Rigidbody nos jogadores que possuem CharacterController. Deixe o Rigidbody somente na bola. Se a bola ficar presa no pé, aumente `controlRange` ou mova o `BallSocket` para a frente. Se ela atravessar o gol, mantenha Collision Detection em `Continuous Dynamic` e use um trigger suficientemente profundo.
