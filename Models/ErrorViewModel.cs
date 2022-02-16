using System;

namespace JOE.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}

:- abolir(cacador/3).
:- abolir(wumpus/2).
:- abolir(poco/2).
:- abolir(ouro/2).
:- abolir(agarrar/2).
:- abolir(acoes/1).
:- abolir(visitou/2).
:- abolir(filmado/2).
:- abolir(runloop/1).

:- dinamico([
  cacador/3
  wumpus/2
  poco/2
  ouro/2
  pegar/2
  acoes/1,
  atirou/2,
  visitado/2
]).

% Define a matriz mundial NxM.
mundo(4 , 4 ).

% Posicao inicial do jogador
cacador(1 , 1 , leste ).
visitados(1 , 1 ).

% ----------------------------%
% De predicados do ambiente%
% ----------------------------%
has_gold(sim ):- pegue(X , Y ), ouro(X , Y )! .
has_gold(nao ).

has_arrows(nao ):- filmado(_ , _ ),! .
has_arrows(sim ).

% De percepcoes
% ===========
% Se tem ouro, tem glitter.
has_glitter(sim ):- has_gold(G ), G  ==  nao , cacador(X , Y , _ ), ouro(X , Y )! .
has_glitter(nao ).

% Senses brisa se o bloco adjacente tiver um poco.
has_breeze(yes ):-
  cacador(X , Y , _ ), N is Y  +  1 , cova(X , N )! ;
  cacador(X , Y , _ ), S is Y  -  1 , cova(X , S )! ;
  cacador(X , Y , _ ), E is X  +  1 , cova(E , Y )! ;
  cacador(X , Y , _ ), W is X  -  1 , cova(W , Y )! .
has_breeze(nao ).

% Sentidos fede se o bloco adjacente tiver o wumpus.
has_stench(sim ):-
  cacador(X , Y , _ ), N is Y  +  1 , wumpus(X , N )! ;
  cacador(X , Y , _ ), S is Y  -  1 , wumpus(X , S )! ;
  cacador(X , Y , _ ), E is X  +  1 , wumpus(E , Y )! ;
  cacador(X , Y , _ ), W is X  -  1 , wumpus(W , Y )! .
has_stench(nao ).

% Sentidos colidem se está de frente para uma parede
has_bump(sim ):-
  mundo(W , _ ), cacador(W , _ , leste )  ! ;
  mundo(_ , H ), cacador(_ , H , norte )! ;
  cacador(1 , _ , oeste )  ! ;
  cacador(_ , 1 , sul )! .
has_bump(nao ).

% Senses screm se wumpus morreram
has_scream(sim ):- is_wumpus(morto )! .
has_scream(nao ).

% Verificar o estado do jogador
is_player(morto ):- cacador(X , Y , _ ), wumpus(X , Y )! .
is_player(morto ):- cacador(X , Y , _ ), poco(X , Y )    ! .
is_player(vivo ).

% Verificar a condicao de Wumpus
is_wumpus(morto ):- atirou(X , Y ), wumpus(X , Y )! .
is_wumpus(vivo ).

% Verifique se a posicao está nos limites do mapa.
in_bounds(X , Y ):-
  mundo(W , H ),
  X  >  0 , X  = <  W ,
  Y  >  0 , Y  = <  H .

% Retorna as porcentagens atuais
percepcoes([ fedor , brisa , brilho , galo , grito ]):-
  has_stench(Stench ), has_breeze(brisa ), has_glitter(brilho ),
  has_bump(Bump ), has_scream(Grito )! .

% Move o Player para uma nova posicao.
mover(X , Y ):-
  assertz(acoes(movimento )),
  in_bounds(X , Y ),
  % format("- Movendo para~dx~d~n", [X, Y]),
  direcao(X , Y , D ),
  retractall(cacador(_ , _ , _ )),
  asserta(cacador(X , Y , D )),
  assertz(visitado(X , Y )),
  ! .
move(X , Y ):- format('!: Nao pode mover para~dx~d~n' , [ X , Y ]).

% Obter a direcao
direcao(X , Y , D ):- cacador(Xi , Yi , _ ), X  >  Xi , Y  ==  Yi , D  =  leste ,    ! .
direcao(X , Y , D ):- cacador(Xi , Yi , _ ), X  ==  Xi , Y  <  Yi , D  =  norte ,   ! .
direcao(X , Y , D ):- cacador(Xi , Yi , _ ), X  <  Xi , Y  ==  Yi , D  =  oeste ,    ! .
direcao(X , Y , D ):- cacador(Xi , Yi , _ ), X  ==  Xi , Y  >  Yi , D  =  sul ,   ! .
direcao(_ , _ , D ):- cacador(_ , _ , D ).

% Atirar em determinada posicao
shoot(_ , _ ):- has_arrows(no ), write('!: Eu nao tenho mais setas.' ),! .
atirar(X , Y ):-
  assertz(acoes(shoot )),
  has_arrows(sim ),
  assertz(filmado(X , Y )).

% Obtism todos os blocos adjacentes
vizinhos(N ):-  findall([ X , Y ], vizinhos(X , Y ), N ).

% Definir os blocos adjacentes
vizinhos(X , Y ):- cacador(Xi , Yi , _ ), E is Xi + 1 , in_bounds(E , Yi ), X is E ,   Y is Yi .
vizinhos(X , Y ):- cacador(Xi , Yi , _ ), N is Yi + 1 , in_bounds(Xi , N ), X is Xi , Y is N .
vizinhos(X , Y ):- cacador(Xi , Yi , _ ), W is Xi - 1 , in_bounds(W , Yi ), X is W ,   Y is Yi .
vizinhos(X , Y ):- cacador(Xi , Yi , _ ), S is Yi - 1 , in_bounds(Xi , S ), X is Xi , Y is S .

% De acoes do jogador
acao(saída ):-  escreva('Tchau, tchau!' ), nl , print_result , nl , print_world , nl , halt .

acao([ movimento ,   X , Y ]):- move(X , Y ).
acao([ atirar , X , Y ]):- atirar(X , Y ).

acao(agarrar ):-
  assertz(acoes(agarrar )),
  cacador(X , Y , _ ), assertz(agarrar(X , Y )),
 (ouro(X , Y ), tem_gold(nao ))->
    escreva('!: Encontrado ouro!' ), nl ;
    verdade .

% Um movimento aleatorio ingênuo
acao(aleatória ):-
  vizinhos(N ), comprimento(N , L ), aleatorio_entre(1 , L , R ), nth1(R , N , [ X , Y ]),
  mover(X , Y ).

acao(noop ).

% Ponto
pontuacao(S ):-  findall(P , pontos(P ), Ps ), sum_list(Ps , S ).

Os pontos(P ):- os passos(T ),         P is - t .
pontos(P ):- is_player(morto ), P is - 1000 .
pontos(P ):- has_gold(sim ),    P is + 1000 .

passos(S ):-  findall(A , acoes(A ), As ), comprimento(As , S ).

% Impressao
print_result  :-
  formato('~n~tResult~t~40 |~n' ),
  pontuacao(S ), passos(T ),
  format('Passos:~`.t~d~40 |' , [ T ]), nl ,
  format('Pontuacao:~`.t~d~40 |' , [ S ]), nl ,
 (has_gold(sim ), cacador(1 , 1 , _ ))->
    format('Resultado:~`.t~p~40 |' , [ win ]), nl ;
    format('Resultado:~`.t~p~40 |' , [ solto ]), nl .

print_world  :-
  is_player(P ), cacador(Hx , Hy , _ ),
  format('~n~tWorld~t~40 |~n' ),
  format('Player:~`.t~p em~dx~d~40 |' , [ P , Hx , Hy ]), nl ,
  is_wumpus(W ), wumpus(Wx , Wy ),
  format('Wumpus:~`.t~p em~dx~d~40 |' , [ W , Wx , Wy ]), nl ,
  has_gold(G ), ouro(Gx , Gy ),
  formato('Gold:~`.t~p em~dx~d~40 |' , [ G , Gx , Gy ]), nl ,
  findall([ Px , Py ], poco(Px , Py ), Ps ),
  format('Pit:~`.t~p~40 |' , [ Ps ]), nl .

% Execute o jogo
executado(aleatorio ):-
  random_between(2 , 5 , X1 ), random_between(2 , 5 , Y1 ), assertz(ouro(X1 , Y1 )),
  random_between(2 , 5 , X2 ), aleatorio_entre(2 , 5 , Y2 ), assertz(wumpus(X2 , Y2 )),
  random_between(2 , 5 , X3 ), random_between(2 , 5 , Y3 ), assertz(pit(X3 , Y3 )),
  random_between(2 , 5 , X4 ), random_between(2 , 5 , Y4 ), assertz(pit(X4 , Y4 )),
  random_between(2 , 5 , X5 ), aleatorio_entre(2 , 5 , Y5 ), assertz(poco(X5 , Y5 )),
  correr .

run([ Gx , Gy ], [ Wx , Wy ], [ P1x , P1a ], [ P2x , P2y ], [ P3x , P3y ]):-
  assertz(ouro(Gx , Gy )),
  assertz(wumpus(Wx , Wy )),
  assertz(poco(P1x , P1y )),
  assertz(poco(P2x , P2y )),
  assertz(poco(P3x , P3y )),
  correr .

execute  :- runloop(0 ).

runloop(200 ):-  write('!: Máximo de movimentos permitidos alcancados.' ), nl , action(exit ),! .
runloop(T ):-
  cacador(X , Y , D ), percepcoes(P ),
  formato('~d: At~dx~d voltado para~p, sentidos~p.' , [ T , X , Y , D , P ]),
  heurística(P , A ),
  formato('I \' m  fazendo~p .~n ', [ A ]),
  acao(A ),
  % Iterar
  is_player(morto )->(
    escreva('Você morreu' ), nl ,
    acao(saída ),
    !);
  Ti  is  T + 1 ,
  runloop(Ti ).
