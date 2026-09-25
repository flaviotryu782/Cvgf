# IA adaptativa e formações

Foi adicionado um sistema leve de aprendizado online, apropriado para Android. Ele não treina uma rede neural pesada: cada jogador mantém valores Q por estado e ação, atualizados uma vez por segundo e salvos no `PlayerPrefs`. Assim o comportamento melhora com recompensas de passe, avanço, chute e gols, sem depender de internet.

## Ações aprendidas

- Advance: avançar/driblar;
- Pass: procurar companheiro;
- Shoot: finalizar;
- Return: voltar à posição;
- Mark: proteger/marcar.

A IA escolhe com exploração ocasional e reduz essa exploração conforme joga. O aprendizado persiste entre partidas no dispositivo.

## Configuração

Adicione `AdaptiveAIBrain` em cada jogador de IA ou deixe `TeamAIController` criá-lo automaticamente. Preencha `Team Id` diferente para cada equipe e configure `Role`. Preencha o array `Teammates` para habilitar passes entre companheiros.

Crie um objeto `Tactics` com `TacticalFormation`. As formações aceitas são `ThreeTwo`, `TwoOneTwo`, `FourThreeThree` e `FourFourTwo`. Use `SetFormation` a partir de botões do menu para trocar a formação.

## Goleiro

O goleiro agora usa a velocidade da bola para prever sua posição e se mover antes da chegada, limitado à largura e profundidade da área.

## Limitações honestas

Esse é aprendizado por reforço tabular local, não uma rede neural. Ele aprende preferências de decisão e tática durante as partidas, mas não cria um jogador humano perfeito. Para treinar uma rede neural real seria necessário um pipeline separado de simulações, treinamento e importação de modelo, o que é mais pesado para Android.
