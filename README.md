# Rhytm Mania

Videojuego de ritmo desarrollado en Unity.

## Requisitos para Compilar

### Unity
- **Versión**: Unity 6 (6000.0.75f1)

### Assets de Terceros Requeridos

Los siguientes assets son necesarios para compilar el proyecto pero **no están incluidos en el repositorio** por ser de pago o privados:

#### Assets Excluidos (deben adquirirse por separado)
- **[Feel](https://assetstore.unity.com/packages/tools/particles-effects/feel-183370)** (MoreMountains) - Sistema de feedback y efectos
- **[AssetInventory](https://assetstore.unity.com/packages/tools/utilities/asset-inventory-163892)** - Gestión de inventario de assets
- **[NiceVibrations](https://assetstore.unity.com/packages/tools/integration/nice-vibrations-by-lofelt-haptic-feedback-for-mobile-and-game-200552)** (Lofelt) - Feedback háptico
- **[DryWetMIDI](https://github.com/melanchall/drywetmidi)** (Melanchall) - Procesamiento de archivos MIDI

#### Assets Incluidos
- **TextMesh Pro** - Renderizado de texto avanzado (incluido con Unity)
- **RhythmEngine** - Motor de ritmo personalizado
- [GameSaveSystem for Unity](https://github.com/AlextintorDev/GameSaveSystem-For-Unity) - Para la gestión de datos de juego.

## Plataformas Soportadas

- **Windows** (Standalone)
- **Linux** (Standalone)

### Ubicación de Archivos de Guardado

Los archivos se almacenan en `Application.persistentDataPath`:

**Windows:**
```
C:\Users\[Usuario]\AppData\LocalLow\Kibobyte\Rhytm Mania\
```

**Linux:**
```
~/.config/unity3d/Kibobyte/Rhytm Mania/
```

### Archivos de Leaderboard

Cada canción tiene su propio archivo de leaderboard con extensión `.leaderboard`:

```
[songCode].leaderboard
```

**Formato del archivo:**
```
Jugador1;1000
Jugador2;850
Jugador3;1200
```

Cada línea contiene: `nombre;puntuación`

### Datos de Sesión

El sistema `AnalyticsRecorder` captura datos detallados de cada sesión de juego:

- Nombre del jugador
- Configuración de game feel activa
- Intentos por nivel
- Puntuaciones, combos máximos y ghost notes
- Datos de cada hit (tipo y diferencia en milisegundos)

Estos datos pueden exportarse en formato JSON para análisis externo.

## Estructura del Proyecto

```
Assets/
├── Scripts/              # Lógica del juego
│   ├── Core/             # Gestión principal del juego
│   │   ├── GameManager.cs
│   │   ├── LevelManager.cs
│   │   ├── BeatScroller.cs
│   │   ├── Note.cs
│   │   ├── InputKey.cs
│   │   ├── GameSettings.cs
│   │   ├── PauseController.cs
│   │   └── CameraController.cs
│   ├── Scoring/          # Sistema de puntuación
│   │   ├── ScoreManager.cs
│   │   └── LeaderboardController.cs
│   ├── UI/               # Interfaces de usuario
│   │   ├── GameUI.cs
│   │   ├── InitialUI.cs
│   │   ├── MainMenuController.cs
│   │   ├── LogoUI.cs
│   │   ├── NameInput.cs
│   │   ├── VersionText.cs
│   │   └── ButtonSoundPlayer.cs
│   ├── VisualEffects/    # Efectos visuales y feedback
│   │   ├── FeedbackController.cs
│   │   ├── IFeedbackPlayer.cs
│   │   ├── ParticleManager.cs
│   │   ├── GlowEffectController.cs
│   │   ├── LightsManager.cs
│   │   ├── LightController.cs
│   │   └── LowLightController.cs
│   ├── Media/            # Audio y video
│   │   ├── BackgroundPlayerController.cs
│   │   └── BackgroundDatabase.cs
│   ├── Analytics/        # Registro de datos
│   │   ├── AnalyticsRecorder.cs
│   │   └── Metadata.cs
│   └── Debug/            # Herramientas de desarrollo
│       ├── DebugDPSTimeViewer.cs
│       ├── NoteRecorder.cs
│       └── CodeButton.cs
├── Scenes/               # Escenas de Unity
├── Prefabs/              # Prefabricados
├── Materials/            # Materiales y shaders
├── Songs/                # Datos de canciones
├── Sound/                # Audio y música
├── Sprites/              # Gráficos 2D
├── Video/                # Videos de fondo
├── Resources/            # Assets cargados en runtime
└── ThirdParty/           # Assets de terceros
    ├── Private/          # Assets privados (excluidos del repo)
    └── Public/           # Assets públicos
```

## Configuración del Juego

La configuración principal se encuentra en `Assets/Resources/GameSettings.asset`:

- **Note Speed**: Velocidad de las notas
- **First Beat Offset**: Offset inicial del primer beat
- **Perfect Note Threshold**: Ventana de tiempo para hits perfectos (ms)
- **Good Note Threshold**: Ventana de tiempo para hits buenos (ms)
- **Game Feel**: Activar/desactivar efectos de feedback

## Créditos

**Desarrollado por**: Kibobyte

### Assets de Terceros
- [Feel](https://assetstore.unity.com/packages/tools/particles-effects/feel-183370) por MoreMountains
- [GameSaveSystem](https://github.com/AlextintorDev/GameSaveSystem-For-Unity) por AlextintorDev
- [DryWetMIDI](https://github.com/melanchall/drywetmidi) por Melanchall
- [Rhythm Game Template](https://takabro.itch.io/rhythm-game-temp) por TakaBro
