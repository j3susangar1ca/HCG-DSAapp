<div align="center">

<img src="https://raw.githubusercontent.com/microsoft/TemplateStudio/main/docs/WinUI/images/banner.png" alt="DSAapp Banner" width="100%" />

# 🏢 DSAapp

**Sistema Institucional de Gestión y Control de Oficios**

[![WinUI 3](https://img.shields.io/badge/WinUI-3.0-blue?style=for-the-badge&logo=windows)](https://docs.microsoft.com/windows/apps/winui/winui3/)
[![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![MVVM Toolkit](https://img.shields.io/badge/Architecture-MVVM-orange?style=for-the-badge)](https://learn.microsoft.com/windows/communitytoolkit/mvvm/introduction)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-SQLite-green?style=for-the-badge&logo=sqlite)](https://learn.microsoft.com/ef/core/)

*Una aplicación de escritorio moderna diseñada con **WinUI 3** y principios de diseño **Fluent Design 2**, enfocada en optimizar los flujos de trabajo administrativos, el registro de oficios y el acceso a los sistemas internos de la institución.*

---

</div>

## ✨ Características Principales

DSAapp está estructurada mediante un modelo de navegación intuitivo (Navigation View) que proporciona acceso rápido a todas las herramientas esenciales:

| Herramienta | Descripción |
| :--- | :--- |
| 📊 **Panel Principal** | *Dashboard interactivo* con el resumen de tareas pendientes y el estado general de los trámites. |
| 📝 **Registro de Oficios** | Módulo de *captura y gestión* de oficios institucionales. Simplifica el flujo de documentos. |
| 🌐 **Intranet SII** | Acceso directo y rápido al *Sistema de Información Institucional* a través de una vista web integrada. |
| 🖨️ **Escáner** | Utilidad para digitalizar documentos utilizando la *cámara / escáner local* (Soporte de Webcam y Biblioteca de Imágenes). |
| 🗃️ **Cuadrícula de Datos** | Visualización avanzada de datos tabulares (DataGrids) para un *análisis profundo* y exportación de información. |
| 🖼️ **Cuadrícula de Contenido** | Interfaz visual tipo *galería* para explorar colecciones de documentos o elementos visuales. |
| 📋 **Detalles de Lista** | Flujo maestro-detalle (Master/Detail) para visualizar *información específica* de un registro en la misma pantalla. |

## 🛠️ Tecnologías Utilizadas

La arquitectura de la aplicación está construida sobre tecnologías modernas de Microsoft para garantizar rendimiento, mantenibilidad y una experiencia nativa fluida:

*   **[Windows App SDK (WinUI 3)](https://github.com/microsoft/microsoft-ui-xaml)**: El framework de UI nativo más moderno para aplicaciones de Windows.
*   **.NET 9**: La última versión de la plataforma unificada, rápida y moderna.
*   **[CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)**: Para una separación limpia de las responsabilidades (Model-View-ViewModel).
*   **[Entity Framework Core (SQLite)](https://learn.microsoft.com/ef/core/)**: ORM ligero y local para el almacenamiento seguro y rápido de los datos y oficios registrados.
*   **[CommunityToolkit.WinUI.UI.Controls](https://github.com/CommunityToolkit/WindowsCommunityToolkit)**: Colección de controles avanzados como el DataGrid.
*   **Template Studio**: Generación inicial de la base arquitectónica siguiendo las mejores prácticas de Microsoft.

## 🚀 Empezando (Getting Started)

### Prerrequisitos

Para compilar y ejecutar este proyecto, necesitarás:

1.  [Visual Studio 2022 (Versión 17.10 o superior)](https://visualstudio.microsoft.com/)
2.  Carga de trabajo: **Desarrollo de la Plataforma universal de Windows** o **Desarrollo de escritorio con .NET**.
3.  [Windows App SDK](https://learn.microsoft.com/windows/apps/windows-app-sdk/set-up-your-development-environment) instalado.
4.  SDK de .NET 9.

### Instalación y Ejecución

1. Clona el repositorio:
   ```bash
   git clone https://github.com/TuUsuario/DSAapp.git
   ```
2. Abre la solución `DSAapp.slnx` en Visual Studio.
3. Restaura los paquetes NuGet (usualmente automático al abrir la solución).
4. Selecciona tu arquitectura de destino (`x64` recomendado).
5. Compila y Ejecuta (`F5`).

> **Nota para Desarrolladores:**
> Revisa los comentarios `TODO:` en `Ver -> Lista de tareas` (Task List) en Visual Studio para entender los siguientes pasos requeridos para conectar la capa visual con la capa de datos de producción.

## 📦 Empaquetado y Distribución

Este proyecto está configurado para empaquetarse como **MSIX**.

1. Haz clic derecho sobre el proyecto `DSAapp` en el *Explorador de Soluciones*.
2. Selecciona **Empaquetar y publicar -> Crear paquetes de aplicaciones...** (*Package and Publish -> Create App Packages...*).
3. Sigue el asistente para generar el paquete MSIX o subirlo directamente a la Microsoft Store (Requiere certificado firmado `DSAapp_TemporaryKey.pfx`).

Adicionalmente, el proyecto soporta despliegue auto-contenido (`Self-Contained`):
```xml
<WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>
```

## 📸 Capturas de Pantalla

*(Reemplaza los enlaces con imágenes reales de tu aplicación una vez en producción)*

<div align="center">
  <img src="https://raw.githubusercontent.com/microsoft/TemplateStudio/main/docs/WinUI/images/navview.png" width="45%" alt="Navegación Principal" />
  &nbsp; &nbsp; &nbsp;
  <img src="https://raw.githubusercontent.com/microsoft/TemplateStudio/main/docs/WinUI/images/datagrid.png" width="45%" alt="Cuadrícula de Datos" />
</div>

## 📂 Estructura del Proyecto

El proyecto está diseñado de forma modular separando la interfaz visual (`DSAapp`) del núcleo de lógica (`DSAapp.Core`):

```text
📦 DSAapp Solution
 ┣ 📂 DSAapp (Proyecto UI - WinUI 3)
 ┃ ┣ 📂 Assets         # Iconos, imágenes y logos (Splashscreen, StoreLogo)
 ┃ ┣ 📂 Behaviors      # Comportamientos XAML interactivos
 ┃ ┣ 📂 Helpers        # Funciones y extensiones de ayuda (Navegación)
 ┃ ┣ 📂 Models         # Clases y estructuras de datos locales
 ┃ ┣ 📂 Services       # Servicios de UI (Tema, Dialogos, Configuración)
 ┃ ┣ 📂 ViewModels     # Lógica de presentación (MvvmToolkit)
 ┃ ┗ 📂 Views          # Archivos XAML (ShellPage, MainPage, OficiosPage...)
 ┃
 ┗ 📂 DSAapp.Core (Proyecto de Librería - Lógica de Negocio)
   ┣ 📂 Contracts      # Interfaces y abstracciones compartidas
   ┣ 📂 Models         # Modelos de dominio y Entidades (EF Core)
   ┗ 📂 Services       # Servicios de base de datos (SQLite) y lógica pura
```

## 🔐 Permisos y Capacidades

La aplicación solicita los siguientes permisos a nivel de sistema (`Package.appxmanifest`):

*   🌐 **Cliente de Internet** (`internetClient`): Requerido para acceder a la Intranet SII.
*   📷 **Cámara Web** (`webcam`): Requerido para la funcionalidad del Escáner.
*   🖼️ **Biblioteca de Imágenes** (`picturesLibrary`): Para el almacenamiento y selección de documentos digitalizados o fotografías.

## 🤝 Contribuir

¡Las contribuciones son bienvenidas!

1. Realiza un *Fork* del proyecto.
2. Crea tu rama de características (`git checkout -b feature/NuevaCaracteristica`).
3. Haz *Commit* de tus cambios (`git commit -m 'Añade una Nueva Caracteristica'`).
4. Sube a la rama (`git push origin feature/NuevaCaracteristica`).
5. Abre un *Pull Request*.

## 📄 Licencia

Este proyecto se distribuye bajo la licencia **MIT**. Consulta el archivo `LICENSE` para obtener más información.

---
<div align="center">
Desarrollado con ❤️ para optimizar la gestión institucional.
</div>