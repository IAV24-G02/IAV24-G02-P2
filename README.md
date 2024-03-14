# IAV - Práctica 2: Navegación

> Hay quien implementa el A* con una estructura de registro de nodo muy simple (el identificador del nodo y el coste f), sólo usa lista de apertura, se apoya en tener toda la información completa del grafo a mano (costes incluidos) y como estructura de datos auxiliar usa una cola de prioridad muy simple.  
Según el pseudocódigo que plantea Millington, la estructura de registro de nodo es más rica (identificador del nodo, conexión con el nodo padre, coste g y coste f), se usa una lista de apertura y una lista de cierre, no se asume que toda la información del grafo esté disponible y la cola de prioridad se implementa con BinaryHeap.

## Autores
- Yi (Laura) Wang Qiu [GitHub](https://github.com/LauraWangQiu)
- Agustín Castro De Troya [GitHub](https://github.com/AgusCDT)
- Ignacio Ligero Martín [GitHub](https://github.com/theligero)
- Alfonso Jaime Rodulfo Guío [GitHub](https://github.com/ARodulfo)

## Propuesta
Este proyecto es una práctica de la asignatura de Inteligencia Artificial para Videojuegos del Grado en Desarrollo de Videojuegos de la UCM, cuyo enunciado original es este: [El secreto del laberinto](https://narratech.com/es/inteligencia-artificial-para-videojuegos/navegacion/el-secreto-del-laberinto/).

Esta práctica consiste en recrear un prototipo basado en el mito griego del hilo de Ariadna. La idea es que el jugador controle a `Teseo`, que debe encontrar la salida del laberinto, mientras es perseguido por el `Minotauro`. El laberinto es un grafo que se genera aleatoriamente y el `Minotauro` debe seguir un camino que le lleve a `Teseo`. Si `Teseo` es atrapado por el `Minotauro`, el nivel se reinicia.

## Punto de partida

Se parte de un proyecto base de **Unity 2022.3.5f1** proporcionado por el profesor y disponible en este repositorio: [IAV-Navegación](https://github.com/Narratech/IAV-Navegacion)

| Clases: AGENT | Información |
| - | - |
| Agente | Explicado en la [Práctica 1: Movimiento](https://github.com/IAV24-G02/IAV24-G02-P1) |
| Comportamiento Agente | Explicado en la [Práctica 1: Movimiento](https://github.com/IAV24-G02/IAV24-G02-P1) |
| Direccion | Explicado en la [Práctica 1: Movimiento](https://github.com/IAV24-G02/IAV24-G02-P1) |

| Clases: ANIMATION | Información |
| - | - |
| Animal Animation Controller | Explicado en la [Práctica 1: Movimiento](https://github.com/IAV24-G02/IAV24-G02-P1) |
| Camera Follow | Explicado en la [Práctica 1: Movimiento](https://github.com/IAV24-G02/IAV24-G02-P1) |
| Player Animator | Explicado en la [Práctica 1: Movimiento](https://github.com/IAV24-G02/IAV24-G02-P1) |

| Clases: ANIMATION | Información |
| - | - |
| Control Jugador | Explicado en la [Práctica 1: Movimiento](https://github.com/IAV24-G02/IAV24-G02-P1) |
| Llegada | Explicado en la [Práctica 1: Movimiento](https://github.com/IAV24-G02/IAV24-G02-P1) |
| Merodear | Explicado en la [Práctica 1: Movimiento](https://github.com/IAV24-G02/IAV24-G02-P1) |

| Clases: ANIMATION | Información |
| - | - |
| Mino Collision | OnCollisionEnter(Collision collision): En una colisión, por DuckTyping, detecta si se trata del `Teseo` y si es el caso, reinicia el nivel. |
| Mino Evader | OnTriggerEnter(Collider other): En una colisión con un _trigger_, por DuckTyping, detecta si se trata del `Minotauro` y si también tiene el componente **Seguir Camino**, se resetea su camino. |
| Mino Manager | Busca en la escena si existe el GameObject "GraphGrid", coge referencia a su componente **GraphGrid** y genera un número de `Minotauros` en posiciones aleatorias. |
| Seguir Camino | La clase "SeguirCamino", se encarga de hacer uso del grafo de Teseo que indica el camino que automáticamente debe seguir Teseo cuando su moivimiento no es establecido por el jugador.
La clase elige el siguiente nodo del grafo y actualiza la dirección del avatar en función de la posicion de este. Además aporta un método para resetear el grafo de Teseo. |
| Slow | Esta clase se encarga de ralentizar al avatar cuando se encuentra dentro del trigger del minotauro. Para indentificar si lo que entre y/o sale dentro del collider es el avatar, chequea si el Game Object tiene el componente
"ControlJugador", y en caso afirmativo se da por hecho que es el avatar. Cuando el avatar entre en el collider, la velocidad máxima del componente Agente del avatar es reducida a uno, y al salir del collider, la velocidad máxima es 
devuelta a su valor original.  |
| Teseo | Esta clase se encarga de altenar entre los dos posibles comportamientos del avatar. Tiene variables para almacenar sendas referencias a los componentes de "SeguirCamino" y de "ControlJugador" que el avatar tiene incorporados.
Si el jugador presiona la tecla espacio, la clase Teseo se encarga de hacerque el avatar siga un camino automáticamente creado con el algoritmo A*. En caso contrario, el jugador tendrá el control del avatar. |

| Clases: EXTRA | Información |
| - | - |
| Binary Heap | Clase montículo binario para C#. Es una estructura de datos en forma de árbol binario balanceado. Tiene un tamaño predeterminado, un array de tipo `T` del tamaño previamente mencionado, un entero de elementos en uso, otro entero del tamaño que se inicializa con el tamaño predeterminado y un booleano que determina si está internamente ordenado. Además, cuenta con métodos para eliminar todos los elementos del montículo, o agregar / eliminar elementos o decir si tiene o no un elemento concreto. |
| DropDown | Clase que se subdivide en dos casos concretos: en si el booleano mino está activo o no. Mientras que en ambos casos se añade un _listener_ al GameObject que contiene el componente Dropdown, en el caso de que mino no esté activo se llama al método `changeSize()` y en el caso de que sí esté activo se llama al método `setNumNinos()`. En resumen, este método se utiliza dentro del menú de inicio para mandarle al GameManager los parámetros que el jugador ha seleccionado previamente, como es el caso del tamaño del mapa o el número de minotauros. |

| Clases: GRAPH | Información |
| - | - |
| Graph | Aquí implementaremos la mayoría de lo que se pide en la práctica. Contiene listas para representar el grafo y métodos para obtener los nodos vecinos y sus costes. Además un método para BuilPath() para reconstruir el camino que da la vuelta a los nodos anotados. |
| Graph Grid | Script que genera el mapa, sus métodos principales son: LoadMap(string filename) que lee el mapa de archivo, genera el terreno y lee los nodos vecinos; SetNeighbours(int x, int y, bool get8 = false)el cuál coloca los nodos vecinos en el terreno; GetNearestVertex(Vector3 position)que mientras haya nodos en la cola, busca el nodo más próximo; GetRandomPos() devuelve posición aleatoria del mapa; UpdateVertexCost(Vector3 position, float costMultiplier) actualiza el coste de los nodos; WallInstantiate(Vector3 position, int i, int j) se encarga de colocar en escena todos los muros del escenario. |
| Theseus Graph | Gestiona qué algoritmo utilizar. Inicializa objetos para la cámara, el Avatar y el hilo que se dibujará al hacer el Avatar el camino, además de las propiedades del propio hilo(anchura). Su método Update() controla si se pulsa la tecla "Espacio" la cuál activa el hilo, y en otro caso lo desactiva. Si se pulsa la tecla "S" activa/desactiva el camino suavizado. Luego si el hilo está activado elige el algoritmo a utilizar, comprueba si estará suavizado y dibuja el hilo(llamada a DibujaHilo()); OnDrawGizmos() que dibuja las esferas en las baldosas y las líneas del hilo; ShowPathVertices(List<Vertex> path, Color color)el cuál recorre el camino mostrándolo de un color; GetNodeFromScreen(Vector3 screenPosition)que mediante RayCast traduce las posiciones en la pantalla a los nodos; DibujaHilo() setea las posiciones donde el hilo de Ariadna se dibujará; updateAriadna(bool ar) según el valor del booleano activa/desactiva el hilo; ChangeHeuristic()que cambia la heurística del algoritmo; ResetPath() pone a null el camino. |
| Vertex | Clase para representar los vértices. Contiene métodos para comparar los costes entre vértices o para saber si un vérice es igual a otro. |
| Clases: GAMEMANAGER | Información |
| - | - |
| GameManager | Clase que gestiona todo lo relativo a la propia partida. En primer lugar está la interfaz gráfica, que durante la sección del menú principal, muestra un menú de configuración para establecer el tipo de mapa (10x10, 20x20 o 30x30) y la cantidad de minotauros en la partida. A continuación, en base a los datos previamente pedidos al usuario, se carga una de las tres escenas y guarda el valor del número de minotauros que van a aparecer en la partida. Una vez ésta se ha iniciado, se coloca una interfaz gráfica en la esquina superior izquierda donde aparezca la tasa de fotogramas por segundo y diversas configuraciones que se pueden habilitar a través de distintas teclas del teclado (como el cambio de tasa de fotogramas). Tiene distintos métodos de tipo "getters" (como el tamaño del laberinto, el número de minotauros o el jugador) y "setters" (como la zona de inicio y fin del laberinto o el número de minotauros), así como un buscador de GameObjects, reinicio de escena, carga de escenas... |

## Diseño de la solución

C.

D.

## Pruebas y métricas

| Pruebas | Métricas | Links |
|:-:|:-:|:-:|
| **Característica A** | | |
| | | _link no disponible_ |

| Pruebas | Métricas | Links |
|:-:|:-:|:-:|
| **Característica B** | | |
| | | _link no disponible_ |

| Pruebas | Métricas | Links |
|:-:|:-:|:-:|
| **Característica C** | | |
| | | _link no disponible_ |

| Pruebas | Métricas | Links |
|:-:|:-:|:-:|
| **Característica D** | | |
| | | _link no disponible_ |

| Pruebas | Métricas | Links |
|:-:|:-:|:-:|
| **Característica E** | | |
| | | _link no disponible_ |

## Ampliaciones

No se han realizado ampliaciones hasta el momento.

## Producción

Las tareas se han realizado y el esfuerzo ha sido repartido entre los autores. Observa la tabla de abajo para ver el estado y las fechas de realización de las mismas. Puedes visitar nuestro proyecto de GitHub en el siguiente [link](https://github.com/orgs/IAV24-G02/projects/2/).

| Estado  |  Tarea  |  Fecha  |  
|:-:|:--|:-:|
|  | Diseño: Primer borrador | ..-..-2024 |
|  | Característica A | ..-..-2024 |
|  | Característica B | ..-..-2024 |
|  | Característica C | ..-..-2024 |
|  | Característica D | ..-..-2024 |
|  | Característica E | ..-..-2024 |
|  | **OTROS** | |
|  |  | ..-..-2024 |
|  |  **OPCIONALES**  | |
|  |  | ..-..-2024 |
|  |  | ..-..-2024 |

## Licencia

Yi (Laura) Wang Qiu, Agustín Castro De Troya, Ignacio Ligero Martín, Alfonso Jaime Rodulfo Guío, autores de la documentación, código y recursos de este trabajo, concedemos permiso permanente a los profesores de la Facultad de Informática de la Universidad Complutense de Madrid para utilizar nuestro material, con sus comentarios y evaluaciones, con fines educativos o de investigación; ya sea para obtener datos agregados de forma anónima como para utilizarlo total o parcialmente reconociendo expresamente nuestra autoría.

Una vez superada con éxito la asignatura se prevee publicar todo en abierto (la documentación con licencia Creative Commons Attribution 4.0 International (CC BY 4.0) y el código con licencia GNU Lesser General Public License 3.0).

## Referencias

Los recursos de terceros utilizados son de uso público.

- *AI for Games*, Ian Millington.
- [Kaykit Medieval Builder Pack](https://kaylousberg.itch.io/kaykit-medieval-builder-pack)
- [Kaykit Dungeon](https://kaylousberg.itch.io/kaykit-dungeon)
- [Kaykit Animations](https://kaylousberg.itch.io/kaykit-animations)

- Unity 2018 Artificial Intelligence Cookbook, Second Edition (Repositorio)
https://github.com/PacktPublishing/Unity-2018-Artificial-Intelligence-Cookbook-Second-Edition 
- Unity Artificial Intelligence Programming, 5th Edition (Repositorio)
https://github.com/PacktPublishing/Unity-Artificial-Intelligence-Programming-Fifth-Edition : Unity Artificial Intelligence Programming – Fifth Edition, published by Packt (github.com)
