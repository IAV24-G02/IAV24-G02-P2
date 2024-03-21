# IAV - Práctica 2: Navegación

> Hay quien implementa el A* con una estructura de registro de nodo muy simple (el identificador del nodo y el coste f), sólo usa lista de apertura, se apoya en tener toda la información completa del grafo a mano (costes incluidos) y como estructura de datos auxiliar usa una cola de prioridad muy simple.  
Según el pseudocódigo que plantea Millington, la estructura de registro de nodo es más rica (identificador del nodo, conexión con el nodo padre, coste g y coste f), se usa una lista de apertura y una lista de cierre, no se asume que toda la información del grafo esté disponible y la cola de prioridad se implementa con BinaryHeap.

## Autores
- Yi (Laura) Wang Qiu [GitHub](https://github.com/LauraWangQiu)
- Agustín Castro De Troya [GitHub](https://github.com/AgusCDT)
- Ignacio Ligero Martín [GitHub](https://github.com/theligero)
- Alfonso Jaime Rodulfo Guío [GitHub](https://github.com/ARodulfo)

## Propuesta
Este proyecto es una práctica de la asignatura de Inteligencia Artificial para Videojuegos del Grado en Desarrollo de Videojuegos de la UCM, cuyo enunciado original es este: [El Secreto del Laberinto](https://narratech.com/es/inteligencia-artificial-para-videojuegos/navegacion/el-secreto-del-laberinto/).

Esta práctica consiste en recrear un prototipo basado en el mito griego del hilo de Ariadna. La idea es que el jugador controle a `Teseo`, que debe encontrar la salida del laberinto, mientras es perseguido por el `Minotauro` si es percibido. Si `Teseo` es atrapado por el `Minotauro`, el nivel se reinicia.

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
| Seguir Camino | Hace uso del grafo de Teseo que indica el camino que automáticamente debe seguir `Teseo` cuando su movimiento no es establecido por el jugador. La clase elige el siguiente nodo del grafo y actualiza la dirección del `Teseo` en función de la posicion de este. Además aporta un método para resetear el grafo de Teseo. |
| Slow | Ralentiza al `Teseo` cuando se encuentra dentro del trigger del `Minotauro`. Para indentificar si lo que entre y/o sale dentro del collider es el `Teseo`, chequea si el Game Object tiene el componente "ControlJugador", y en caso afirmativo se da por hecho que es el `Teseo`. Cuando el `Teseo` entre en el collider, la velocidad máxima del componente Agente del `Teseo` es reducida, y al salir del collider, la velocidad máxima es devuelta a su valor original. |
| Teseo | Esta clase se encarga de altenar entre los dos posibles comportamientos del `Teseo`. Tiene variables para almacenar sendas referencias a los componentes de "SeguirCamino" y de "ControlJugador" que el `Teseo` tiene incorporados. Si el jugador presiona la tecla `"Espacio"`, la clase Teseo se encarga de hacer que el `Teseo` siga un camino automáticamente creado con el algoritmo A*. En caso contrario, el jugador tendrá el control del `Teseo`. |

| Clases: EXTRA | Información |
| - | - |
| Binary Heap | Clase montículo binario para C#. Es una estructura de datos en forma de árbol binario balanceado. Tiene un tamaño predeterminado, un array de tipo `T` del tamaño previamente mencionado, un entero de elementos en uso, otro entero del tamaño que se inicializa con el tamaño predeterminado y un booleano que determina si está internamente ordenado. Además, cuenta con métodos para eliminar todos los elementos del montículo, o agregar / eliminar elementos o decir si tiene o no un elemento concreto. |
| DropDown | Clase que se subdivide en dos casos concretos: en si el booleano mino está activo o no. Mientras que en ambos casos se añade un _listener_ al GameObject que contiene el componente Dropdown, en el caso de que mino no esté activo se llama al método `changeSize()` y en el caso de que sí esté activo se llama al método `setNumNinos()`. En resumen, este método se utiliza dentro del menú de inicio para mandarle al GameManager los parámetros que el jugador ha seleccionado previamente, como es el caso del tamaño del mapa o el número de minotauros. |

| Clases: GRAPH | Información |
| - | - |
| Graph | Aquí implementaremos la mayoría de lo que se pide en la práctica. Contiene listas para representar el grafo y métodos para obtener los nodos vecinos y sus costes. Además un método para `BuildPath()` para reconstruir el camino que da la vuelta a los nodos anotados. |
| Graph Grid | Genera el mapa, sus métodos principales son: `LoadMap(string filename)` que lee el mapa de archivo, genera el terreno y lee los nodos vecinos; `SetNeighbours(int x, int y, bool get8 = false)` el cuál coloca los nodos vecinos en el terreno; `GetNearestVertex(Vector3 position)` que mientras haya nodos en la cola, busca el nodo más próximo; `GetRandomPos()` devuelve posición aleatoria del mapa; `UpdateVertexCost(Vector3 position, float costMultiplier)` actualiza el coste de los nodos; `WallInstantiate(Vector3 position, int i, int j)` se encarga de colocar en escena todos los muros del escenario. |
| Theseus Graph | Gestiona qué algoritmo utilizar. Inicializa objetos para la cámara, el `Teseo` y el hilo que se dibujará al hacer el `Teseo` el camino, además de las propiedades del propio hilo (anchura). Su método `Update()` controla si se pulsa la tecla `"Espacio"` la cuál activa el hilo, y en otro caso lo desactiva. Si se pulsa la tecla `"T"` activa/desactiva el camino suavizado. Luego si el hilo está activado elige el algoritmo a utilizar, comprueba si estará suavizado y dibuja el hilo (llamada a `DrawThread()`); `OnDrawGizmos()` que dibuja las esferas en las baldosas y las líneas del hilo; `ShowPathVertices(List<Vertex> path, Color color)` el cuál recorre el camino mostrándolo de un color; GetNodeFromScreen(Vector3 screenPosition)que mediante RayCast traduce las posiciones en la pantalla a los nodos; `DrawThread()` setea las posiciones donde el hilo de Ariadna se dibujará; `updateAriadna(bool ar)` según el valor del booleano activa/desactiva el hilo; `ChangeHeuristic()` que cambia la heurística del algoritmo; `ResetPath()` pone a null el camino. |
| Vertex | Representa los vértices. Contiene métodos para comparar los costes entre vértices o para saber si un vérice es igual a otro. |

| Clases: GAMEMANAGER | Información |
| - | - |
| GameManager | Gestiona todo lo relativo a la propia partida. En primer lugar está la interfaz gráfica, que durante la sección del menú principal, muestra un menú de configuración para establecer el tipo de mapa (10x10, 20x20 o 30x30) y la cantidad de minotauros en la partida. A continuación, en base a los datos previamente pedidos al usuario, se carga una de las tres escenas y guarda el valor del número de minotauros que van a aparecer en la partida. Una vez ésta se ha iniciado, se coloca una interfaz gráfica en la esquina superior izquierda donde aparezca la tasa de fotogramas por segundo y diversas configuraciones que se pueden habilitar a través de distintas teclas del teclado (como el cambio de tasa de fotogramas). Tiene distintos métodos de tipo "getters" (como el tamaño del laberinto, el número de minotauros o el jugador) y "setters" (como la zona de inicio y fin del laberinto o el número de minotauros), así como un buscador de GameObjects, reinicio de escena, carga de escenas... |

## Diseño de la solución

C. El pseudocódigo del algoritmo A* es el siguiente:

```pseudo
# Estructura de nodo: contiene solamente su identificador y tiene métodos para comparar nodos
class Vertex:
    id : int                    # Identificador del nodo
    PreviousId : int            # Identificador del nodo anterior
    Cost : float                # Coste del nodo
    CostSoFar : float           # Coste acumulado hasta el nodo
    EstimatedTotalCost : float  # Coste total estimado hasta el nodo

    # Comprueba si es el mismo nodo a partir de su id
    function Equals(Vertex other) -> bool:
        return id == other.id

    # Comprueba si es el mismo nodo objeto
    function Equals(object other) -> bool:
        return Equals(other as Vertex)

    # Devuelve el hash code del nodo
    function GetHashCode() -> int:
        return id.GetHashCode()

# Estructura de Grafo: representación de una escena de juego como un grafo
class Graph:
    vertices : Vertex[]                 # Lista de nodos
    neighbourVertex : Vertex[][]        # Lista de nodos vecinos a partir de cada nodo
    costs : float[][]                   # Lista de costes entre nodos
    mapVertices : bool[][]              # Mapa de nodos
    costsVertices : float[][]           # Coste acumulado entre nodos
    numCols : int                       # Número de columnas
    numRows : int                       # Número de filas
    path : Vertex[]                     # Camino solución

    # Estructura de heurística: método para estimar el coste entre dos nodos
    function Heuristic(a : Vertex, b : Vertex) -> float

    # Algoritmo A* para encontrar el camino más corto entre dos nodos de un grafo con pesos no negativos y dirigido
    # start : Nodo de inicio
    # end : Nodo de fin
    # heuristic : Heurística a utilizar
    function pathfindStar(start : Vertex, end : Vertex, heuristic : Heuristic) -> Vertex[]:
        start.PreviousId = -1               # Indica que no tiene nodo anterior
        start.CostSoFar = 0
        start.EstimatedTotalCost = heuristic(startNode, goalNode)

        # Inicializa las listas abierta y cerrada
        open = new BinaryHeap<Vertex>()
        open.add(start)
        closed = new BinaryHeap<Vertex>()

        Vertex current = new Vertex()

        # Mientras haya nodos en la lista abierta
        while open.Count > 0:
            # Coge el nodo con menor coste total estimado
            current = open.SmallestElement() # open.Top
            
            # Si el nodo es el nodo de fin, termina
            if current.id == goal.id:
                break

            # Si no, coge las conexiones que hay entre el nodo actual y sus vecinos
            connections : Vertex[] = GetConnectionNeighbours(current)

            # Itera sobre las conexiones para cada nodo vecino
            for connection in connections:
                # Coge el nodo vecino
                endNode = connection
                endNodeCost = current.CostSoFar + connection.Cost *f = g + h*
                endNodeHeuristic : float

                # Si el nodo vecino está en la lista cerrada, continúa
                if closed.Contains(endNode):

                    # Si sigue siendo más barato, continúa
                    if endNode.CostSoFar <= endNodeCost:
                        continue

                    # Si no, se quita de la lista cerrada
                    closed.remove(endNode)

                    endNodeHeuristic = endNode.EstimatedTotalCost - endNode.CostSoFar
                    
                # Si el nodo vecino está en la lista abierta
                else if open.Contains(endNode):

                    if endNode.CostSoFar <= endNodeCost:
                        continue

                    endNodeHeuristic = endNode.Cost - endNode.CostSoFar
                
                # Si no, crea un registro para el nodo vecino
                else:
                    endNodeHeuristic = heuristic(endNode, goalNode)
                
                endNode.CostSoFar = endNodeCost
                endNode.PreviousId = current.id
                endNode.EstimatedTotalCost = endNodeCost + endNodeHeuristic

                # Si el nodo vecino no está en la lista abierta, se añade
                if not open.contains(endNode):
                    open.add(endNode)

            # Se quita el nodo de la lista abierta y se añade a la lista cerrada
            open.remove(current)
            closed.add(current)

        # Si no se ha encontrado el nodo de fin, no hay solución
        if current.id != goal.id:
            return null
        # Si se ha encontrado, se reconstruye el camino
        else:
            BuildPath(current, start)
    
    # Método para reconstruir el camino
    function BuildPath(current : Vertex, start : Vertex):
        path = []
        path.add(current)

        # Reconstruye el camino
        while current.id != start.id:
            prevVertex : Vertex = GetVertexById(current.PreviousId)
            path.add(prevVertex)
            current = prevVertex

        # Devuelve el camino reconstruido en orden inverso
        return reverse(path)
```

D. El propio algoritmo de A* requerirá un suavizado si queremos que el personaje encuentre el camino correctamente y éste llegue a su destino. Para ello, se recurrirá al libro _AI for Games_ de Ian Millington. En dicho libro, Millington menciona que los grafos basados en mosaicos pueden en ocasiones ser algo erráticos por lo que, para que la inteligencia artificial parezca medianamente inteligente, se utilizan adicionalmente comportamientos de dirección como los vistos en la práctica anterior que hagan suavizar dichos movimientos. Estos comportamientos son esenciales si se quiere que las IA parezcan inteligentes en su comportamiento y, aunque sea fácil de implementar, requiere consultas constantes a nivel de geometría, por lo que el rendimiento puede en consecuencia verse afectado.

Para suavizar el camino de entrada, primero se crea un nuevo camino vacío, que será el camino de salida, y tendrá como punto de inicio y destino los mismos que el de entrada. Comenzando desde el tercer nodo del camino de entrada, se traza un rayo a cada nodo sucesivamente desde el último nodo del camino de salida (nótese que se asume que hay una clara línea entre los nodos primero y segundo). Si un rayo falla, el nodo anterior del camino de entrada es añadido al de salida. El trazado de rayos vuelve a empezar desde el nodo siguiente del camino de entrada. Cuando se alcanza el último nodo, se añade al camino de salida. La siguiente imagen muestra un camino que ha sido suavizado por éste algoritmo:

![image](https://github.com/IAV24-G02/IAV24-G02-P2/assets/82498461/de9048db-8b09-4716-9e90-787151e6bb40)


Aunque dicho algoritmo produce un camino suavizado, éste no busca todos los posibles caminos suavizados hasta encontrar el más adecuado. La anterior imagen muestra el camino más suavizado, pero no se puede generar por el propio algoritmo. Esto se debe a que habría que hacer una búsqueda de todos los posibles caminoz suavizados, lo cual no será ni siquiera necesario.

Millington, en base a lo explicado anteriormente, plantea el siguiente _pseudo-código_, donde se acepta un camino de nodos ya establecido sin suavizar y lo devuelve suavizado:

```pseudo
function smoothPath(inputPath: Vector[]) -> Vector[]:
    # Si el camino es solo de dos nodos de longitud, entonces no se puede
    # suavizar
    if len(inputPath) == 2:
        return inputPath
    # Compilar un camino de salida
    outputPath = [inputPath[0]]
    
    # Se hace un seguimiento de la posición en la que se encuentra. Se empieza con dos,
    # ya que se asume que dos nodos adyacentes pasarán el trazado del rayo
    inputIndex: int = 2

    # Loop until we find the last item in the input.
    while inputIndex < len(inputPath) - 1:
        # Hacer el trazado del rayo
        fromPt = outputPath[len(outputPath) - 1]
        toPt = inputPath[inputIndex]
        if not rayClear(fromPt, toPt):
            # Si el trazado de rayo ha fallado, se añade el último nodo al final
            # de la lista de salida
            outputPath += inputPath[inputIndex - 1]

            # Se considera el siguiente nodo
            inputIndex ++

    # Se ha llegado al final del camino de entrada, por lo que se añade el último
    # nodo al de salida y se devuelve
    outputPath += inputPath[len(inputPath) - 1]

    return outputPath
```

## Pruebas y métricas

| Pruebas | Métricas | Links |
|:-:|:-:|:-:|
| **Característica A** | | |
| Comprobar que funcione la navegación del `Teseo` con el clic izquierdo | - | [MOVIMIENTO](https://drive.google.com/file/d/12TAMhU3OP-bBdbMAnVh1SWoZj0bHm-g6/view?usp=sharing) |

| Pruebas | Métricas | Links |
|:-:|:-:|:-:|
| **Característica B** | | |
| Comprobar que la detección del jugador mediante la vista del `Minotauro` funciona correctamente, solo detecta cuando el jugador entra dentro del cono de visión y no hay muros de por medio. | - Mapa de 10x10 <br> - Un mapa en el que el `Minotauro`y el `Teseo` estan separados por un muro impenetrable. <br> - Un mapa en el que no hay obstáculos a excepción de uno en el medio y el jugador debe entrar y salir del cono de visión. | [DETECCIÓN VISTA](https://drive.google.com/file/d/1yjne_gPnRA5pM24ZW6FfbKC4LbddWreM/view?usp=sharing) |
| Comprobar que el `Minotauro` detecta al `Teseo` cuando entran en su rango de detección por sonido. | - Desactivación de la detección por visión. <br> - Un mapa en el que solo haya una pared alrededor de la cuál el jugador puede probar la audición del `Minotauro` | [DETECCIÓN OÍDO](https://drive.google.com/file/d/1kbypJ9AABAl0-OOR0HZDHgcnpvvsCdEw/view?usp=sharing) |  

| Pruebas | Métricas | Links |
|:-:|:-:|:-:|
| **Característica C** | | |
| Comprobar el cambio de representación del hilo y de movimiento del `Teseo` con la implicación de un `Minotauro` estático | - Mapa en el que haya dos caminos de igual coste pero siempre elige el que no incluya al `Minotauro` en medio  | [UN MINOTAURO](https://drive.google.com/file/d/1XJoTXgNrIlESjEo2dIhWiPdkuWuX2LAH/view?usp=sharing) |
| Comprobar el cambio de representación del hilo y de movimiento del `Teseo` con la implicación de uno o varios `Minotauros` en movimiento | - | [VARIOS MINOTAUROS](https://drive.google.com/file/d/1nuSeHpZmtHwazfHJc5SkJKftI4GhISVt/view?usp=sharing) |

| Pruebas | Métricas | Links |
|:-:|:-:|:-:|
| **Característica D** | | |
| Comprobar que el suavizado de A* se activa/desactiva al usar la opción de la interfaz, y el camino cambia. | Cambio de suavizado mediante la opicón de la interfaz. | [SUAVIZADO INTERFAZ](https://drive.google.com/file/d/1Rs9qw0BKmlXU7kXSELcKZLcQhK0vRimp/view?usp=drive_link) |
| Comprobar si el suavizado se realiza correctamente con las diversas heurísticas. | - Distintos tamaños de mapa: 10x10, 20x20, 30x30 <br> - Cambio de heurística: Primera, Segunda, etc. | [DISTINTOS MAPAS](https://drive.google.com/file/d/1L2RtdqQLwM6m-jgnZ6g-iSkRYbAppLdx/view?usp=drive_link) |

| Pruebas | Métricas | Links |
|:-:|:-:|:-:|
| **Característica E** | | |
| Comprobar que se desactive parte del hilo y esferas por las zonas según el `Teseo` va navegando automáticamente y cuando se desactiva la navegación automática.| - Distintos tamaños de mapa: 10x10, 20x20, 30x30 <br> - Cambio de heurística: Primera, Segunda, etc. | [ARIADNA](https://drive.google.com/file/d/1BP72W27eE55rpuz1eSdx0s5N6PxH-VGZ/view?usp=sharing) |
| Comprobar que desaparece y aparece de forma correcta el hilo activando y desactivando repetidas veces la navegación automática y retrocediendo al punto de partida manualmente. | - Distintos tamaños de mapa: 10x10, 20x20, 30x30 <br> - Cambio de heurística: Primera, Segunda, etc. | [ARIADNA REPETIDAMENTE](https://drive.google.com/file/d/1E-kBHf0Kohm11gpHut5OWXPePT9d9sYM/view?usp=sharing) |

## Ampliaciones

- Se muestran visualmente las zonas de percepción visual y auditiva de todos los minotauros representados con una esfera alrededor de los minotauros y un cono que representa el cono de visión de los minotauros.

## Producción

Las tareas se han realizado y el esfuerzo ha sido repartido entre los autores. Observa la tabla de abajo para ver el estado y las fechas de realización de las mismas. Puedes visitar nuestro proyecto de GitHub en el siguiente [link](https://github.com/orgs/IAV24-G02/projects/2/).

| Estado  |  Tarea  |  Fecha  |  
|:-:|:--|:-:|
| ✔ | Diseño: Primer borrador | 15-03-2024 |
| ✔ | Característica A | 19-03-2024 |
| ✔ | Característica B | 19-03-2024 |
| ✔ | Característica C | 21-03-2024 |
| ✔ | Característica D | 21-03-2024 |
| ✔ | Característica E | 19-03-2024 |
|  |  **OTROS**  | |
| ✔ | Merodeo de los `Minotauros` sin que se salgan del laberinto | 20-03-2024 |
| ✔ | Coloreado de las baldosas de influencia de los `Minotauros` | 20-03-2024 |
|  |  **OPCIONALES**  | |
| ✔ | Muestra visualmente las zonas de percepción visual y auditiva de todos los minotauros | 21-03-2024 |

## Instrucciones para probar el proyecto

Para probar el proyecto, se debe abrir con Unity. A continuación, se detallan las instrucciones para probar cada una de las características del proyecto:

- Para cambiar el radio de detección del minotauro: Acceder al prefab del Minotaur en Assets > Prefabs > Minotaur y cambiar Detection Radio

- Para no generar minotauros en la escena: En la escena de Labyrinth, Desactivar MinoManager

- Para generar un minotauro de prueba en una posición concreta, activar el modo Debug en el inspector de MinoManager y colocar el GameObject de MinoDebugPos en la posición solicitada

- Para cambiar la estructura de los mapas, acceder a la carpeta de StreamingAssets > Maps, editar el archivo .map solicitado


## Notas

Se ha tenido en cuenta en que en el pseudocódigo proporcionado por el libro de Millington, se ha usado la clase de Connection, que no se ha implementado en el proyecto base pues se ha considerado que era más sencillo y eficiente usar la clase de Vertex para representar las conexiones entre nodos. A continuación se muestra la estructura de la clase Connection:

```pseudo
# Estructura de conexión/arista: contiene el nodo origen, el nodo destino y el coste de la conexión/arista
class Connection:
    FromNode : Vertex   # Nodo origen
    ToNode : Vertex     # Nodo destino
    Cost : float        # Coste de la conexión
```

Como consecuencia, tampoco ha sido necesario implementar la clase PathfindingList ni la clase NodeRecord, que se usan en el pseudocódigo para almacenar los nodos abiertos y cerrados y para guardar registro de las conexiones respectivamente. En el lugar de un PathFindingList, se ha usado un BinaryHeap<Vertex> directamente. A continuación se muestra la estructura de la clase PathfindingList y la clase NodeRecord que se ha usado en el pseudocódigo de Millington:

```pseudo
# Estructura de lista de nodos para pathfinding
class PathFindingList:
    records : NodeRecord[]    # Lista de NodeRecords

    # Añade un nodo a la lista
    function Add(nodeRecord : NodeRecord):
        records += nodeRecord   # En el código real se implementa una inserción

    # Elimina un nodo de la lista
    function Remove(nodeRecord : NodeRecord):
        records -= nodeRecord  # En el código real se implementa una eliminación

    # Encuentra un nodo en la lista
    function Find(node : Vertex) -> NodeRecord:
        return records.find(node)   # En el código real se implementa una búsqueda

    # Comprueba si la lista contiene un nodo
    function Contains(node : Vertex) -> bool:
        return records.Find(node) != null

    # Devuelve el nodo con menor coste total estimado
    function SmallestElement() -> NodeRecord:
        return records.minBy(record => record.estimatedTotalCost)

    # Devuelve la longitud de la lista
    function Length() -> int:
        return records.length

# Estructura de registro de nodo que incluye el nodo, la conexión, el coste hasta el momento y el coste total estimado
class NodeRecord:
    Node : Vertex               # Nodo seleccionado
    PreviousNode : NodeRedord   # Referencia al nodo anterior
    Cost: float                 # Coste de la conexión entre node y previousNode
    CostSoFar: float            # Coste acumulado hasta el nodo seleccionado
    EstimatedTotalCost: float   # Coste total estimado hasta el nodo seleccionado
```

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

