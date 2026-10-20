---
projeto: Eco-Scrapper Reclamation
disciplina: Desenvolvimento de Games (prof. Saulo)
grupo: Thiago, Arthur, Rafael
entrega_1_bimestre: 2026-10-22 (quinta)
repositorio: https://github.com/ThiagoGama1/desenvolvimento_games
unity: 6000.0.8f1
template: Universal 3D (URP)
atualizado_em: 2026-10-20
tags: [unity, game-dev, trabalho-faculdade]
---

# Eco-Scrapper: Reclamation — Contexto do Projeto

> **Para o Claude (ou qualquer IA) que ler este arquivo:** este é o documento central do projeto. Ele registra o que foi pedido pelo professor, tudo o que o grupo já decidiu e por quê, quem faz o quê, os nomes combinados entre as partes e o estado atual. **Leia inteiro antes de ajudar.** Respeite as decisões registradas aqui. Se for sugerir mudar alguma, explique o motivo e lembre a pessoa de avisar o grupo e atualizar este arquivo. Ao final de qualquer mudança relevante, sugira a linha que deve entrar no [[#12. Registro de alterações]].

> **Para o grupo:** sempre que algo for decidido, feito ou mudado, atualize a seção correspondente e adicione uma linha no registro de alterações, com data e autor. Este arquivo vai para o Git como qualquer outro, então faça pull antes de editar para não dar conflito.

---

## Índice
1. [[#1. O que é o jogo]]
2. [[#2. O que o professor pede]]
3. [[#3. Decisões técnicas e motivos]]
4. [[#4. Estrutura do repositório e do projeto]]
5. [[#5. Divisão do trabalho]]
6. [[#6. Contratos entre as partes (nomes combinados)]]
7. [[#7. Fluxo de trabalho com Git]]
8. [[#8. Cronograma até a entrega]]
9. [[#9. Status atual]]
10. [[#10. Problemas conhecidos e dicas]]
11. [[#11. GDD: situação do documento]]
12. [[#12. Registro de alterações]]

---

## 1. O que é o jogo

**Eco-Scrapper: Reclamation** é um jogo 3D em terceira pessoa de aventura, plataforma e puzzle de física. O jogador controla o **Scrap-E (Unidade S-01)**, um robô de limpeza bípede, amarelo, com visor de LED azul. Ele é religado pela IA **ARIA** para despoluir o vale de Aethelgard, que foi soterrado por lixo industrial.

**Tema escolhido para o 1º bimestre:** Ecorenovação.

### Nível 1: O Vale Estéril (único nível do 1º bimestre)
- **Objetivo:** coletar os **5 Bio-Filtros**, remover os **3 entulhos** que bloqueiam o canal e ativar a **Torre Purificadora** no console (tecla E).
- **Setores, em ordem:**
  1. **Plataforma de Despertar.** Spawn e tutorial (andar, pular, pulso, coletar).
  2. **Ravina Entulhada.** Passagem estreita bloqueada, primeiro puzzle de física.
  3. **Leito Fluvial Seco.** Área central com poças de lodo, barris de contrapeso e os pontos de spawn aleatório dos filtros.
  4. **Base da Represa e Torre.** Comporta com válvula giratória e o console de ativação.
- **Vitória:** 5 filtros na torre + 3 entulhos removidos + torre ativada. Toca a explosão de partículas verdes, a névoa some, aparece a Tela de Vitória com a pontuação.
- **Derrota:** HP chega a 0 (lodo tóxico tira 10 HP/s, detrito em queda tira 25 HP) ou o Eco-Timer de **3 minutos** zera (só se estiver ligado). Aparece a Tela de Game Over com o botão Reiniciar.
- **Pontuação:** +100 por sucata, +1000 por concluir, +10 por segundo restante no Eco-Timer.

### Controles
| Tecla | Ação |
|---|---|
| W / S | Andar para frente / para trás |
| A / D | Girar o robô (esquerda / direita) |
| Shift (segurar) | Correr |
| Mouse | Câmera (por enquanto a câmera é fixa atrás do robô; órbita com mouse só se sobrar tempo) |
| Espaço | Pular |
| Botão esquerdo / E | Pulso Cinético (arremessa entulho) e interagir |
| Esc | Pausa |
| R | Reiniciar o nível |
| G / H | Emotes (opcional) |
| E-C-O em sequência | Cheat: vida cheia e velocidade (opcional) |

O detalhamento completo está no GDD (ver [[#11. GDD: situação do documento]]).

---

## 2. O que o professor pede

Fonte: enunciado `Trabalho_2026-2.pdf` e template `GDD_Template.Rev01.docx`.

### Regras gerais
- Jogo **3D**, em 1ª ou 3ª pessoa (nós: 3ª pessoa).
- **Unity 6000.0.8f1**, template **Universal 3D (URP)**, "a mesma versão instalada no lab 12".
- Apresentação e execução **no laboratório**, durante a aula.
- **Cópia parcial ou total de outro jogo anula o trabalho.**
- **Validar com o professor** quais assets de terceiros podem ser usados.
- Nota de participação proporcional ao que cada um fez.

### Entrega do 1º bimestre
- **GDD detalhado**, focado no nível 1.
- **Projeto Unity zipado inteiro:** `Assets`, `Packages`, `ProjectSettings`, `UserSettings` e scripts.
- Vídeo: **não** é pedido no 1º bimestre, só no 2º.

### Critérios de avaliação do 1º bimestre
Usar como checklist final antes de entregar.
- [ ] **Tema** inserido e valorizado (um dos três; nós: Ecorenovação, mínimo de 5 elementos)
- [ ] **Colisão**
- [ ] **Movimentação** com `Transform.Translate` **e** `Rigidbody.AddForce`
- [ ] **Rotação** (`Transform.Rotate`)
- [ ] **Torque** (`Rigidbody.AddTorque`)
- [ ] **Vetor de prefabs**
- [ ] **Scripts**
- [ ] **Partículas** (explosão, pedrinhas)
- [ ] **Fumaça**
- [ ] **Som**
- [ ] **Animações:** parado, caminhando, correndo, pulando, caindo
- [ ] **Randomizar a posição no vetor** (`Random.Range`)
- [ ] **UI** com texto e botão, **incluindo um botão de reiniciar o jogo**
- [ ] **Spawn de objetos por função**
- [ ] **Tag**
- [ ] **5 níveis planejados** no GDD, com **pelo menos 70 assets de categorias diferentes por nível** (no 1º bimestre, só o nível 1 é construído)
- [ ] **Documentação do código:** cada função com comentário dizendo o que faz, o que significam os parâmetros e o retorno, e o que acontece no corpo
- [ ] Gameplay, mecânica e qualidade gráfica (assets, cenário, terreno, animações)

### Já previsto para o 2º bimestre (não fazer agora)
Os três temas (Ecorenovação, Tecnologia e Skills, mínimo de 3 itens cada), terreno próprio com no mínimo 10 camadas de textura, NavMesh, NPC, partículas criadas por nós, iluminação, pós-processamento, os 5 níveis completos e um vídeo de 1 a 3 minutos.

---

## 3. Decisões técnicas e motivos

| # | Decisão | Motivo |
|---|---|---|
| D1 | Usar a **Unity 6000.0.8f1** | É a versão escrita no enunciado. Projeto criado numa versão mais nova pode não abrir no lab. A 8f1 é da fase "Unity 6 Preview" (jun/2024) e não aparece na lista normal do Hub. Instala colando `unityhub://6000.0.8f1/fa7102f01711` no navegador. |
| D2 | Ignorar o "Security Alert" do Hub para essa versão | O alerta é sobre uma falha nos **builds publicados** com versões antigas da Unity. O jogo não será publicado, só roda no lab, então não afeta o trabalho. |
| D3 | **Não** usar o AI Assistant da Unity | Ele exige a 6000.0.76f1 ou mais nova, então não funciona na 8f1. Abrir o projeto numa versão nova para usá-lo atualizaria o projeto e poderia impedir que ele abrisse no lab. |
| D4 | **Não** usar o controle de versão da Unity (Unity Version Control) | Usamos Git e GitHub. |
| D5 | **Sem branches**: todo mundo commita direto na `main` | Prazo de 2 dias. Branch não evita conflito, só adia para o merge. E conflito em cena e prefab da Unity é difícil de resolver. A separação por cena e pasta já evita que um mexa no arquivo do outro. |
| D6 | Cada pessoa mexe **só na sua cena**. A cena `Nivel1` é **só do Arthur** | Duas pessoas editando a mesma cena geram conflito no Git. |
| D7 | Tudo o que entra na cena principal vai como **prefab** | Um prefab pode ser criado na cena de teste e depois arrastado para a `Nivel1` pelo Arthur, sem que os outros mexam na cena dele. |
| D8 | Pastas **direto em `Assets/`** (sem a pasta `_Projeto`) | O template já criou `Assets/Scenes`, então ficou mais simples seguir esse padrão. |
| D9 | `.gitignore` padrão de Unity dentro de `EcoScrapper/` | Impede que a `Library` (vários GB, recriada sozinha), a `Temp`, os `Logs` e os arquivos do Visual Studio subam para o GitHub. |
| D10 | O **zip da entrega é feito da pasta do PC**, não baixado do GitHub | O `.gitignore` ignora a `UserSettings`, mas o professor pede essa pasta no zip. |
| D11 | **Seção 6 do GDD (código) fica em branco** até os scripts ficarem prontos | O código que estava no GDD era só exemplo, tinha bugs e contradizia o próprio documento. Os scripts reais entram no fim. |
| D12 | O código é documentado **enquanto é escrito** (`/// <summary>` em toda função) | O professor avalia isso no 1º bimestre. Deixar para o fim vira mutirão na véspera. |
| D13 | Assets de terceiros: pacotes **grátis** da Unity Asset Store, free3d, Mixamo e TextureBox, **validados com o professor** | Exigência do enunciado. Evitar assets gerados por IA por enquanto (difíceis de validar e costumam vir pesados). |
| D14 | Personagem: **Y Bot do Mixamo** pintado de amarelo (ou um robô humanoide grátis) | O Y Bot já tem visual robótico e já vem com rig, o que acelera tudo. |
| D15 | Scripts no **estilo do curso Create with Code**: variáveis `public`, lógica no `Update`, `Translate`/`Rotate`/`AddForce` diretos | É o que o professor ensinou. Código mais simples de entender, explicar na apresentação e manter entre os três. |
| D16 | Movimento estilo "tanque": **W/S anda com `Translate`, A/D gira com `Rotate`**, Shift corre, Espaço pula com `AddForce` | Deixa os critérios de movimentação e rotação bem visíveis e dispensa um script de câmera. |
| D17 | Câmera: **Main Camera como filha do Player**, posicionada atrás e acima | Segue o robô sem precisar de script. Órbita com mouse fica como extra. |
| D18 | Eco-Timer com **3 minutos** | O `MainMenuController` do Rafael já usava 3 minutos; foi mais simples ajustar o GDD (seções 3.1 e 3.7) do que o código. |

---

## 4. Estrutura do repositório e do projeto

### Repositório
```
desenvolvimento_games_saulo/            ← raiz do Git (repo: ThiagoGama1/desenvolvimento_games)
├── CONTEXTO_PROJETO.md                 ← este arquivo
├── Trabalho_2026-2 (arquivos do saulo).zip   ← enunciado e template do professor
└── EcoScrapper/                        ← projeto Unity (abrir esta pasta no Hub)
    ├── .gitignore
    ├── Assets/
    ├── Packages/
    ├── ProjectSettings/
    ├── UserSettings/   (ignorada pelo Git)
    └── Library/        (ignorada pelo Git, recriada sozinha)
```

> Para abrir: Unity Hub → **Add → Add project from disk** → selecionar a pasta **`EcoScrapper`**, não a raiz do repositório.

### Pastas dentro de `Assets/`
```
Assets/
├── Scenes/                     cenas
│   ├── Nivel1.unity            cena principal (dono: Arthur)
│   ├── MenuPrincipal.unity     (dono: Rafael)
│   ├── Cena_Teste_Thiago.unity (dono: Thiago)
│   └── Cena_Teste_Rafael.unity (dono: Rafael)
├── Scripts/                    scripts C# (dono: Thiago)
├── Prefabs/                    objetos prontos para usar
│   ├── Cenario/                partículas, objetos de cenário montados (Arthur)
│   ├── Personagem/             ScrapE (Rafael)
│   ├── UI/                     HUD_Canvas (Rafael)
│   └── Gameplay/               Bio-Filtro, Torre, SpawnManager etc. (Thiago)
├── Art/                        matéria-prima (modelos, texturas, sons, animações)
│   ├── Cenario/                (Arthur)
│   └── Personagem/             modelo e animações do Mixamo (Rafael)
├── Settings/                   configuração do URP. NÃO MEXER. Se apagar, tudo fica rosa.
└── InputSystem_Actions.inputactions   mapa de controles do Input System (não apagar)
```

- **`Art` versus `Prefabs`:** `Art` guarda o arquivo como foi baixado (o modelo 3D do barril). `Prefabs` guarda o objeto montado e configurado (o barril com Rigidbody, Collider e tag `HeavyDebris`).
- Pacotes importados da Asset Store criam pastas próprias direto em `Assets/`. Isso é normal e não precisa mover.
- Pastas e arquivos devem ser criados **pela Unity** (aba Project), não pelo Explorer do Windows, para gerar os `.meta`. Os `.meta` **devem** ir para o Git.

---

## 5. Divisão do trabalho

### Thiago: programação (cena `Cena_Teste_Thiago`, pasta `Scripts`, `Prefabs/Gameplay`)
Testa tudo com cubos no lugar dos modelos até as partes dos outros ficarem prontas. Prioridade, em ordem:
1. Movimento do jogador (`Translate` + `Rotate` + `AddForce`, pulo) e câmera filha do Player. **`PlayerMovement.cs` pronto e testado em 2026-10-20** (anda, corre, gira, pula só no chão, empurra caixote). Material `Art/Personagem/SemAtrito` no Capsule Collider. Tremida leve ao encostar em parede fixa: conhecida, correção com `SweepTest` pronta se incomodar.
2. Empurrão por contato (física da Unity, sem código) e Pulso Cinético (`AddForce` + `AddTorque`, `Physics.OverlapSphere`). **`InteractionController.cs` pronto e testado em 2026-10-20.** Campos opcionais no Inspector: `impactParticlePrefab` (prefab de pedrinhas do Arthur) e `pulseSound`.
3. `SpawnManager` (vetor de prefabs + `Random.Range` + spawn por função)
4. Coleta de Bio-Filtros e sucata (inventário do jogador)
5. Torre: depositar filtros, tecla E no console, condição de vitória
6. HP, dano do lodo, dano de detrito e game over
7. Rotação dos filtros (`Transform.Rotate`) e válvula da comporta
8. Ligar HUD, painéis e botões do Rafael aos scripts (`SceneManager.LoadScene` no Reiniciar)
9. Extras se sobrar tempo: Eco-Timer, emotes e cheat E-C-O

### Dependências entre as partes
O que cada script do Thiago precisa dos outros. "Não" quer dizer que dá para fazer e testar com cubos na `Cena_Teste_Thiago`; o que vem dos outros só é encaixado depois, arrastando no Inspector, sem mudar código.

| Script / tarefa do Thiago | Precisa de | De quem | Bloqueia? |
|---|---|---|---|
| `SpawnManager` | Pontos `SpawnPoint_01...` na `Nivel1` | Arthur | Não (testa com objetos vazios próprios) |
| `Rotator` + `PlayerInventory` | Nada | | Não |
| `PurifierTower` | Modelo da torre e prefab `Particula_Purificacao` | Arthur | Não (testa com cubo) |
| `PlayerHealth` | Poças de lodo com `ToxicSlime` | Arthur | Não (testa com cubo marcado) |
| `GameManager` + `UIManager` | Prefab `HUD_Canvas` com textos, painéis e botões | Rafael | **Sim**, a parte de UI só fecha com o canvas |
| Animações no robô | Prefab `ScrapE` com `ScrapE_Animator` | Rafael | **Sim**, para ver as animações (o código já envia `Speed`, `IsGrounded`, `Jump`) |
| Partícula e som do pulso | Prefab `Particula_Pedrinhas` e um som | Arthur | Não (1 minuto: arrastar no `InteractionController`) |

**Prioridade de ajuda:** o Rafael, porque o `HUD_Canvas` e o `ScrapE` são os únicos bloqueios reais.

**Como ligar o `ScrapE` quando chegar (~15 min):** arrastar o `ScrapE` para dentro do `Player` (como filho), desativar o Mesh Renderer da cápsula, arrastar o `ScrapE` para o campo `animator` do `PlayerMovement`, alinhar os pés com a base da cápsula e ajustar o Capsule Collider ao tamanho do modelo.

### Arthur: cenário (cena `Nivel1`, `Art/Cenario`, `Prefabs/Cenario`)
1. Terreno (Terrain): os 4 setores, ravina, leito do rio e platôs
2. Camadas de textura no terreno: solo árido, cascalho, rocha, lodo
3. Importar e posicionar **70+ assets** seguindo as categorias da seção 4.1 do GDD
4. Objetos com física: Rigidbody + Collider + tag `HeavyDebris` (massa 15 para o tambor e 50 para o contrapeso)
5. Poças de lodo: Collider com **Is Trigger** + tag `ToxicSlime`
6. Uns 10 pontos de spawn vazios (`SpawnPoint_01`, `SpawnPoint_02`...) no Leito Seco
7. Névoa (Lighting → Environment → Fog)
8. Partículas: fumaça (fica na cena), pedrinhas (**prefab**), purificação (**prefab**)
9. Sons (AudioSource) e integração dos prefabs dos outros na `Nivel1`

### Rafael: personagem e interface (cenas `Cena_Teste_Rafael` e `MenuPrincipal`, `Art/Personagem`, `Prefabs/Personagem`, `Prefabs/UI`)
1. Scrap-E e animações: Idle, Walking, Running, Jump, Falling Idle (Walking e Running com **In Place**). **Pronto em 2026-10-20** (`Art/Personagem/Idle.anim`, `Walking.anim`, `Running.anim`, `Jump.anim`, `FallingIdle.anim` + materiais URP amarelo metálico, junta escura e visor LED azul emissivo).
2. Importação **Humanoid** e **Loop Time** nas animações de ciclo. **Pronto em 2026-10-20** (clipes de ciclo com `loopTime` ativo, `Jump.anim` sem loop, mais utilitário `Art/Personagem/Editor/MixamoSetupUtility.cs` no menu `EcoScrapper → Rafael` para troca automática em 1 clique por FBX do Mixamo se desejado).
3. `ScrapE_Animator` com os parâmetros e transições combinados (ver [[#6. Contratos entre as partes (nomes combinados)]]). **`Art/Personagem/ScrapE_Animator.controller` pronto e testado em 2026-10-20** (`Speed` Float, `IsGrounded` Bool, `Jump` Trigger e máquina de estados completa sem Has Exit Time nas transições de movimento).
4. Prefab `ScrapE` com Animator, **Apply Root Motion desligado** e tag `Player`. **`Prefabs/Personagem/ScrapE.prefab` pronto em 2026-10-20** (malha 3D articulada do robô com hierarquia `Hips`/`Torso`/`Head`/`Visor_LED` e membros para animação imediata).
5. HUD e painéis de Pausa, Vitória e Game Over, salvos como prefab `HUD_Canvas`. **`Prefabs/UI/HUD_Canvas.prefab` pronto em 2026-10-20** com todos os nomes exatos da seção 6 (`BarraHP`, `TextoFiltros`, `TextoSucata`, `TextoTimer` desativado, `PainelPausa`, `PainelVitoria`, `PainelGameOver` desativados e botões configurados).
6. Cena `MenuPrincipal` e lista de cenas no Build Profiles / Build Settings. **`MenuPrincipal.unity`, `Cena_Teste_Rafael.unity`, `Scripts/MainMenuController.cs` e `EditorBuildSettings.asset` (`MenuPrincipal` índice 0, `Nivel1` índice 1) prontos e testados em 2026-10-20.**
7. Na quinta: zip final e conferência do checklist da seção 2

---

## 6. Contratos entre as partes (nomes combinados)

> **Regra:** estes nomes são usados pelo código. Mudar um nome aqui **quebra o jogo sem dar erro**: o script simplesmente não encontra o que procura. Se precisar mudar, avise o grupo e atualize esta seção **antes**.

### Tags (já criadas no projeto em 2026-10-20)
| Tag | Vai em | Quem cola | O que o código faz |
|---|---|---|---|
| `Player` | Scrap-E (já vem na Unity) | Rafael | Identifica o jogador nos triggers |
| `EcoItem` | Bio-Filtros | Thiago | Coleta e conta para a meta de 5 |
| `ScrapCoin` | Sucatas coletáveis | Arthur / Thiago | Soma pontos |
| `HeavyDebris` | Entulhos, barris e pedras com física | Arthur | O Pulso Cinético arremessa |
| `ToxicSlime` | Poças de lodo | Arthur | Causa dano contínuo |
| `PurifierTrigger` | Área do console da torre | Thiago / Arthur | Permite ativar a torre com E |

### Animator (`ScrapE_Animator`)
| Parâmetro | Tipo | Quem escreve | Significado |
|---|---|---|---|
| `Speed` | Float | `PlayerMovement` | 0 parado, até 0.5 andando, até 1 correndo (Shift) |
| `IsGrounded` | Bool | `PlayerMovement` | Está pisando no chão (detectado por `OnCollisionEnter` com superfície embaixo) |
| `Jump` | Trigger | Script de movimento | Disparado no momento do pulo |

Transições: Idle↔Walking (Speed 0.1), Walking↔Running (Speed 0.6), Any State→Jump (trigger), Jump→Falling (fim da animação), Falling→Idle (IsGrounded = true). Desmarcar **Has Exit Time** nas de movimento.

> **A confirmar quando o script de movimento estiver pronto:** os valores exatos de `Speed` em que o robô troca de animação.

### Prefabs
| Prefab | Pasta | Dono |
|---|---|---|
| `ScrapE` | `Prefabs/Personagem` | Rafael |
| `HUD_Canvas` | `Prefabs/UI` | Rafael |
| `Particula_Pedrinhas` | `Prefabs/Cenario` | Arthur |
| `Particula_Purificacao` | `Prefabs/Cenario` | Arthur |
| `BioFiltro` | `Prefabs/Gameplay` | Thiago |
| `SucataCoin` | `Prefabs/Gameplay` | Thiago |
| `TorrePurificadora` | `Prefabs/Gameplay` | Thiago / Arthur |

### Objetos de UI (dentro do `HUD_Canvas`)
| Nome do objeto | O que é |
|---|---|
| `BarraHP` | Slider da integridade (0 a 100) |
| `TextoFiltros` | "Bio-Filtros: X / 5" |
| `TextoSucata` | "Sucata: Y" |
| `TextoTimer` | Cronômetro (começa desativado) |
| `PainelPausa` | Botões `BotaoContinuar`, `BotaoReiniciar`, `BotaoMenu` |
| `PainelVitoria` | `TextoPontuacao`, `BotaoReiniciar`, `BotaoMenu` |
| `PainelGameOver` | `BotaoReiniciar`, `BotaoMenu` |

Todos os painéis começam **desativados**.

### Cena e spawn
- Pontos de spawn dos filtros: objetos vazios chamados `SpawnPoint_01`, `SpawnPoint_02`... (mínimo de 5, ideal 10).
- Cenas no build, em ordem: `MenuPrincipal` (índice 0), `Nivel1` (índice 1).

### Padrão de documentação do código (exigido pelo professor)
Toda função leva este cabeçalho:
```csharp
/// <summary>
/// O que a função faz.
/// </summary>
/// <param name="nome">O que esse parâmetro representa.</param>
/// <returns>O que é retornado (ou "void, sem retorno").</returns>
/// <remarks>
/// Passo a passo do que acontece no corpo da função.
/// </remarks>
```

---

## 7. Fluxo de trabalho com Git

- Branch única: **`main`**.
- **Antes de começar a trabalhar:** `git pull`
- **Sempre que algo funcionar:**
  ```bash
  git pull
  git add .
  git status      # conferir: NÃO pode aparecer Library/ nem Temp/
  git commit -m "descrição curta do que foi feito"
  git push
  ```
- Commits pequenos e frequentes.
- Quem não usa terminal pode usar o **GitHub Desktop**.

### Arquivos que todo mundo pode mexer sem perceber (risco de conflito)
| Arquivo | Quando muda | Regra |
|---|---|---|
| `ProjectSettings/TagManager.asset` | Ao criar tag | As tags já foram criadas. Não criar novas sem avisar. |
| `Packages/manifest.json` | Ao importar pacote (Asset Store, TextMeshPro...) | Quem importar faz commit e push **logo em seguida** e avisa no grupo. |
| `ProjectSettings/EditorBuildSettings.asset` | Ao mexer na lista de cenas do build | Só o Rafael mexe. |
| `Assets/Scenes/Nivel1.unity` | Ao editar a cena principal | Só o Arthur mexe. |

Se der conflito, quase sempre vai ser num desses arquivos. Mandar o print do erro antes de tentar resolver.

---

## 8. Cronograma até a entrega

| Quando | Meta |
|---|---|
| **Terça 20/10** | Projeto criado e no GitHub ✅. Cada um começa sua parte. |
| **Quarta 21/10, tarde** | Primeira junção na `Nivel1`: robô animado + movimento + cenário. |
| **Quarta 21/10, noite** | Ajustes, correções, UI ligada aos scripts. |
| **Quinta 22/10, antes da aula** | Testar num PC do lab, preencher a seção 6 do GDD com os scripts reais, zipar e entregar. |

### Se faltar tempo, cortar nesta ordem
1. Emotes (G/H)
2. Cheat E-C-O
3. Eco-Timer
4. Easter eggs

**Não pode cortar:** movimento, física (AddForce/Torque), rotação, spawn aleatório, coleta, partículas, fumaça, som, as 5 animações, tags, UI com o botão Reiniciar, documentação do código.

---

## 9. Status atual

### Feito
- [x] GDD do Rafael revisado (2026-10-20)
- [x] Guia do grupo em PDF (regras e passo a passo do Arthur e do Rafael)
- [x] Unity 6000.0.8f1 instalada no PC do Thiago
- [x] Projeto `EcoScrapper` criado (Universal 3D, 6000.0.8f1)
- [x] Repositório no GitHub ligado ao projeto, primeiro commit na `main`
- [x] `.gitignore` de Unity em `EcoScrapper/`
- [x] Pastas criadas (`Scripts`, `Prefabs/*`, `Art/*`)
- [x] Cenas criadas (`Nivel1`, `MenuPrincipal`, `Cena_Teste_Thiago`, `Cena_Teste_Rafael`)
- [x] Tags criadas (`EcoItem`, `ScrapCoin`, `HeavyDebris`, `ToxicSlime`, `PurifierTrigger`)
- [x] Push da estrutura de pastas, cenas e tags
- [x] `Cena_Teste_Thiago` montada (cápsula Player com câmera filha, chão, Caixa, Degrau, Plataforma e Caixote com `HeavyDebris`)
- [x] `PlayerMovement.cs` pronto e testado
- [x] `InteractionController.cs` (Pulso Cinético) pronto e testado
- [x] `ScrapE.prefab` pronto em `Prefabs/Personagem` (tag `Player`, `Apply Root Motion` desligado, malha 3D articulada amarela com visor LED azul)
- [x] `ScrapE_Animator.controller` + 5 animações (`Idle`, `Walking`, `Running`, `Jump`, `FallingIdle`) e utilitário `MixamoSetupUtility.cs`
- [x] `HUD_Canvas.prefab` pronto em `Prefabs/UI` (`BarraHP`, `TextoFiltros`, `TextoSucata`, `TextoTimer` e painéis `PainelPausa`, `PainelVitoria`, `PainelGameOver`)
- [x] `MenuPrincipal.unity` e `MainMenuController.cs` prontos (vitrine 3D giratória do Scrap-E, Iniciar, Controles, Eco-Timer e Sair)
- [x] `Cena_Teste_Rafael.unity` montada e integrada com `PlayerMovement`, `InteractionController`, `ScrapE` e `HUD_Canvas`
- [x] Parte do Rafael testada na Unity pelo Thiago (2026-10-20): `MenuPrincipal`, `Cena_Teste_Rafael`, `ScrapE` e animações funcionando
- [x] `ScrapE.prefab` corrigido: ID reservado `100100000` trocado e tag `Player` removida (fica só na cápsula)
- [x] `EditorBuildSettings.asset` configurado com `MenuPrincipal` (índice 0) e `Nivel1` (índice 1)

### Pendente
- [ ] Convidar Arthur e Rafael como colaboradores no GitHub
- [ ] Arthur e Rafael clonarem e abrirem o projeto na 6000.0.8f1
- [ ] Validar assets de terceiros com o professor
- [ ] Apagar `Assets/TutorialInfo` e `Assets/Readme.asset` (opcional, só conteúdo do template)
- [ ] Thiago: `SpawnManager`, `Rotator` + `PlayerInventory`, `PurifierTower`, `PlayerHealth`, `GameManager` + `UIManager`
- [ ] Arthur: tudo da seção 5 (cenário)
- [ ] Rafael: item 7 da seção 5 (na quinta-feira: zip final e conferência do checklist da seção 2 após integração final)

---

## 10. Problemas conhecidos e dicas

### Gerais
- **Arquivos da Unity gerados por IA fora do editor** (prefabs, cenas, `.anim`, `.controller` escritos direto em texto) podem ter erros que só aparecem ao abrir na Unity. Já aconteceu: o `ScrapE.prefab` veio com o ID `100100000`, que a Unity reserva, e gerou o erro *"File contains the same file identifier (100100000) for multiple object types"*. **Regra:** depois de gerar qualquer arquivo assim, abrir na Unity, olhar o Console e dar Play antes de commitar. Ao escolher IDs (`&número`) num arquivo YAML da Unity, não usar `100100000` nem números "redondos" que possam colidir.
- **Objeto rosa** depois de importar um asset: o material não é do URP. Use `Window → Rendering → Render Pipeline Converter`, marque **Material Upgrade** e clique em **Convert**.
- **Não mexer em `Assets/Settings`.** É a configuração do URP.
- **Abrir o projeto pela pasta `EcoScrapper`**, não pela raiz do repositório.
- **Instalação da Unity falhou com "Prompt de elevação cancelado":** a janela de permissão de administrador ficou escondida ou expirou. Clique em Tentar novamente e aceite. Se não tiver permissão de admin, troque o local de instalação no Hub (Configurações → Instalações) para uma pasta dentro do usuário.

### Programação (Thiago)
- **UI usa o `Text` antigo da Unity** (`UnityEngine.UI.Text`), não TextMeshPro. Os scripts que mexem no HUD devem usar esse tipo.
- **`MainMenuController` (Rafael) já cuida de:** botões Reiniciar e Menu Principal do HUD (`ReiniciarJogo()`, `CarregarMenuPrincipal()`), tecla **R** para reiniciar, `Time.timeScale` voltando a 1 ao trocar de cena, e a escolha do Eco-Timer na variável estática `MainMenuController.useEcoTimer`. O `UIManager` não deve repetir isso; falta só o `BotaoContinuar`, a pausa com Esc e atualizar os textos.
- **`Nivel1` precisa de um `EventSystem`** (GameObject → UI → Event System) para os botões do HUD funcionarem.
- **Input System:** o `Active Input Handling` do projeto está em **`Both`** (conferido em 2026-10-20), então tanto o `Input` antigo (`Input.GetAxis`, `Input.GetKeyDown`) quanto o Input System novo funcionam. Não mudar essa configuração.
- O jogador usa **Rigidbody** (o professor exige `AddForce`), então **não** usar CharacterController no mesmo objeto, e **não** usar `OnControllerColliderHit`, que só funciona com CharacterController. Para detectar contato, use `OnCollisionEnter` / `OnCollisionStay` / `OnTriggerEnter`.
- No Rigidbody do jogador, travar rotação em X e Z (`Constraints → Freeze Rotation X e Z`) para o robô não tombar.
- Para dano de detrito em queda, olhar `collision.relativeVelocity.magnitude` para não dar dano em simples esbarrão.

### Personagem (Rafael)
- Walking e Running **sem In Place** fazem o personagem andar sozinho e brigar com o script.
- **Apply Root Motion** tem que estar desligado no Animator.
- Animações importadas sem **Copy From Other Avatar** ficam tortas ou não tocam.

### Cenário (Arthur)
- Partículas de impacto e de purificação precisam ser **prefabs**, com **Looping** e **Play On Awake** desligados, porque o código cria e dispara elas na hora certa.
- Poça de lodo sem **Is Trigger** vira chão sólido, e o dano não funciona.

### Usando o Claude no projeto
- Mande **print da tela da Unity** junto com a pergunta. Fica muito mais fácil saber onde clicar.
- Passe este arquivo como contexto no começo da conversa.

---

## 11. GDD: situação do documento

- **Autor original:** Rafael.
- **Versão revisada:** `GDD - Eco-Scrapper Reclamation (revisado).pdf`, gerada em 2026-10-20. Está na **raiz do repositório** (`GDD - Eco-Scrapper Reclamation (revisado).pdf`). Há uma cópia antiga no PC do Thiago em `Downloads/Trabalho_2026-2`, mas a do repositório é a oficial.
- **O que foi adicionado ou mudado na revisão, para seguir o template do professor:**
  - 3.2: rotação com `Transform.Rotate`, separada do torque (filtros girando, válvula da comporta, orientação do robô)
  - 3.6: Combate e Conflito (sem combate direto, perigos ambientais com valores de dano)
  - 3.7: Opções do Jogo (Eco-Timer ligado/desligado)
  - 3.1 e 3.7: Eco-Timer ajustado de 4 para **3 minutos** em 2026-10-20 (D18)
  - 3.8: Replay e Salvamento (sem save no nível 1, replay por botão e tecla R)
  - 3.9: Fluxo de Telas (diagrama + descrição de cada tela)
  - 6: Documentação do Código, **em branco até o fim do desenvolvimento**
  - 7: Inteligência Artificial (inimigos, NPCs e IA de suporte)
  - 8: Requisitos Técnicos (era a 7)
  - 9: Arte do Jogo (estilo e tabela dos assets principais)
- **Falta fazer antes da entrega:** preencher a seção 6 com os scripts reais e conferir se o que está no GDD bate com o jogo construído. O que for cortado precisa ser marcado como "planejado".

---

## 12. Registro de alterações

Formato: `AAAA-MM-DD | quem | o que mudou`. A linha mais nova vai no topo.

- 2026-10-20 | Thiago | Parte do Rafael testada na Unity; `ScrapE.prefab` corrigido (ID reservado 100100000 e tag Player); alerta sobre arquivos gerados fora do editor na seção 10
- 2026-10-20 | Thiago | Branch do Rafael revisada e mesclada na `main`; Eco-Timer fixado em 3 minutos (D18); GDD revisado movido para a raiz do repositório; notas de integração com o `MainMenuController` e o `HUD_Canvas`
- 2026-10-20 | Rafael | `MenuPrincipal.unity`, `Cena_Teste_Rafael.unity`, `MainMenuController.cs` e `EditorBuildSettings.asset` (`MenuPrincipal` índice 0, `Nivel1` índice 1) prontos e testados; criado `GUIA_RAFAEL.md`
- 2026-10-20 | Rafael | `HUD_Canvas.prefab` criado em `Prefabs/UI` com todos os objetos e painéis desativados do contrato da seção 6 (`BarraHP`, `TextoFiltros`, `TextoSucata`, `TextoTimer`, `PainelPausa`, `PainelVitoria`, `PainelGameOver`)
- 2026-10-20 | Rafael | `ScrapE.prefab` (`Prefabs/Personagem`), `ScrapE_Animator.controller` com as 5 animações (`Idle`, `Walking`, `Running`, `Jump`, `FallingIdle`) e utilitário `MixamoSetupUtility.cs` prontos
- 2026-10-20 | Thiago | Status atualizado; adicionada a tabela de dependências entre as partes
- 2026-10-20 | Thiago | `InteractionController.cs` (Pulso Cinético) pronto e testado; `PlayerMovement` com `OnCollisionStay` e zerando velocidade horizontal
- 2026-10-20 | Thiago | `PlayerMovement.cs` escrito no estilo Create with Code (D15, D16, D17); Shift adicionado para correr
- 2026-10-20 | Thiago | Conferido: Active Input Handling em Both; estrutura pronta para os outros clonarem
- 2026-10-20 | Thiago | Tags criadas no projeto; arquivo de contexto criado
- 2026-10-20 | Thiago | Pastas e cenas criadas; decidido não usar a pasta `_Projeto` (D8)
- 2026-10-20 | Thiago | Decidido trabalhar sem branches, todos na `main` (D5)
- 2026-10-20 | Thiago | Projeto criado na 6000.0.8f1, repositório ligado, `.gitignore` adicionado
- 2026-10-20 | Thiago | Decidido não usar o AI Assistant da Unity (D3)
- 2026-10-20 | Thiago | GDD revisado; seção 6 fica em branco até o fim (D11)
- 2026-10-20 | Thiago | Divisão do trabalho e cronograma definidos
