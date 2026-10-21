---
documento: Guia de tarefa do cenário (Nivel1)
responsavel: Arthur
projeto: Eco-Scrapper Reclamation
entrega: 2026-10-22 (quinta), antes da aula
unity: 6000.0.8f1 (Universal 3D / URP)
atualizado_em: 2026-10-21
---

# Guia Técnico: Cenário da `Nivel1` (Arthur)

> **Como usar este documento.** Este guia é a **tarefa**: diz exatamente o que entregar, onde ligar cada coisa e como saber que terminou. O `CONTEXTO_PROJETO.md` é o **contexto** (o jogo, as decisões do grupo e o estado de cada parte). Leia os dois antes de começar. Se algo aqui contradisser o contexto, vale este guia para a parte do cenário, e o contexto deve ser corrigido.

> **Estado atual do projeto (21/10, 15h).** Todo o código do jogo está pronto e testado. O personagem, as animações, o HUD e o menu também. Existe um prefab, `SIstemaDeJogo`, que traz o jogador e todos os sistemas já ligados. **Falta só o cenário da `Nivel1`, as partículas e os sons**, que são esta tarefa. O jogo já funciona de ponta a ponta na `Cena_Teste_Thiago`, com cubos no lugar do cenário: use essa cena como referência do que precisa existir.

---

## 0. Antes de começar

1. `git pull` na pasta do repositório.
2. Abrir o projeto `EcoScrapper` no Unity Hub com a versão **6000.0.8f1**. Abrir a pasta `EcoScrapper`, não a raiz do repositório.
3. Abrir a cena **`Assets/Scenes/Nivel1.unity`**. Ela é **só sua**: ninguém mais edita.
4. Dar Play na `Cena_Teste_Thiago` uma vez para ver o jogo funcionando: andar (W/S), girar (A/D), correr (Shift), pular (Espaço), Pulso Cinético (E ou botão esquerdo), coletar filtros, lodo, torre com E e vitória.

### Pode criar e editar
- `Assets/Scenes/Nivel1.unity`
- `Assets/Art/Cenario/` (texturas, materiais, modelos, sons)
- `Assets/Prefabs/Cenario/` (partículas e objetos de cenário montados)
- Pacotes importados da Asset Store (criam pastas próprias em `Assets/`)
- Scripts de editor próprios, se precisar, em `Assets/Art/Cenario/Editor/`

### Não editar
- `Assets/Scripts/` (código do jogo, pronto e documentado no GDD)
- `Assets/Prefabs/Gameplay/`, `Assets/Prefabs/Personagem/`, `Assets/Prefabs/UI/`. Arrastar esses prefabs para a cena pode; editar os arquivos deles, não.
- As outras cenas (`MenuPrincipal`, `Cena_Teste_Thiago`, `Cena_Teste_Rafael`)
- `ProjectSettings/TagManager.asset`: as tags já existem, não criar novas
- `ProjectSettings/EditorBuildSettings.asset`: a lista de cenas já está certa

### Regra técnica do projeto (importante)
Cenas (`.unity`), prefabs (`.prefab`), materiais (`.mat`), animações e arquivos `.meta` **devem ser criados pelo editor da Unity**, à mão ou por um script de editor rodado dentro da Unity. **Não escrever esses arquivos diretamente como texto.** O projeto já teve um prefab escrito como texto que veio com o ID `100100000`, reservado pela Unity, e deu erro. Depois de criar qualquer coisa: **abrir na Unity, conferir o Console sem erros vermelhos e dar Play antes do commit.**

---

## 1. O que entregar e quando está pronto

| # | Entregável | Pronto quando... |
|---|---|---|
| 1 | Terreno com os 4 setores | Dá para ir do Setor 1 ao Setor 4 andando e pulando, e o jogador não consegue sair do mapa pelas bordas |
| 2 | Texturas no terreno | Pelo menos 4 camadas pintadas: solo árido, cascalho, rocha e lodo |
| 3 | 70+ assets | A cena tem pelo menos 70 assets **diferentes**, de categorias variadas (ver Apêndice A), e a tabela do Apêndice B está preenchida |
| 4 | Entulhos com física | O Pulso Cinético (E) arremessa os entulhos |
| 5 | Poças de lodo | Pisar na poça faz a barra de HP descer |
| 6 | Pontos de spawn | Ao dar Play, aparecem 5 Bio-Filtros verdes e algumas moedas, em lugares que o jogador alcança |
| 7 | Névoa | A cena começa enevoada e a névoa some quando a torre é ativada |
| 8 | Partículas | Fumaça no cenário; pedrinhas ao usar o pulso num entulho; explosão verde ao ativar a torre |
| 9 | Sons | Som ambiente tocando; som no pulso, na coleta e na ativação da torre |
| 10 | Torre e integração | O jogo funciona do menu até a vitória na `Nivel1` (checklist da seção 5) |

---

## 2. Como fazer cada item

### 2.1 Terreno (GDD 2.3 e 4.1)
- `GameObject → 3D Object → Terrain`. Ferramentas no Inspector: **Raise or Lower Terrain** (Shift abaixa), **Set Height**, **Smooth Height**.
- **Layout sugerido:** um vale comprido ao longo do eixo Z, com os setores em sequência e paredões altos nas laterais.
  1. **Plataforma de Despertar** (início): platô elevado e seguro. É onde o jogador nasce. Precisa de um **desnível simples** (para ensinar o pulo) e de uma **barricada de caixotes leves** bloqueando a saída (para ensinar o Pulso).
  2. **Ravina Entulhada:** passagem estreita entre paredões, bloqueada por contêineres e pedras com física.
  3. **Leito Fluvial Seco:** área ampla e rebaixada no meio do vale, onde ficam as **poças de lodo**, os **barris de contrapeso** e os **pontos de spawn** dos filtros.
  4. **Base da Represa e Torre:** o fim do vale, com a **torre** e o **console**. Os **3 entulhos que bloqueiam o canal** ficam entre o Setor 3 e o 4.
- Atenção à escala: o Scrap-E tem cerca de 2 m de altura e anda a 5 m/s (8 correndo). Um nível de uns 150 a 250 m de comprimento leva de 1 a 3 minutos para ser atravessado, o que combina com o Eco-Timer de 3 minutos.

### 2.2 Texturas do terreno
- No Terrain, **Paint Texture → Edit Terrain Layers → Create Layer**.
- Mínimo de 4 camadas: **solo árido fendido, cascalho/areia, rocha, lodo tóxico**. O 2º bimestre vai exigir 10 camadas; se der para adiantar (grama, terra fértil, caminho, seixos de rio, areia, cascalho fino, musgo), melhor.

### 2.3 Assets (70+, GDD 4.1)
- Baixar pacotes **grátis** de low-poly na Unity Asset Store (rochas, deserto/cânion, industrial/sucata/barris, plantas secas e vivas). Importar por `Window → Package Manager → My Assets → Download → Import`.
- Material rosa: `Window → Rendering → Render Pipeline Converter → Material Upgrade → Convert`.
- O critério conta **assets diferentes**, não cópias: 70 modelos distintos, não 1 pedra repetida 70 vezes. Repetir para encher o cenário pode e deve; o mínimo é de 70 tipos.
- Seguir as **categorias** do GDD (Apêndice A). Os nomes não precisam ser idênticos: se o pacote tem "Rock_03" no lugar de "Pedregulho Médio Pontiagudo", serve. **Anotar no Apêndice B o que foi usado**, para o GDD ser atualizado com a lista real.
- **Organização da Hierarchy:** agrupar por setor em objetos vazios (`Setor1_Despertar`, `Setor2_Ravina`, `Setor3_LeitoSeco`, `Setor4_Represa`).
- Validar com o professor os assets de terceiros (exigência do enunciado), se ainda não foi feito.

### 2.4 Entulhos com física (Pulso Cinético)
Cada objeto que o jogador deve poder arremessar:
- **Rigidbody** (`Add Component → Rigidbody`). Massa: 15 nos tambores leves, 50 nos barris de contrapeso, de 5 a 20 nos caixotes da barricada do tutorial.
- **Collider:** Box ou Capsule, ou **Mesh Collider com Convex marcado**. Sem Convex, a Unity não deixa um Mesh Collider ter Rigidbody.
- **Tag `HeavyDebris`.** Sem essa tag, o pulso ignora o objeto.
- Rochas e estruturas que **não** devem se mexer: **sem** Rigidbody e **sem** a tag. Precisam de Collider para o jogador não atravessar.

### 2.5 Poças de lodo
- Usar o prefab pronto **`Assets/Prefabs/Gameplay/Lodo`**, que já tem **Box Collider com Is Trigger** e a **tag `ToxicSlime`**. Arrastar para a cena e posicionar.
- Se preferir um visual próprio, colocar o modelo ou plano verde **como filho** do `Lodo`, sem tirar o collider nem a tag dele.
- A poça precisa estar **na altura do chão**, encostando onde o jogador pisa.

### 2.6 Pontos de spawn
- Criar um objeto vazio `SpawnPoints` em `0, 0, 0` e, dentro dele, **8 a 12 objetos vazios** `SpawnPoint_01`, `SpawnPoint_02`...
- Espalhar pelos **Setores 2 e 3**, a uns **0,5 m acima do chão**, em lugares que o jogador alcança e fora de rochas.
- São usados pelo `SpawnManager` (seção 4, passo 4). O jogo sorteia 5 para os filtros e alguns para as moedas, sem repetir pontos.

### 2.7 Névoa
- `Window → Rendering → Lighting → aba Environment → marcar Fog`. Cor cinza-amarelada, modo **Exponential**, densidade por volta de **0.02** (ajustar no olho).
- **Tem que ser essa névoa, a do Lighting.** A torre desliga exatamente ela quando é ativada (`RenderSettings.fog = false`). Névoa feita de outro jeito (Volume, partículas) não some na vitória.

### 2.8 Partículas (`GameObject → Effects → Particle System`)
Material das partículas no URP: usar um material com o shader **`Universal Render Pipeline/Particles/Unlit`**. Salvar em `Assets/Prefabs/Cenario/` com estes nomes:

| Prefab | Uso | Configuração |
|---|---|---|
| **`Fumaca`** | Fica **na cena**: drenos, beira das poças, chaminés | **Looping ligado**, Play On Awake ligado. Cinza, Start Speed baixo (0,5 a 1), Start Size crescendo com Size over Lifetime, Color over Lifetime com alpha sumindo no fim |
| **`Particula_Pedrinhas`** | **Criada pelo código** no ponto de impacto do pulso | **Looping desligado, Play On Awake LIGADO**. Emission com **Burst** de ~20 no tempo 0, Start Speed 3 a 6, Gravity Modifier 1, Start Lifetime ~1 s, cor marrom/cinza, Shape Cone ou Sphere |
| **`Particula_Purificacao`** | **Criada pelo código** ao ativar a torre | **Looping desligado, Play On Awake LIGADO**. Shape **Sphere**, Burst de ~200, Start Speed 5 a 10, Start Lifetime ~2 s, verde claro brilhante. Opcional: segundo sistema com pétalas |

> **Play On Awake precisa estar LIGADO** nas duas partículas criadas pelo código. O script só cria o objeto (`Instantiate`) e não chama `Play()`, então a partícula tem que começar sozinha. Os guias anteriores diziam "desligado"; isso estava errado. O código também apaga a partícula de pedrinhas 2 s depois de criar.

### 2.9 Sons
Fontes grátis: **pixabay.com/sound-effects**, **freesound.org**. Salvar em `Assets/Art/Cenario/Sons/`.

| Som | Onde vai |
|---|---|
| Vento / zumbido industrial (ambiente) | `AudioSource` num objeto vazio `SomAmbiente` na `Nivel1`, com **Loop** e **Play On Awake** ligados e Spatial Blend em 0 (2D) |
| Impacto metálico | Campo `Pulse Sound` do `InteractionController` |
| Bip de coleta | Campo `Collect Sound` do `PlayerInventory` |
| Ativação / água correndo | Campo `Activation Sound` do `PurifierTower` |
| Confirmação (opcional) | Campo `Cheat Sound` do `CheatCode` |

### 2.10 Torre (GDD 2.3, setor 4)
- Arrastar o prefab **`Assets/Prefabs/Gameplay/Torre`** para o Setor 4. Ele é a **área de ativação**: Box Collider **Is Trigger** de 4 x 2 x 4, tag `PurifierTrigger` e script `PurifierTower`.
- Colocar o **modelo da torre e do console como filhos** do objeto `Torre`, de forma que a área de ativação fique **na frente do console**. Não tirar o collider, a tag nem o script do objeto `Torre`.
- Opcional, para cumprir o GDD (os 3 entulhos que bloqueiam o canal): criar um cubo vazio de collider (Is Trigger, sem renderer) cobrindo o canal, chamado `AreaCanal`. No `PurifierTower`, arrastar esse cubo para **Channel Area** e os 3 entulhos para **Blocking Debris**. Com isso, a torre só ativa quando os 3 saem do canal. Se esses campos ficarem vazios, a regra é ignorada.

---

## 3. Onde ligar cada coisa (campos no Inspector)

Todos estes campos ficam **na cópia do `SIstemaDeJogo` que está na `Nivel1`** ou na `Torre` da cena. Basta preencher na cena. **Não** é preciso aplicar no prefab.

| Objeto na cena | Componente | Campo | O que arrastar |
|---|---|---|---|
| `SIstemaDeJogo/Player` | Interaction Controller | Impact Particle Prefab | `Prefabs/Cenario/Particula_Pedrinhas` |
| `SIstemaDeJogo/Player` | Interaction Controller | Pulse Sound | som de impacto metálico |
| `SIstemaDeJogo/Player` | Player Inventory | Collect Sound | som de coleta |
| `SIstemaDeJogo/Player` | Player Health | Death Effect Prefab (opcional) | uma partícula de faíscas, se fizer |
| `SIstemaDeJogo/Player` | Cheat Code | Cheat Sound (opcional) | som de confirmação |
| `SIstemaDeJogo/SpawnManager` | Spawn Manager | Filter Spawn Points | os `SpawnPoint_XX` da `Nivel1` |
| `SIstemaDeJogo/SpawnManager` | Spawn Manager | Scrap Spawn Points | os mesmos `SpawnPoint_XX` |
| `Torre` | Purifier Tower | Purification Effect Prefab | `Prefabs/Cenario/Particula_Purificacao` |
| `Torre` | Purifier Tower | Effect Point (opcional) | um objeto vazio no topo da torre (onde a explosão sai) |
| `Torre` | Purifier Tower | Activation Sound | som de ativação / água |
| `Torre` | Purifier Tower | Channel Area / Blocking Debris (opcional) | ver 2.10 |

**Arrastar sempre da Hierarchy os objetos da cena** (pontos de spawn, Effect Point) e da **aba Project os arquivos** (prefabs de partícula, sons).

---

## 4. Integração do `SIstemaDeJogo` na `Nivel1` (passo a passo)

1. Na `Nivel1`, **apagar a `Main Camera`** que veio com a cena. O prefab já traz a câmera do jogador. **Manter a `Directional Light`.**
2. Arrastar **`Assets/Prefabs/Gameplay/SIstemaDeJogo`** para a Hierarchy e conferir se ficou em `0, 0, 0`.
3. Selecionar o **`Player`** dentro dele e posicionar no **Setor 1**, em cima do platô, uns 1 m acima do chão (o centro da cápsula fica no meio do corpo).
4. No **`SpawnManager`** dentro do `SIstemaDeJogo`:
   - **Filter Spawn Points:** pôr o tamanho igual ao número de pontos e arrastar todos os `SpawnPoint_XX`. Dica: selecionar todos na Hierarchy e soltar de uma vez em cima do nome do campo.
   - **Scrap Spawn Points:** os mesmos pontos.
   - **Filters To Spawn:** 5. **Scrap To Spawn:** número de pontos menos 5.
5. Arrastar a **`Torre`** para o Setor 4 e as **poças `Lodo`** para o Setor 3.
6. Ligar as partículas e os sons conforme a seção 3.
7. Ctrl+S.

> **Câmera:** é filha do `Player` e segue o robô por trás. Se ela entrar dentro de paredes em lugares estreitos, alargar a passagem é mais simples do que mexer na câmera.

---

## 5. Checklist final (testar antes do commit)

Abrir a **`MenuPrincipal`** e dar Play. O Play da Unity roda a cena que está aberta no editor, então, para testar o fluxo completo, a cena aberta tem que ser o menu.

- [ ] Console sem erros vermelhos ao abrir a `Nivel1` e ao dar Play
- [ ] Menu aparece; o botão **Iniciar** carrega a `Nivel1`
- [ ] O robô nasce no Setor 1, em cima do chão, e a câmera mostra o robô por trás
- [ ] Névoa visível
- [ ] Andar, correr, girar, pular e as animações trocando
- [ ] A barricada do tutorial sai com o Pulso (E), com **pedrinhas** e **som**
- [ ] 5 Bio-Filtros verdes girando no mapa, todos alcançáveis; o HUD conta "Bio-Filtros: X / 5"
- [ ] Moedas de sucata coletáveis; o HUD conta a sucata; som de coleta
- [ ] Poça de lodo tira HP; ficar até zerar dá **Game Over**; **Reiniciar** recomeça e os filtros mudam de lugar
- [ ] Esc pausa; **Continuar** volta
- [ ] Com os 5 filtros, E na torre: **explosão verde**, **som**, **névoa some** e, 2 s depois, **tela de vitória com pontuação**
- [ ] O jogador não cai para fora do mapa em nenhum lugar
- [ ] Contagem de assets diferentes na cena ≥ 70 (Apêndice B preenchido)

---

## 6. Ao terminar

1. Preencher o **Apêndice B** com os assets usados.
2. Atualizar o `CONTEXTO_PROJETO.md`: marcar como feitos os itens do Arthur na seção 9 (Status) e adicionar uma linha no registro de alterações (seção 12), com data e autor.
3. Commit e push **direto na `main`** (o grupo decidiu não usar branches, decisão D5 no contexto):
   ```bash
   git pull
   git add .
   git status   # NÃO pode aparecer Library/ nem Temp/
   git commit -m "Cenário da Nivel1: terreno, assets, partículas e sons"
   git push
   ```
4. Avisar o grupo, porque o Thiago atualiza a lista de assets do GDD a partir do Apêndice B.

---

## Apêndice A: os 72 assets do GDD (seção 4.1), por categoria

Referência de **categorias e ideias**. Não precisa ter cada item exato; precisa ter **70+ assets diferentes** que cubram essas categorias. Itens marcados com ✅ já existem no projeto.

**Categoria 1: Terreno, Geologia e Formações Rochosas (20)**
1. Rocha Modular de Falésia Grande A
2. Rocha Modular de Falésia Grande B
3. Pedregulho Médio Pontiagudo (com colisor físico)
4. Pedregulho Médio Arredondado
5. Laje de Pedra Chata (Plataforma natural)
6. Conjunto de Seixos e Lascas de Pedra Pequenas
7. Arco Rochoso Natural de Entrada
8. Paredão de Cânion Desgastado
9. Coluna de Rocha Basáltica
10. Bloco Rochoso Quebrável de Atalho
11. Rampa de Terra Batida
12. Talude de Cascalho Seco
13. Textura de Solo Árido Fendido (Material PBR)
14. Textura de Cascalho e Areia Grossa
15. Textura de Rocha Sedimentar
16. Textura de Lodo Tóxico Químico (Emissivo)
17. Textura de Terra Fértil e Gramado (Pós-Restauração)
18. Malha de Calha Fluvial Seca
19. Margem de Seixos de Rio
20. Crateras de Erosão do Solo

**Categoria 2: Flora Árida e Bioativa (15)**
21. Árvore Reta Petrificada e Seca
22. Árvore Retorcida Caída (Obstáculo transponível)
23. Toco de Tronco Oco Apodrecido
24. Raiz Gigante Exposta e Ressecada
25. Tufo de Arbusto Seco Espinhoso
26. Ramo de Espinheiro Rasteiro
27. Arbusto Queimado por Ácido
28. Muda Bioativa de Caule Bioluminescente (Estágio Broto)
29. Flor Purificadora de Esporos Azuis (Surgimento dinâmico)
30. Moita de Grama Verdejante Alta (Pós-Restauração)
31. Conjunto de Trepadeiras Vivas de Encosta
32. Cogumelo Filtrador de Solo
33. Líquen Musgoso Fluorescente
34. Aglomerado de Juncos Aquáticos
35. Arbusto Frutífero Restaurador

**Categoria 3: Resíduos Industriais e Sucatas com Física (18)**
36. Tambor de Óleo Amassado (Rigidbody, massa 15)
37. Barril Químico de Contrapeso (Rigidbody, massa 50)
38. Caçamba de Metal Enferrujada Grande
39. Duto Metálico Cilíndrico Quebrado A (Pedaço Curvo)
40. Duto Metálico Quebrado B (Pedaço Reto transponível)
41. Caixa Industrial de Madeira Compensada (Quebrável)
42. Caixote de Aço Reforçado de Bloqueio
43. Viga Estrutural I-Beam Torcida
44. Engrenagem Mecânica Desgastada de 1.5m
45. Eixo de Roda Dentada Pequena
46. Rolo de Cabos Elétricos de Cobre Desencapados
47. Pneu Industrial Tratorado Descartado
48. Placa Metálica Corrugada de Entulho
49. Chapa de Aço para Gangorra de Contrapeso
50. Carcaça de Gerador a Óleo Desativado
51. Tanque de Pressão Esférico Furado
52. Pedaço de Escada de Incêndio Torcida
53. Sucata Metálica Reciclável (coletável `ScrapCoin`) ✅ prefab `SucataCoin`

**Categoria 4: Infraestrutura, Hidráulica e Mecanismos (10)**
54. Torre Purificadora Central (Estrutura Bioenergética)
55. Console / Terminal Interativo da Torre
56. Comporta Hidráulica de Concreto da Represa
57. Roda / Válvula Mecânica de Rotação Manual
58. Passarela de Grade de Aço Industrial
59. Pilar de Sustentação de Concreto Armado
60. Corrimão Metálico de Proteção Desgastado
61. Tubo Central de Drenagem Pluvial
62. Caixa de Distribuição Elétrica Quebrada
63. Poste de Sinalização Industrial Inclinado

**Categoria 5: Personagens, Entidades Vivas e Fauna (9)**
64. Protagonista Scrap-E ✅ prefab `ScrapE`
65. Terminal Holográfico / Drone ARIA
66. Lagarto das Rochas (Sand-Gecko)
67. Pássaro Canyon Swift
68. Lebre do Prado (Eco-Hare)
69. Besouro-Blindado de Carapaça Metálica
70. Peixe de Água Doce Cintilante
71. Easter Egg: Esteira de Lagarta e Bota com Broto
72. Easter Egg: Extintor de Incêndio Vintage Enferrujado

Também já existe no projeto: o Bio-Filtro (prefab `BioFiltro`).

---

## Apêndice B: assets usados na `Nivel1` (preencher)

| # | Nome do asset (como está no projeto) | Pacote de origem | Categoria (1 a 5) | Setor(es) |
|---|---|---|---|---|
| 1 | | | | |
| 2 | | | | |
| ... | | | | |
