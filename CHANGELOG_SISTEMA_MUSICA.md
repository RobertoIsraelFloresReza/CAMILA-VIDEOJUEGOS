# 🎵 Documentación: Sistema de Música Persistente

**Fecha de implementación:** 4 de Diciembre, 2024
**Implementado por:** Roberto Israel Flores Reza (@dev-isra)
**Rama:** `dev-isra`
**Versión:** 1.0.0

---

## 📋 Tabla de Contenidos

1. [Resumen Ejecutivo](#resumen-ejecutivo)
2. [Problema Identificado](#problema-identificado)
3. [Solución Implementada](#solución-implementada)
4. [Archivos Creados](#archivos-creados)
5. [Archivos Modificados](#archivos-modificados)
6. [Arquitectura y Patrones de Diseño](#arquitectura-y-patrones-de-diseño)
7. [Guía de Uso](#guía-de-uso)
8. [Pruebas y Verificación](#pruebas-y-verificación)
9. [Troubleshooting](#troubleshooting)
10. [Mantenimiento Futuro](#mantenimiento-futuro)

---

## 🎯 Resumen Ejecutivo

Se implementó un **sistema de música persistente** que permite que la música de fondo del juego continúe reproduciéndose sin interrupciones cuando el jugador cambia entre diferentes escenas del juego.

### Cambios principales:
- ✅ Creación de un prefab reutilizable `MusicManager`
- ✅ Implementación de patrón Singleton para gestión de música
- ✅ Integración del prefab en 6 escenas del juego
- ✅ Sistema automático de prevención de duplicados

---

## ❌ Problema Identificado

### Situación Anterior

En la configuración original del proyecto, el sistema de música presentaba los siguientes problemas:

#### 1. **Música solo en MainMenu**
```
MainMenu.unity
└── GameObject "musica"
    ├── AudioSource (con música de fondo)
    └── Sin script de persistencia
```

**Problema:** Cuando el jugador iniciaba el juego desde el MainMenu y cambiaba a cualquier escena de juego (ej. CabinMap), la música se detenía porque:
- El GameObject "musica" **no tenía** `DontDestroyOnLoad`
- Unity destruye todos los objetos de una escena cuando se carga otra escena
- Las escenas de juego no tenían su propio sistema de música

#### 2. **Experiencia de Usuario Deficiente**

```
Flujo de juego anterior:
MainMenu.unity → 🎵 Música sonando
       ↓ (Usuario presiona "Play")
CabinMap.unity → ❌ Silencio total (música se detiene)
       ↓
ChurchMap.unity → ❌ Silencio total
       ↓
LakeMap.unity → ❌ Silencio total
```

**Resultado:** Los jugadores solo escuchaban música en el menú principal, no durante el gameplay.

#### 3. **Configuración del GameObject "musica" original**

**Ubicación:** `Assets/Scenes/MainMenu.unity` (línea 2372-2500 aproximadamente)

```yaml
--- !u!1 &1841927037
GameObject:
  m_Name: musica
  m_Component:
  - component: {fileID: 1841927039}  # Transform
  - component: {fileID: 1841927038}  # AudioSource

--- !u!82 &1841927038
AudioSource:
  OutputAudioMixerGroup: {fileID: 8017999561391187091, guid: 042038128ef0b7d46949dde4ae1b6206}
  m_Resource: {fileID: 8300000, guid: 9a9057144458c1f46a591cda51081e1c}
  m_PlayOnAwake: 1
  Loop: 1
  m_Volume: 1
```

**Características:**
- ✅ Reproduce automáticamente al iniciar (`m_PlayOnAwake: 1`)
- ✅ En bucle infinito (`Loop: 1`)
- ✅ Conectado al AudioMixer para control de volumen
- ❌ **NO tiene persistencia entre escenas**
- ❌ **Solo existe en MainMenu.unity**

---

## ✅ Solución Implementada

### Enfoque: Prefab Persistente + Singleton Pattern

Se decidió implementar una solución basada en:
1. **Prefab reutilizable** que puede ser instanciado en múltiples escenas
2. **Patrón Singleton** para garantizar una sola instancia de música
3. **DontDestroyOnLoad** para persistencia entre cambios de escena

### Flujo de Juego Mejorado

```
Cualquier escena cargada → MusicManager se instancia → 🎵 Música inicia
       ↓
Cambio a otra escena → MusicManager persiste → 🎵 Música continúa
       ↓
Cambio a otra escena → MusicManager persiste → 🎵 Música continúa
       ↓
Regreso a escena anterior → Singleton previene duplicado → 🎵 Música continúa
```

**Resultado:** Música continua e ininterrumpida en todas las escenas del juego.

---

## 📦 Archivos Creados

### 1. `Assets/Scripts/MusicManager.cs`

**Propósito:** Script de gestión de persistencia de música usando patrón Singleton.

**Contenido completo:**

```csharp
using UnityEngine;

/// <summary>
/// Singleton que hace persistente el GameObject de música entre escenas.
/// Evita que se duplique cuando se recarga una escena.
/// </summary>
public class MusicManager : MonoBehaviour
{
    // Singleton estático
    private static MusicManager instance;

    void Awake()
    {
        // Si ya existe una instancia de MusicManager...
        if (instance != null)
        {
            // Destruir este duplicado
            Destroy(gameObject);
            return;
        }

        // Esta es la primera instancia
        instance = this;

        // Hacer persistente entre escenas
        DontDestroyOnLoad(gameObject);
    }
}
```

**Explicación Línea por Línea:**

| Línea | Código | Explicación |
|-------|--------|-------------|
| 1 | `using UnityEngine;` | Importa la librería base de Unity |
| 8 | `public class MusicManager : MonoBehaviour` | Define la clase que hereda de MonoBehaviour (componente de Unity) |
| 11 | `private static MusicManager instance;` | Variable estática que guarda la única instancia permitida |
| 13 | `void Awake()` | Método que se ejecuta ANTES de Start() cuando el GameObject se activa |
| 16 | `if (instance != null)` | Pregunta: ¿Ya existe una instancia? |
| 19 | `Destroy(gameObject);` | Si ya existe, destruye ESTE GameObject duplicado |
| 20 | `return;` | Sale del método Awake para no ejecutar el resto del código |
| 24 | `instance = this;` | Si NO existe, guarda ESTA instancia como la oficial |
| 27 | `DontDestroyOnLoad(gameObject);` | **CLAVE:** Marca este GameObject para que NO se destruya al cambiar de escena |

**¿Por qué Awake() y no Start()?**
- `Awake()` se ejecuta ANTES que `Start()`
- Necesitamos verificar duplicados lo más pronto posible
- Si usáramos `Start()`, podrían crearse duplicados temporalmente

**Metadata del archivo (.meta):**

Ubicación: `Assets/Scripts/MusicManager.cs.meta`

```yaml
fileFormatVersion: 2
guid: a7b3c9d2e4f5a6b8c9d0e1f2a3b4c5d6
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData:
  assetBundleName:
  assetBundleVariant:
```

**GUID único:** `a7b3c9d2e4f5a6b8c9d0e1f2a3b4c5d6` - Unity usa este identificador para referenciar el script.

---

### 2. `Assets/Prefabs/MusicManager.prefab`

**Propósito:** Prefab que encapsula todo el sistema de música para ser reutilizado en múltiples escenas.

**Estructura del Prefab:**

```
MusicManager (GameObject)
├── Transform (Component)
│   ├── Position: (0, 0, 0)
│   ├── Rotation: (0, 0, 0, 1)
│   └── Scale: (1, 1, 1)
│
├── MusicManager Script (Component)
│   └── Singleton + DontDestroyOnLoad
│
└── AudioSource (Component)
    ├── OutputAudioMixerGroup: AudioMixer (GUID: 042038128ef0b7d46949dde4ae1b6206)
    ├── Audio Clip: (GUID: 9a9057144458c1f46a591cda51081e1c)
    ├── PlayOnAwake: true ✅
    ├── Loop: true ✅
    ├── Volume: 1.0
    ├── Pitch: 1.0
    ├── Priority: 128 (default)
    ├── Spatial Blend: 2D (0.0)
    └── Min/Max Distance: 1/500
```

**Configuración Detallada del AudioSource:**

| Propiedad | Valor | Significado |
|-----------|-------|-------------|
| `OutputAudioMixerGroup` | `{fileID: 8017999561391187091, guid: 042038128ef0b7d46949dde4ae1b6206, type: 2}` | Conectado al AudioMixer para control de volumen desde el menú de opciones |
| `m_audioClip` | `{fileID: 0}` | No usa clip directo (usa Resource) |
| `m_Resource` | `{fileID: 8300000, guid: 9a9057144458c1f46a591cda51081e1c, type: 3}` | Archivo de audio cargado como recurso |
| `m_PlayOnAwake` | `1` (true) | La música empieza automáticamente cuando se carga la escena |
| `m_Volume` | `1` | Volumen al 100% (el AudioMixer controla el volumen real) |
| `m_Pitch` | `1` | Velocidad normal (1.0 = normal, 2.0 = doble velocidad) |
| `Loop` | `1` (true) | La música se repite infinitamente |
| `Mute` | `0` (false) | No silenciado |
| `Spatialize` | `0` (false) | Sonido 2D (no espacial/3D) |
| `Priority` | `128` | Prioridad media (0 = máxima, 256 = mínima) |

**Curvas de Audio (Rolloff, Pan, Spread):**
- Como es audio 2D, estas curvas no afectan el sonido
- Se mantienen en valores por defecto

**Metadata del Prefab (.meta):**

```yaml
fileFormatVersion: 2
guid: f1e2d3c4b5a697887766554433221100
PrefabImporter:
  externalObjects: {}
  userData:
  assetBundleName:
  assetBundleVariant:
```

**GUID único:** `f1e2d3c4b5a697887766554433221100` - Las escenas usan este GUID para referenciar el prefab.

---

## 🔧 Archivos Modificados

Se modificaron **6 escenas** del juego para incluir el prefab `MusicManager`.

### Cambios Realizados en Cada Escena

Para cada escena, se realizaron **2 modificaciones**:

1. **Inserción de PrefabInstance** (antes de SceneRoots)
2. **Adición de referencia** en la lista `m_Roots` de SceneRoots

#### Estructura de los Cambios

**ANTES:**
```yaml
  m_SourcePrefab: {fileID: 100100000, guid: <algún-prefab-anterior>, type: 3}
--- !u!1660057539 &9223372036854775807
SceneRoots:
  m_ObjectHideFlags: 0
  m_Roots:
  - {fileID: <objeto1>}
  - {fileID: <objeto2>}
  - {fileID: <objeto3>}
```

**DESPUÉS:**
```yaml
  m_SourcePrefab: {fileID: 100100000, guid: <algún-prefab-anterior>, type: 3}
--- !u!1001 &<fileID-único>          ← NUEVO BLOQUE INSERTADO
PrefabInstance:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_Modification:
    serializedVersion: 3
    m_TransformParent: {fileID: 0}   ← Sin padre (root level)
    m_Modifications:
    - target: {fileID: 5834076382806549728, guid: f1e2d3c4b5a697887766554433221100, type: 3}
      propertyPath: m_LocalPosition.x
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5834076382806549728, guid: f1e2d3c4b5a697887766554433221100, type: 3}
      propertyPath: m_LocalPosition.y
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5834076382806549728, guid: f1e2d3c4b5a697887766554433221100, type: 3}
      propertyPath: m_LocalPosition.z
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5834076382806549728, guid: f1e2d3c4b5a697887766554433221100, type: 3}
      propertyPath: m_LocalRotation.w
      value: 1
      objectReference: {fileID: 0}
    - target: {fileID: 5834076382806549728, guid: f1e2d3c4b5a697887766554433221100, type: 3}
      propertyPath: m_LocalRotation.x
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5834076382806549728, guid: f1e2d3c4b5a697887766554433221100, type: 3}
      propertyPath: m_LocalRotation.y
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5834076382806549728, guid: f1e2d3c4b5a697887766554433221100, type: 3}
      propertyPath: m_LocalRotation.z
      value: 0
      objectReference: {fileID: 0}
    - target: {fileID: 5834076382806549729, guid: f1e2d3c4b5a697887766554433221100, type: 3}
      propertyPath: m_Name
      value: MusicManager
      objectReference: {fileID: 0}
    m_RemovedComponents: []
    m_RemovedGameObjects: []
    m_AddedGameObjects: []
    m_AddedComponents: []
  m_SourcePrefab: {fileID: 100100000, guid: f1e2d3c4b5a697887766554433221100, type: 3}
--- !u!1660057539 &9223372036854775807
SceneRoots:
  m_ObjectHideFlags: 0
  m_Roots:
  - {fileID: <objeto1>}
  - {fileID: <objeto2>}
  - {fileID: <objeto3>}
  - {fileID: <fileID-único>}         ← REFERENCIA AGREGADA
```

---

### Escenas Modificadas (Detalle por Escena)

#### 1. **CabinMap.unity** (Mapa de la Cabaña)

**Ubicación del cambio:** Línea ~13893 (antes de SceneRoots)

**FileID asignado:** `1234567890`

**SceneRoots ANTES:**
```yaml
m_Roots:
  - {fileID: 789296679}
  - {fileID: 592656491}
  - {fileID: 461201097}
  - {fileID: 114404996}
  - {fileID: 1705122094}
  - {fileID: 107919879}
  - {fileID: 362269155}
  - {fileID: 915516173}
  - {fileID: 189980183}
  - {fileID: 130040799}
  - {fileID: 371311776}
```

**SceneRoots DESPUÉS:**
```yaml
m_Roots:
  - {fileID: 789296679}
  - {fileID: 592656491}
  - {fileID: 461201097}
  - {fileID: 114404996}
  - {fileID: 1705122094}
  - {fileID: 107919879}
  - {fileID: 362269155}
  - {fileID: 915516173}
  - {fileID: 189980183}
  - {fileID: 130040799}
  - {fileID: 371311776}
  - {fileID: 1234567890}  ← AGREGADO
```

---

#### 2. **ChurchMap.unity** (Mapa de la Iglesia)

**Ubicación del cambio:** Línea ~5764 (antes de SceneRoots)

**FileID asignado:** `1234567891`

**SceneRoots ANTES:**
```yaml
m_Roots:
  - {fileID: 1606644977}
  - {fileID: 625038203}
  - {fileID: 1094527096}
  - {fileID: 509048205}
  - {fileID: 899671581}
  - {fileID: 1752781446}
```

**SceneRoots DESPUÉS:**
```yaml
m_Roots:
  - {fileID: 1606644977}
  - {fileID: 625038203}
  - {fileID: 1094527096}
  - {fileID: 509048205}
  - {fileID: 899671581}
  - {fileID: 1752781446}
  - {fileID: 1234567891}  ← AGREGADO
```

---

#### 3. **LakeMap.unity** (Mapa del Lago)

**Ubicación del cambio:** Línea ~7111 (antes de SceneRoots)

**FileID asignado:** `1234567892`

**SceneRoots ANTES:**
```yaml
m_Roots:
  - {fileID: 1191142398}
  - {fileID: 1748214320}
  - {fileID: 233902631}
  - {fileID: 3879569400024119921}
  - {fileID: 1170723737}
  - {fileID: 153824025}
```

**SceneRoots DESPUÉS:**
```yaml
m_Roots:
  - {fileID: 1191142398}
  - {fileID: 1748214320}
  - {fileID: 233902631}
  - {fileID: 3879569400024119921}
  - {fileID: 1170723737}
  - {fileID: 153824025}
  - {fileID: 1234567892}  ← AGREGADO
```

---

#### 4. **MountainMap.unity** (Mapa de la Montaña)

**Ubicación del cambio:** Línea ~271 (antes de SceneRoots)

**FileID asignado:** `1234567893`

**SceneRoots ANTES:**
```yaml
m_Roots:
  - {fileID: 436251383}
  - {fileID: 1990618184}
```

**SceneRoots DESPUÉS:**
```yaml
m_Roots:
  - {fileID: 436251383}
  - {fileID: 1990618184}
  - {fileID: 1234567893}  ← AGREGADO
```

---

#### 5. **NorthWestMap.unity** (Mapa Noroeste)

**Ubicación del cambio:** Línea ~1338 (antes de SceneRoots)

**FileID asignado:** `1234567894`

**SceneRoots ANTES:**
```yaml
m_Roots:
  - {fileID: 1823370045}
  - {fileID: 1602239473}
  - {fileID: 478873528}
  - {fileID: 340344255}
  - {fileID: 1481538374}
  - {fileID: 801241088}
  - {fileID: 833580890}
```

**SceneRoots DESPUÉS:**
```yaml
m_Roots:
  - {fileID: 1823370045}
  - {fileID: 1602239473}
  - {fileID: 478873528}
  - {fileID: 340344255}
  - {fileID: 1481538374}
  - {fileID: 801241088}
  - {fileID: 833580890}
  - {fileID: 1234567894}  ← AGREGADO
```

---

#### 6. **Game.unity** (Escena Principal del Juego)

**Ubicación del cambio:** Línea ~723 (antes de SceneRoots)

**FileID asignado:** `1234567895`

**SceneRoots ANTES:**
```yaml
m_Roots:
  - {fileID: 330585546}
  - {fileID: 410087041}
  - {fileID: 832575519}
  - {fileID: 32246157}
  - {fileID: 8873029776107908791}
  - {fileID: 715666180}
```

**SceneRoots DESPUÉS:**
```yaml
m_Roots:
  - {fileID: 330585546}
  - {fileID: 410087041}
  - {fileID: 832575519}
  - {fileID: 32246157}
  - {fileID: 8873029776107908791}
  - {fileID: 715666180}
  - {fileID: 1234567895}  ← AGREGADO
```

---

### Resumen de FileIDs Asignados

| Escena | FileID Asignado | Propósito |
|--------|-----------------|-----------|
| CabinMap.unity | 1234567890 | Identificador único del PrefabInstance en esta escena |
| ChurchMap.unity | 1234567891 | Identificador único del PrefabInstance en esta escena |
| LakeMap.unity | 1234567892 | Identificador único del PrefabInstance en esta escena |
| MountainMap.unity | 1234567893 | Identificador único del PrefabInstance en esta escena |
| NorthWestMap.unity | 1234567894 | Identificador único del PrefabInstance en esta escena |
| Game.unity | 1234567895 | Identificador único del PrefabInstance en esta escena |

**Nota:** Estos FileIDs son únicos POR ESCENA y no se repiten dentro de la misma escena.

---

### ¿Qué es SceneRoots?

`SceneRoots` es una sección especial en las escenas de Unity que lista todos los GameObjects de nivel raíz (root level) en la jerarquía de la escena.

**Ejemplo de Jerarquía:**
```
Jerarquía de la Escena:
├── Main Camera         ← Root (en SceneRoots)
├── Directional Light   ← Root (en SceneRoots)
├── Player              ← Root (en SceneRoots)
│   ├── Camera Child    ← NO root (hijo de Player)
│   └── Weapon          ← NO root (hijo de Player)
├── Terrain             ← Root (en SceneRoots)
└── MusicManager        ← Root (en SceneRoots) ← NUEVO
```

Agregar `{fileID: 1234567890}` en `m_Roots` le dice a Unity:
> "Este GameObject MusicManager está en el nivel raíz de la escena"

---

## 🏗️ Arquitectura y Patrones de Diseño

### Patrón Singleton

**Definición:** Garantiza que una clase tenga **solo una instancia** y proporciona un punto de acceso global a ella.

**Implementación en MusicManager:**

```csharp
public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;  // ← Variable de clase (compartida por todas las instancias)

    void Awake()
    {
        if (instance != null)        // ← ¿Ya existe una instancia?
        {
            Destroy(gameObject);     // ← Sí → Destruir ESTE duplicado
            return;
        }

        instance = this;             // ← No → Esta es la instancia oficial
        DontDestroyOnLoad(gameObject);
    }
}
```

**Ventajas:**
- ✅ Solo una música sonando a la vez (no duplicados)
- ✅ Control centralizado del audio
- ✅ Fácil acceso desde cualquier script (si fuera necesario)

**Diagrama de Flujo:**

```
Primera escena cargada (ej. CabinMap):
    ┌─────────────────────────────┐
    │ MusicManager se instancia   │
    └───────────┬─────────────────┘
                │
                ▼
    ┌─────────────────────────────┐
    │ Awake() se ejecuta          │
    └───────────┬─────────────────┘
                │
                ▼
    ┌─────────────────────────────┐
    │ ¿instance != null?          │
    │ NO (es la primera vez)      │
    └───────────┬─────────────────┘
                │
                ▼
    ┌─────────────────────────────┐
    │ instance = this             │
    │ DontDestroyOnLoad(this)     │
    └───────────┬─────────────────┘
                │
                ▼
    ┌─────────────────────────────┐
    │ MusicManager PERSISTE       │
    │ 🎵 Música sonando           │
    └─────────────────────────────┘

Segunda escena cargada (ej. ChurchMap):
    ┌─────────────────────────────┐
    │ Nuevo MusicManager intenta  │
    │ instanciarse (del prefab)   │
    └───────────┬─────────────────┘
                │
                ▼
    ┌─────────────────────────────┐
    │ Awake() se ejecuta          │
    └───────────┬─────────────────┘
                │
                ▼
    ┌─────────────────────────────┐
    │ ¿instance != null?          │
    │ SÍ (ya existe uno)          │
    └───────────┬─────────────────┘
                │
                ▼
    ┌─────────────────────────────┐
    │ Destroy(gameObject)         │
    │ return;                     │
    └───────────┬─────────────────┘
                │
                ▼
    ┌─────────────────────────────┐
    │ Duplicado DESTRUIDO         │
    │ MusicManager ORIGINAL sigue │
    │ 🎵 Música continúa sonando  │
    └─────────────────────────────┘
```

---

### DontDestroyOnLoad

**¿Qué hace?**
Marca un GameObject para que NO sea destruido cuando Unity carga una nueva escena.

**Comportamiento Normal de Unity:**
```
Escena A cargada:
    GameObject "Musica" existe en memoria
    ↓
Unity carga Escena B:
    Unity destruye TODOS los GameObjects de Escena A
    GameObject "Musica" → DESTRUIDO ❌
```

**Con DontDestroyOnLoad:**
```
Escena A cargada:
    GameObject "MusicManager" existe en memoria
    DontDestroyOnLoad(MusicManager) marcado
    ↓
Unity carga Escena B:
    Unity destruye GameObjects de Escena A EXCEPTO los marcados
    GameObject "MusicManager" → PERSISTE ✅
    ↓
Unity carga Escena C:
    GameObject "MusicManager" → PERSISTE ✅
```

**Escena Especial en Jerarquía:**

Cuando un GameObject tiene `DontDestroyOnLoad`, Unity lo mueve a una escena especial llamada **"DontDestroyOnLoad"**:

```
Jerarquía en Unity Editor (durante gameplay):
📁 DontDestroyOnLoad        ← Escena especial (invisible en modo edición)
├── MusicManager            ← Nuestro objeto persistente
└── (otros objetos persistentes del juego)

📁 CabinMap (escena actual)
├── Main Camera
├── Player
├── Terrain
└── ...
```

---

### Prefab System

**¿Qué es un Prefab?**
Un "molde" reutilizable de un GameObject con todos sus componentes y configuración.

**Ventajas en este Proyecto:**

1. **Reutilización:**
   ```
   1 Prefab → Usado en 6 escenas
   Sin Prefab → Configurar manualmente 6 veces
   ```

2. **Consistencia:**
   ```
   Todos los MusicManager tienen:
   - Mismo AudioSource configurado
   - Mismo script MusicManager
   - Misma configuración de volumen
   ```

3. **Mantenimiento:**
   ```
   Cambio en Prefab → Se aplica a todas las escenas automáticamente
   Sin Prefab → Cambiar manualmente en 6 escenas
   ```

**Ejemplo de Uso:**
```
Cambiar la música del juego:

CON PREFAB:
1. Abrir MusicManager.prefab
2. Cambiar el AudioSource → nuevo clip de audio
3. Guardar
✅ TODAS las escenas actualizadas automáticamente

SIN PREFAB:
1. Abrir CabinMap.unity → Cambiar audio → Guardar
2. Abrir ChurchMap.unity → Cambiar audio → Guardar
3. Abrir LakeMap.unity → Cambiar audio → Guardar
4. Abrir MountainMap.unity → Cambiar audio → Guardar
5. Abrir NorthWestMap.unity → Cambiar audio → Guardar
6. Abrir Game.unity → Cambiar audio → Guardar
❌ Propenso a errores y olvidos
```

---

## 📘 Guía de Uso

### Para Desarrolladores

#### Agregar MusicManager a una Nueva Escena

Si crean una nueva escena y quieren que tenga música:

**Método 1: Desde el Editor de Unity**

1. Abrir la nueva escena en Unity
2. En la ventana `Project`, navegar a `Assets/Prefabs/`
3. Arrastrar `MusicManager.prefab` a la jerarquía de la escena
4. Guardar la escena (Ctrl+S / Cmd+S)

**Método 2: Desde Código (Avanzado)**

```csharp
// En algún script de inicialización de la escena
using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    void Start()
    {
        // Verificar si ya existe MusicManager
        if (FindObjectOfType<MusicManager>() == null)
        {
            // Cargar y instanciar el prefab
            GameObject prefab = Resources.Load<GameObject>("Prefabs/MusicManager");
            if (prefab != null)
            {
                Instantiate(prefab);
            }
        }
    }
}
```

---

#### Modificar la Música de Fondo

**Paso 1:** Importar el nuevo archivo de audio

1. Copiar el archivo de audio (MP3, WAV, OGG) a `Assets/SFX/` o crear una carpeta `Assets/Music/`
2. En Unity, seleccionar el archivo importado
3. En el Inspector, configurar:
   - `Load Type`: Streaming (para archivos grandes)
   - `Compression Format`: Vorbis (para música)
   - `Quality`: 70-80% (balance tamaño/calidad)

**Paso 2:** Actualizar el Prefab

1. En `Project`, abrir `Assets/Prefabs/MusicManager.prefab`
2. En el Inspector, buscar el componente `Audio Source`
3. En `AudioClip`, arrastrar el nuevo archivo de audio
4. Guardar el prefab (Ctrl+S / Cmd+S)

**¡Listo!** Todas las escenas ahora usarán la nueva música.

---

#### Ajustar el Volumen de la Música

**Opción 1: Desde el AudioMixer (Recomendado)**

El volumen se controla desde el AudioMixer para permitir ajustes desde el menú de opciones:

1. Abrir `Window → Audio → Audio Mixer`
2. Buscar el parámetro `volumenMusica `
3. Ajustar el slider de volumen
4. El script `Music.cs` (NewMonoBehaviourScript) ya controla esto desde el UI

**Opción 2: Directamente en el Prefab**

1. Abrir `Assets/Prefabs/MusicManager.prefab`
2. En el componente `Audio Source`
3. Ajustar `Volume` (0.0 a 1.0)

⚠️ **Nota:** Si cambias el volumen en el prefab, esto será el volumen BASE. El AudioMixer aplicará multiplicadores sobre este valor.

---

#### Detener/Pausar la Música por Script

Si necesitas detener o pausar la música desde código:

```csharp
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private AudioSource musicSource;

    void Start()
    {
        // Encontrar el MusicManager en la escena
        MusicManager manager = FindObjectOfType<MusicManager>();
        if (manager != null)
        {
            musicSource = manager.GetComponent<AudioSource>();
        }
    }

    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.UnPause();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void SetVolume(float volume)
    {
        if (musicSource != null)
        {
            // volume debe estar entre 0.0 y 1.0
            musicSource.volume = Mathf.Clamp01(volume);
        }
    }
}
```

---

### Para Diseñadores de Niveles

#### Verificar que la Música Funciona en tu Escena

1. **Abrir tu escena en Unity**
2. **Verificar en la jerarquía:**
   - Debe existir un GameObject llamado `MusicManager`
   - Si NO existe, arrastrar el prefab `Assets/Prefabs/MusicManager.prefab` a la jerarquía
3. **Probar en Play Mode:**
   - Presionar el botón Play (▶)
   - La música debe empezar a sonar automáticamente
   - Cambiar a otra escena → la música debe continuar

#### ¿La Música No Suena?

**Checklist:**
- ✅ ¿El GameObject `MusicManager` existe en la jerarquía?
- ✅ ¿El componente `Audio Source` está habilitado (checkbox marcado)?
- ✅ ¿El `AudioClip` tiene un archivo asignado?
- ✅ ¿`Play On Awake` está marcado?
- ✅ ¿`Mute` está desmarcado?
- ✅ ¿El volumen del AudioMixer no está en -80dB (silencio)?

---

## 🧪 Pruebas y Verificación

### Lista de Verificación (Checklist)

Para asegurarte de que todo funciona correctamente:

#### 1. Verificación de Archivos

```bash
# En la terminal, desde la raíz del proyecto:
ls -lh Assets/Scripts/MusicManager.cs
ls -lh Assets/Prefabs/MusicManager.prefab
```

**Resultado esperado:**
```
-rw-r--r--  1 user  staff   673B  Assets/Scripts/MusicManager.cs
-rw-r--r--  1 user  staff   3.5K  Assets/Prefabs/MusicManager.prefab
```

#### 2. Verificación de Escenas

```bash
# Verificar que cada escena contiene el MusicManager
grep -c "MusicManager" Assets/Scenes/CabinMap.unity
grep -c "MusicManager" Assets/Scenes/ChurchMap.unity
grep -c "MusicManager" Assets/Scenes/LakeMap.unity
grep -c "MusicManager" Assets/Scenes/MountainMap.unity
grep -c "MusicManager" Assets/Scenes/NorthWestMap.unity
grep -c "MusicManager" Assets/Scenes/Game.unity
```

**Resultado esperado:**
```
1
1
1
1
1
1
```

Cada escena debe tener exactamente **1 referencia** a MusicManager.

#### 3. Prueba Funcional en Unity

**Test 1: Música en Escena Única**

1. Abrir `Assets/Scenes/CabinMap.unity`
2. Presionar Play (▶)
3. **Resultado esperado:** Música empieza a sonar automáticamente
4. Detener Play (■)

**Test 2: Persistencia entre Escenas**

1. Abrir `Assets/Scenes/MainMenu.unity`
2. Presionar Play (▶)
3. Hacer clic en "Play" para cargar `CabinMap`
4. **Resultado esperado:**
   - Música cambia de la del menú a la del juego (si son diferentes)
   - O continúa sonando (si es la misma)
5. Abrir Window → Scene → ChurchMap (mientras está en Play Mode)
6. **Resultado esperado:** Música continúa sin interrupciones

**Test 3: Prevención de Duplicados**

1. Abrir `Assets/Scenes/CabinMap.unity`
2. Presionar Play (▶)
3. Abrir `Window → Hierarchy`
4. Buscar "DontDestroyOnLoad" en la jerarquía
5. **Resultado esperado:** Solo 1 GameObject `MusicManager` dentro de `DontDestroyOnLoad`
6. Cambiar a otra escena durante Play Mode
7. **Resultado esperado:** Sigue habiendo solo 1 `MusicManager`

**Test 4: Control de Volumen**

1. Abrir `Assets/Scenes/MainMenu.unity`
2. Presionar Play (▶)
3. Abrir el menú de Opciones
4. Mover el slider de volumen de música
5. **Resultado esperado:** El volumen cambia en tiempo real
6. Iniciar el juego (cargar CabinMap)
7. **Resultado esperado:** El volumen configurado se mantiene

---

### Casos de Prueba Detallados

#### Caso 1: Primera Carga del Juego

**Precondiciones:**
- Ninguna escena cargada previamente
- Proyecto recién abierto en Unity

**Pasos:**
1. Abrir `CabinMap.unity`
2. Presionar Play

**Resultado Esperado:**
- `MusicManager` se instancia
- `Awake()` se ejecuta
- `instance == null` (primera vez)
- `instance = this`
- `DontDestroyOnLoad(gameObject)` se ejecuta
- Música empieza a sonar
- GameObject aparece en `DontDestroyOnLoad` en la jerarquía

**Resultado Real:** ✅ Pasó / ❌ Falló

---

#### Caso 2: Cambio de Escena (Primera vez)

**Precondiciones:**
- `CabinMap.unity` cargada
- `MusicManager` existente y sonando

**Pasos:**
1. Cambiar a `ChurchMap.unity` (via SceneManager.LoadScene o manualmente)

**Resultado Esperado:**
- Unity intenta instanciar el `MusicManager` del prefab en `ChurchMap`
- `Awake()` del nuevo MusicManager se ejecuta
- `instance != null` (ya existe uno)
- `Destroy(gameObject)` se ejecuta inmediatamente
- El nuevo MusicManager se destruye ANTES de que empiece a sonar
- El MusicManager original continúa sonando
- Solo 1 `MusicManager` visible en la jerarquía

**Resultado Real:** ✅ Pasó / ❌ Falló

---

#### Caso 3: Ciclo Completo de Escenas

**Precondiciones:**
- Proyecto en estado inicial

**Pasos:**
1. Cargar `MainMenu.unity` → Play
2. Iniciar juego → cargar `CabinMap.unity`
3. Cambiar a `ChurchMap.unity`
4. Cambiar a `LakeMap.unity`
5. Cambiar a `MountainMap.unity`
6. Cambiar a `NorthWestMap.unity`
7. Cambiar a `Game.unity`
8. Regresar a `CabinMap.unity`

**Resultado Esperado:**
- En cada transición, la música continúa sin interrupciones
- En cada transición, solo existe 1 `MusicManager`
- No hay picos de audio ni cortes
- El volumen se mantiene consistente

**Resultado Real:** ✅ Pasó / ❌ Falló

---

#### Caso 4: Recarga de la Misma Escena

**Precondiciones:**
- `CabinMap.unity` cargada
- `MusicManager` existente

**Pasos:**
1. Ejecutar `SceneManager.LoadScene("CabinMap")`
2. (La misma escena se recarga)

**Resultado Esperado:**
- La escena se recarga completamente
- Unity intenta crear un nuevo `MusicManager`
- Singleton detecta duplicado
- Nuevo `MusicManager` se destruye
- El original persiste
- Música continúa sonando sin reiniciar

**Resultado Real:** ✅ Pasó / ❌ Falló

---

## 🐛 Troubleshooting

### Problema 1: "La música no suena en ninguna escena"

**Síntomas:**
- Silencio total al dar Play
- AudioSource aparece en Inspector pero sin sonido

**Posibles Causas y Soluciones:**

| Causa | Solución |
|-------|----------|
| AudioClip no asignado | 1. Abrir el prefab `MusicManager`<br>2. Verificar que `Audio Source → AudioClip` tiene un archivo<br>3. Si está vacío, arrastrar un archivo de audio |
| AudioMixer en mute o volumen -80dB | 1. Window → Audio → Audio Mixer<br>2. Verificar que el volumen no está en -80dB<br>3. Ajustar a -10dB o 0dB |
| `Play On Awake` desmarcado | 1. Abrir el prefab `MusicManager`<br>2. En `Audio Source`, marcar `Play On Awake` |
| `Mute` marcado | 1. Abrir el prefab `MusicManager`<br>2. En `Audio Source`, desmarcar `Mute` |

---

### Problema 2: "La música se duplica / se escuchan 2 audios al mismo tiempo"

**Síntomas:**
- Al cambiar de escena, se escucha audio "doblado"
- Dos capas de música sonando simultáneamente

**Causa Probable:**
El Singleton no está funcionando correctamente.

**Diagnóstico:**
1. Durante Play Mode, abrir `Window → Hierarchy`
2. Expandir la sección `DontDestroyOnLoad`
3. Contar cuántos `MusicManager` hay

**Si hay 2 o más MusicManagers:**

**Solución 1:** Verificar el script
```csharp
// Abrir Assets/Scripts/MusicManager.cs
// Verificar que el código sea EXACTAMENTE:

void Awake()
{
    if (instance != null)
    {
        Destroy(gameObject);
        return;
    }

    instance = this;
    DontDestroyOnLoad(gameObject);
}
```

**Solución 2:** Recompilar el script
1. Hacer un cambio mínimo en `MusicManager.cs` (agregar un espacio)
2. Guardar (Ctrl+S)
3. Esperar a que Unity recompile
4. Deshacer el cambio
5. Guardar nuevamente

**Solución 3:** Verificar que el script está asignado al prefab
1. Abrir `Assets/Prefabs/MusicManager.prefab`
2. Verificar que el componente `MusicManager (Script)` existe
3. Verificar que `Script` no dice `None` o `Missing`

---

### Problema 3: "La música se detiene al cambiar de escena"

**Síntomas:**
- Música suena en la primera escena
- Al cambiar a otra escena, silencio

**Causa Probable:**
`DontDestroyOnLoad` no se está ejecutando.

**Diagnóstico:**
1. Agregar un Debug.Log para verificar:

```csharp
void Awake()
{
    Debug.Log("MusicManager Awake llamado");

    if (instance != null)
    {
        Debug.Log("Instancia duplicada detectada, destruyendo...");
        Destroy(gameObject);
        return;
    }

    instance = this;
    DontDestroyOnLoad(gameObject);
    Debug.Log("MusicManager marcado como DontDestroyOnLoad");
}
```

2. Presionar Play
3. Abrir `Window → Console`
4. Verificar los mensajes

**Solución si no aparece "marcado como DontDestroyOnLoad":**
- El script no está siendo ejecutado
- Verificar que `MusicManager.cs` está asignado al prefab
- Verificar que no hay errores de compilación

---

### Problema 4: "Error: NullReferenceException en MusicManager"

**Mensaje de Error:**
```
NullReferenceException: Object reference not set to an instance of an object
MusicManager.Awake () (at Assets/Scripts/MusicManager.cs:XX)
```

**Causa:**
Posible error de sintaxis o variable no inicializada.

**Solución:**
1. Abrir `MusicManager.cs`
2. Verificar que NO estés accediendo a ninguna variable antes de inicializarla
3. El script debe ser EXACTAMENTE como el proporcionado en este documento
4. Si has modificado el script, compara con la versión original

---

### Problema 5: "El prefab aparece como 'Missing' en las escenas"

**Síntomas:**
- En la jerarquía aparece un GameObject con icono roto
- Inspector dice "Missing Prefab"

**Causa:**
El archivo `.meta` del prefab se corrompió o el GUID cambió.

**Solución 1: Reconectar el prefab**
1. Eliminar el GameObject `MusicManager` de TODAS las escenas modificadas
2. Guardar las escenas
3. Volver a arrastrar el prefab a cada escena
4. Guardar nuevamente

**Solución 2: Verificar el archivo .meta**
1. Verificar que existe `Assets/Prefabs/MusicManager.prefab.meta`
2. Verificar que el GUID es `f1e2d3c4b5a697887766554433221100`
3. Si el GUID es diferente, actualizar las escenas con el nuevo GUID

---

### Problema 6: "Conflicto con el GameObject 'musica' de MainMenu"

**Síntomas:**
- En MainMenu hay 2 sistemas de música
- Música duplicada

**Solución Recomendada:**

**Opción A: Eliminar el GameObject "musica" de MainMenu**
1. Abrir `MainMenu.unity`
2. Seleccionar el GameObject "musica"
3. Eliminar (Delete)
4. Arrastrar el prefab `MusicManager` a la jerarquía
5. Guardar

**Opción B: Agregar el script MusicManager al GameObject "musica" existente**
1. Abrir `MainMenu.unity`
2. Seleccionar el GameObject "musica"
3. En Inspector, `Add Component → MusicManager`
4. Guardar

⚠️ **Importante:** Elegir SOLO UNA opción, no ambas.

---

## 🔮 Mantenimiento Futuro

### Agregar Música Diferente por Escena

Si en el futuro quieren música diferente para cada escena (ej. música tensa en ChurchMap, música tranquila en LakeMap):

**Enfoque 1: Múltiples Prefabs**

1. Duplicar el prefab:
   - `MusicManager_Calm.prefab` (música tranquila)
   - `MusicManager_Tense.prefab` (música tensa)
   - `MusicManager_Action.prefab` (música de acción)

2. Asignar diferentes AudioClips a cada prefab

3. En cada escena, usar el prefab correspondiente

⚠️ **Limitación:** El Singleton destruirá todos excepto el primero. Necesitarás modificar el script.

**Enfoque 2: Cambio Dinámico de Música**

Modificar `MusicManager.cs`:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private AudioSource audioSource;

    [System.Serializable]
    public class SceneMusic
    {
        public string sceneName;
        public AudioClip musicClip;
    }

    [SerializeField]
    private SceneMusic[] sceneMusicList;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Buscar la música correspondiente a la escena
        foreach (SceneMusic sm in sceneMusicList)
        {
            if (sm.sceneName == scene.name)
            {
                // Cambiar la música si es diferente
                if (audioSource.clip != sm.musicClip)
                {
                    audioSource.Stop();
                    audioSource.clip = sm.musicClip;
                    audioSource.Play();
                }
                return;
            }
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
```

**Configuración en Unity:**
1. Seleccionar el prefab `MusicManager`
2. En el Inspector, en `Scene Music List`:
   - Size: 6
   - Element 0: Scene Name = "CabinMap", Music Clip = (tu audio)
   - Element 1: Scene Name = "ChurchMap", Music Clip = (tu audio)
   - etc.

---

### Agregar Efectos de Transición (Fade In/Out)

Para música que hace fade suave entre escenas:

```csharp
using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private AudioSource audioSource;
    public float fadeDuration = 1.0f;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
    }

    public void ChangeMusic(AudioClip newClip)
    {
        StartCoroutine(ChangeMusicWithFade(newClip));
    }

    private IEnumerator ChangeMusicWithFade(AudioClip newClip)
    {
        // Fade out
        float startVolume = audioSource.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0;

        // Cambiar clip
        audioSource.clip = newClip;
        audioSource.Play();

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = startVolume;
    }
}
```

**Uso:**
```csharp
// Desde cualquier script
MusicManager manager = FindObjectOfType<MusicManager>();
manager.ChangeMusic(nuevoClipDeAudio);
```

---

### Optimización: Streaming vs Loaded

Para archivos de música grandes (>1 MB):

1. Seleccionar el archivo de audio en Unity
2. En Inspector:
   - `Load Type`: **Streaming** (no carga todo en RAM)
   - `Compression Format`: **Vorbis**
   - `Quality`: 70%

**Beneficios:**
- Menor uso de RAM
- Carga más rápida de escenas
- Sin diferencia audible en calidad

---

### Integración con Sistema de Eventos

Si tienen un `EventManager` o sistema similar:

```csharp
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private AudioSource audioSource;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();

        // Suscribirse a eventos globales
        EventManager.Instance.OnBossFight += PlayBossMusic;
        EventManager.Instance.OnBossDefeated += PlayNormalMusic;
    }

    void OnDestroy()
    {
        // Desuscribirse para evitar memory leaks
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnBossFight -= PlayBossMusic;
            EventManager.Instance.OnBossDefeated -= PlayNormalMusic;
        }
    }

    private void PlayBossMusic()
    {
        // Cambiar a música épica de jefe
    }

    private void PlayNormalMusic()
    {
        // Volver a música normal
    }
}
```

---

## 📞 Contacto y Soporte

### Desarrollador Responsable

**Nombre:** Roberto Israel Flores Reza
**Usuario GitHub:** @dev-isra
**Rama de Desarrollo:** `dev-isra`
**Email:** 20223tn016@utez.edu.mx

### Para Reportar Problemas

Si encuentran bugs o problemas con el sistema de música:

1. **Crear un Issue en GitHub:**
   - Ir a la pestaña "Issues"
   - Click en "New Issue"
   - Título: `[MUSICA] Descripción breve del problema`
   - Descripción:
     ```
     **Descripción del Problema:**
     [Explicar qué está mal]

     **Pasos para Reproducir:**
     1. [Paso 1]
     2. [Paso 2]
     3. [Paso 3]

     **Resultado Esperado:**
     [Qué debería pasar]

     **Resultado Actual:**
     [Qué pasa realmente]

     **Logs de Consola:**
     [Pegar errores de la consola de Unity]

     **Escena Afectada:**
     [Nombre de la escena]
     ```

2. **Etiquetar al responsable:**
   - Mencionar a `@dev-isra` en el issue

3. **Prioridad:**
   - 🔴 Crítico: La música no funciona en ninguna escena
   - 🟡 Media: La música se corta ocasionalmente
   - 🟢 Baja: Mejoras o features nuevas

---

## 📚 Referencias y Recursos

### Documentación de Unity

- **AudioSource:** https://docs.unity3d.com/ScriptReference/AudioSource.html
- **DontDestroyOnLoad:** https://docs.unity3d.com/ScriptReference/Object.DontDestroyOnLoad.html
- **Singleton Pattern:** https://unity.com/how-to/create-modular-and-maintainable-code-unity
- **Prefabs:** https://docs.unity3d.com/Manual/Prefabs.html
- **SceneManager:** https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.html

### Tutoriales Recomendados

- **Unity Audio Best Practices:** https://learn.unity.com/tutorial/audio-best-practices
- **Singleton Pattern en Unity:** https://gamedevbeginner.com/singletons-in-unity-the-right-way/
- **Music Management System:** https://www.youtube.com/watch?v=QL29aTa7J5Q (Brackeys)

---

## 📄 Historial de Cambios

| Versión | Fecha | Autor | Cambios |
|---------|-------|-------|---------|
| 1.0.0 | 2024-12-04 | Roberto Flores (@dev-isra) | - Creación inicial del sistema<br>- Implementación de MusicManager.cs<br>- Creación del prefab MusicManager<br>- Integración en 6 escenas<br>- Documentación completa |

---

## ✅ Checklist de Revisión de Código (Para Code Review)

Cuando revisen este PR, verificar:

### Funcionalidad
- [ ] La música suena correctamente en todas las escenas
- [ ] No hay duplicados de música al cambiar escenas
- [ ] El Singleton funciona correctamente
- [ ] DontDestroyOnLoad está implementado

### Código
- [ ] El código sigue las convenciones del proyecto
- [ ] Los comentarios son claros y útiles
- [ ] No hay código comentado sin usar
- [ ] No hay errores de compilación
- [ ] No hay warnings en la consola

### Archivos
- [ ] Todos los archivos nuevos tienen su .meta
- [ ] Los GUIDs son únicos y correctos
- [ ] No hay archivos binarios innecesarios en el commit

### Escenas
- [ ] Todas las escenas modificadas se guardaron correctamente
- [ ] Los FileIDs son únicos por escena
- [ ] SceneRoots incluyen la referencia a MusicManager

### Documentación
- [ ] Este README está completo y actualizado
- [ ] Los comentarios en el código son suficientes
- [ ] Los casos de uso están documentados

---

## 🎓 Glosario de Términos

Para que todos en el equipo entiendan la documentación:

| Término | Definición |
|---------|------------|
| **Singleton** | Patrón de diseño que garantiza una sola instancia de una clase |
| **DontDestroyOnLoad** | Función de Unity que previene que un GameObject sea destruido al cambiar escenas |
| **Prefab** | "Molde" reutilizable de un GameObject en Unity |
| **AudioSource** | Componente de Unity que reproduce audio |
| **AudioMixer** | Sistema de Unity para controlar y mezclar múltiples fuentes de audio |
| **GUID** | Global Unique Identifier - Identificador único usado por Unity para referenciar assets |
| **FileID** | Identificador único de un objeto dentro de una escena de Unity |
| **SceneRoots** | Lista de GameObjects de nivel raíz en una escena |
| **MonoBehaviour** | Clase base de Unity de la que heredan todos los scripts |
| **Awake()** | Método de Unity que se llama cuando un GameObject se activa (antes que Start) |
| **Component** | Pieza funcional que se agrega a un GameObject (Script, AudioSource, Transform, etc.) |

---

## 🎯 Conclusión

Este sistema de música persistente mejora significativamente la experiencia de usuario al proporcionar una banda sonora continua e ininterrumpida a lo largo del juego.

La implementación usa patrones de diseño estándar de Unity (Singleton, Prefabs, DontDestroyOnLoad) que son escalables y fáciles de mantener.

**Próximos pasos sugeridos:**
1. Revisar y aprobar este PR
2. Probar exhaustivamente en Unity
3. Considerar agregar música diferente por escena (futuro)
4. Implementar efectos de fade in/out (futuro)

---

**Fecha de última actualización:** 4 de Diciembre, 2024
**Versión del documento:** 1.0.0
**Estado:** ✅ Completado y Listo para Revisión

---

**- Roberto Israel Flores Reza (@dev-isra)**
