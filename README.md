﻿# MigrantsExhibition

![MigrantsExhibition Banner](Content/Images/banner.png) <!-- Replace with your actual banner image -->

**MigrantsExhibition** is a dynamic simulation built using **MonoGame**, implementing Conway's Game of Life with enhanced visual effects to portray depth and movement within a galaxy. The application responds to sound intensity, influencing both the simulation's behavior and visual elements, providing an immersive and interactive experience.

---

## Table of Contents

1. [Features](#features)
2. [Directory Structure](#directory-structure)
3. [Prerequisites](#prerequisites)
4. [Installation](#installation)
   - [Cloning the Repository](#cloning-the-repository)
   - [Building the Project](#building-the-project)
5. [Configuration](#configuration)
   - [Modifying Constants](#modifying-constants)
6. [Running the Application](#running-the-application)
   - [Running Locally](#running-locally)
   - [Running with Docker](#running-with-docker)
7. [Controls and Interactions](#controls-and-interactions)
   - [Key Bindings](#key-bindings)
8. [Troubleshooting](#troubleshooting)
9. [Logging](#logging)
10. [Contributing](#contributing)
11. [License](#license)
12. [Acknowledgements](#acknowledgements)
13. [Contact](#contact)

---

## Features

- **Conway's Game of Life:** Implements classic cellular automata with rules for cell survival, death, and birth.
- **Depth Illusion:** Utilizes multiple layers with varying transparency and movement speeds to create a sense of depth.
- **Sound Reactive:** Adjusts simulation behavior and visual effects based on real-time sound intensity.
- **Interactive Controls:** Allows toggling between full-screen and windowed modes, as well as minimizing and restoring the application window.
- **Logging:** Comprehensive logging system to monitor application behavior and debug issues.
- **Docker Support:** Containerized setup ensuring the application can run consistently across different environments.

---

## Directory Structure

Understanding the project's directory structure is crucial for effective navigation and modification. Below is an overview of the project's layout:

```
MigrantsExhibition/
├── .gitattributes
├── .gitignore
├── MigrantsExhibition.sln
├── README.md
├── Dockerfile
├── MigrantsExhibition/
│   ├── .config/
│   ├── bin/
│   ├── Content/
│   │   ├── Images/
│   │   ├── Fonts/
│   │   └── ... other content files ...
│   ├── Fonts/
│   ├── Logs/
│   ├── obj/
│   ├── Src/
│   │   ├── AudioHandler.cs
│   │   ├── Cell.cs
│   │   ├── Constants.cs
│   │   ├── GameOfLife.cs
│   │   ├── GUI.cs
│   │   ├── ImageLoader.cs
│   │   ├── Layer.cs
│   │   ├── Star.cs
│   │   └── Utils.cs
│   ├── app.manifest
│   ├── Game1.cs
│   ├── Icon.ico
│   ├── MigrantsExhibition.csproj
│   └── Program.cs
└── publish/
    ├── win-x64/
    ├── linux-x64/
    └── osx-x64/
```

- **Root Directory (`MigrantsExhibition/`):**
  - `.gitattributes`: Git attributes configuration.
  - `.gitignore`: Specifies intentionally untracked files to ignore.
  - `MigrantsExhibition.sln`: Solution file containing project configurations.
  - `README.md`: This documentation file.
  - `Dockerfile`: Configuration file for building the Docker image.
  
- **Project Directory (`MigrantsExhibition/MigrantsExhibition/`):**
  - `.config/`: Configuration files.
  - `bin/`: Compiled binaries.
  - `Content/`: Contains all content assets like images and fonts.
    - `Images/`: Image assets used in the application.
    - `Fonts/`: Font files for GUI text rendering.
  - `Fonts/`: Additional font resources.
  - `Logs/`: Stores log files generated by the application.
  - `obj/`: Object files generated during the build process.
  - `Src/`: Source code files.
    - `AudioHandler.cs`: Manages audio input and sound intensity analysis.
    - `Cell.cs`: Represents individual cells in the Game of Life simulation.
    - `Constants.cs`: Centralized location for all constant values used throughout the project.
    - `GameOfLife.cs`: Handles the logic for the Game of Life simulation.
    - `GUI.cs`: Manages the graphical user interface elements.
    - `ImageLoader.cs`: Responsible for loading image assets.
    - `Layer.cs`: Manages different simulation layers for depth illusion.
    - `Star.cs`: Represents stars in the background, contributing to the depth illusion.
    - `Utils.cs`: Utility class for logging and other helper functions.
  - `app.manifest`: Application manifest file.
  - `Game1.cs`: The main game class handling initialization, updates, and rendering.
  - `Icon.ico`: Application icon.
  - `MigrantsExhibition.csproj`: Project file containing configurations and dependencies.
  - `Program.cs`: Entry point of the application.
  
- **Publish Directory (`MigrantsExhibition/publish/`):**
  - `win-x64/`: Self-contained build for Windows.
  - `linux-x64/`: Self-contained build for Linux.
  - `osx-x64/`: Self-contained build for macOS.

---

## Prerequisites

Before setting up and running the **MigrantsExhibition** application, ensure you have the following installed on your system:

1. **.NET 6.0 SDK or Later:**
   - [Download .NET](https://dotnet.microsoft.com/download)
   
2. **MonoGame Framework:**
   - [Download MonoGame](https://www.monogame.net/downloads/)
   
3. **Git:**
   - [Download Git](https://git-scm.com/downloads)
   
4. **Docker (Optional for Containerization):**
   - [Download Docker](https://www.docker.com/get-started)
   
5. **VNC Client (If using Docker with GUI):**
   - [RealVNC](https://www.realvnc.com/en/connect/download/viewer/)
   - [TigerVNC](https://tigervnc.org/)

---

## Installation

### Cloning the Repository

1. **Open Terminal or Command Prompt.**

2. **Navigate to Your Desired Directory:**

   ```bash
   cd path/to/your/projects
   ```

3. **Clone the Repository:**

   ```bash
   git clone https://github.com/yourusername/MigrantsExhibition.git
   ```

   *Replace `https://github.com/yourusername/MigrantsExhibition.git` with your repository's actual URL.*

4. **Navigate to the Project Directory:**

   ```bash
   cd MigrantsExhibition
   ```

### Building the Project

1. **Restore Dependencies:**

   ```bash
   dotnet restore
   ```

2. **Build the Project:**

   ```bash
   dotnet build -c Release
   ```

   This compiles the project in Release mode, optimizing it for performance.

---

## Configuration

### Modifying Constants

All configurable values that influence the behavior and appearance of the application are centralized in the `Constants.cs` file. This design ensures ease of maintenance and scalability.

#### **Location:**

```
MigrantsExhibition/
└── Src/
    └── Constants.cs
```

#### **Key Constants and Their Descriptions:**

```csharp
public static class Constants
{
    // Cell/Image Settings
    public const float CellSizeLayer1 = 40f; // Size for the first layer
    public const float CellSizeLayer2 = CellSizeLayer1 * 0.9f; // 10% smaller than layer 1

    // Transparency Settings
    public const float CellOpacity = 1.0f; // Opacity for live cells (1.0f = fully opaque)
    public const float LayerOpacityDifference = 0.8f; // Layer 2 cells are 80% opaque compared to Layer 1

    // Movement and Frame Rate
    public const int TargetFPS = 48; // Target frame rate
    public const double GenerationIntervalLow = 1.0; // One generation per second at <=10% sound
    public const double GenerationIntervalHigh = 0.0; // No generation at >=70% sound
    public const double GenerationIntervalIncreasePer10Sound = 0.07; // 7% increase per 10% sound

    // Sound Intensity Thresholds (1-100 scale)
    public const float SoundThresholdLow = 10f; // 10%
    public const float SoundThresholdMedium = 20f; // 20%
    public const float SoundThresholdHigh = 70f; // 70%

    // Cell Population Control
    public const float MinLiveCellsPercentage = 15f; // Minimum 15% live cells

    // Vibration Parameters
    public const float VibrationIntensityHigh = 10f; // Max vibration at >=70% sound

    // Layers
    public const int TotalLayers = 3; // Three layers

    // Fade-In Parameters
    public const float FadeInDuration = 2.0f; // Fade-in duration in seconds

    // GUI Parameters
    public const int SoundMeterWidth = 200; // Width of the sound intensity meter
    public const int SoundMeterHeight = 20; // Height of the sound intensity meter

    // Star Parameters
    public const float StarBaseSpeedMultiplier = 15f; // Base speed multiplier for stars based on depth
    public const float StarSoundIntensityMultiplier = 7f; // Speed increase based on sound intensity
    public const float StarVibrationMultiplier = 3f; // Vibration strength for stars

    // Additional Parameters for Depth Illusion
    public const float ShadowOffset = 5f; // Pixels to offset Layer 1 shadows
    public const float GlowSizeMultiplier = 1.2f; // Multiplier for glow size
    public const float Layer1Elevation = 5f; // Pixels to offset Layer 1 cells upward
    public const float ShadowOpacity = 0.5f; // Opacity for shadows
    public const float GlowOpacity = 0.5f; // Opacity for glows

    // Layer Colors
    public static readonly Color Layer1Color = Color.Cyan;
    public static readonly Color Layer2Color = Color.MediumPurple;
    public static readonly Color Layer3Color = Color.LightGreen;
}
```

#### **How to Modify Constants:**

1. **Open `Constants.cs`:**

   Navigate to the `Src` directory and open `Constants.cs` using your preferred code editor.

   ```
   MigrantsExhibition/
   └── Src/
       └── Constants.cs
   ```

2. **Locate the Desired Constant:**

   Scroll through the file to find the constant you wish to modify. Constants are grouped logically based on their functionality, such as cell settings, transparency, movement, sound thresholds, etc.

3. **Edit the Constant Value:**

   For example, to change the target frame rate from `48` to `60` FPS:

   ```csharp
   public const int TargetFPS = 60; // Target frame rate
   ```

4. **Save the File:**

   After making changes, save the `Constants.cs` file.

5. **Rebuild the Project:**

   To apply the changes, rebuild the project:

   ```bash
   dotnet build -c Release
   ```

   *Alternatively, use your IDE's build functionality.*

#### **Common Modifications:**

- **Adjusting Cell Sizes:**
  - Increase or decrease `CellSizeLayer1` or `CellSizeLayer2` to change the size of cells in different layers.

- **Changing Opacity:**
  - Modify `CellOpacity` or `LayerOpacityDifference` to make cells more or less transparent.

- **Altering Movement Speed:**
  - Adjust `StarBaseSpeedMultiplier` or `StarSoundIntensityMultiplier` to influence the movement speed of stars based on depth and sound intensity.

- **Sound Thresholds:**
  - Change `SoundThresholdLow`, `SoundThresholdMedium`, or `SoundThresholdHigh` to redefine how sound intensity affects the simulation.

- **Vibration Intensity:**
  - Modify `VibrationIntensityHigh` to increase or decrease the vibration effect on stars and cells.

- **Fade-In Duration:**
  - Alter `FadeInDuration` to make the initial fade-in effect faster or slower.

---

## Running the Application

You can run the **MigrantsExhibition** application either locally on your machine or within a Docker container. Below are detailed instructions for both methods.

### Running Locally

This method involves running the application directly on your operating system without using Docker. It's the most straightforward approach and is recommended for most users.

#### **1. Navigate to the Project Directory:**

Open your Terminal or Command Prompt and navigate to the project directory:

```bash
cd path/to/MigrantsExhibition
```

#### **2. Restore Dependencies:**

Ensure all necessary packages are installed:

```bash
dotnet restore
```

#### **3. Build the Project:**

Compile the application in Release mode:

```bash
dotnet build -c Release
```

#### **4. Run the Application:**

You have two options to run the application:

- **Option A: Using `dotnet run`**

  ```bash
  dotnet run -c Release
  ```

- **Option B: Running the Executable Directly**

  Navigate to the output directory and run the executable:

  ```bash
  cd MigrantsExhibition/bin/Release/net6.0-windows/
  MigrantsExhibition.exe
  ```

  *Note: Replace `net6.0-windows` with your target framework if different.*

#### **5. Prevent System Sleep (Automatically Managed):**

When running locally, the application is configured to prevent your PC from sleeping or hibernating while it's active. You don't need to perform any additional steps.

### Running with Docker

Running **MigrantsExhibition** inside a Docker container ensures consistency across different environments. However, running GUI applications within Docker containers can be complex, especially regarding GUI and audio handling. This method is recommended for advanced users or specific use cases.

#### **a. Dockerfile Location**

Ensure that the `Dockerfile` is placed in the root directory of your project alongside the `.sln` file and `README.md`.

```
MigrantsExhibition/
├── Dockerfile
├── README.md
├── .gitattributes
├── .gitignore
├── MigrantsExhibition.sln
└── MigrantsExhibition/
    ├── ... project files ...
```

#### **b. Building the Docker Image**

1. **Ensure Docker is Installed and Running:**

   - **Windows and macOS:**
     - Install [Docker Desktop](https://www.docker.com/products/docker-desktop) and ensure it's running.
   
   - **Linux:**
     - Install Docker Engine by following [Docker's official documentation](https://docs.docker.com/engine/install/).

2. **Open Terminal or Command Prompt in Project Root:**

   Navigate to the root directory containing the `Dockerfile`.

   ```bash
   cd path/to/MigrantsExhibition
   ```

3. **Build the Docker Image:**

   ```bash
   docker build -t cheloghm/migrantsexhibition:1.9 .
   ```

   - **`-t cheloghm/migrantsexhibition:1.9`**: Tags the image with your Docker Hub username and version.
   - **`.`**: Specifies the current directory as the build context.

#### **c. Running the Docker Container**

Running a GUI application in Docker requires forwarding the display from the host to the container. The process varies based on the operating system.

##### **On Linux:**

1. **Allow Docker to Access the X Server:**

   ```bash
   xhost +local:docker
   ```

2. **Run the Docker Container:**

   ```bash
   docker run -it \
       --env="DISPLAY" \
       --env="XAUTHORITY=$XAUTHORITY" \
       --volume="/tmp/.X11-unix:/tmp/.X11-unix:rw" \
       cheloghm/migrantsexhibition:1.9
   ```

   **Explanation:**
   - **`-it`**: Runs the container in interactive mode with a pseudo-TTY.
   - **`--env="DISPLAY"`**: Passes the display environment variable to the container.
   - **`--env="XAUTHORITY=$XAUTHORITY"`**: Passes the Xauthority file for authentication.
   - **`--volume="/tmp/.X11-unix:/tmp/.X11-unix:rw"`**: Mounts the X11 socket for GUI rendering.

##### **On macOS:**

Running GUI applications in Docker on macOS requires an X11 server like XQuartz.

1. **Install XQuartz:**

   Download and install XQuartz from [https://www.xquartz.org/](https://www.xquartz.org/).

2. **Start XQuartz:**

   After installation, launch XQuartz.

3. **Allow Network Connections:**

   In the XQuartz preferences, under the **Security** tab, check **"Allow connections from network clients."**

4. **Set DISPLAY Environment Variable:**

   Open a Terminal and run:

   ```bash
   export DISPLAY=$(ipconfig getifaddr en0):0
   ```

   *Replace `en0` with your active network interface if different.*

5. **Run the Docker Container:**

   ```bash
   docker run -it \
       --env="DISPLAY" \
       --env="XAUTHORITY=$XAUTHORITY" \
       --volume="/tmp/.X11-unix:/tmp/.X11-unix:rw" \
       cheloghm/migrantsexhibition:1.9
   ```

##### **On Windows:**

Running GUI applications in Docker on Windows requires an X11 server like VcXsrv.

1. **Install VcXsrv:**

   Download and install VcXsrv from [https://sourceforge.net/projects/vcxsrv/](https://sourceforge.net/projects/vcxsrv/).

2. **Start VcXsrv:**

   - Launch VcXsrv with default settings.
   - Ensure that **"Disable access control"** is checked to allow connections.

3. **Set DISPLAY Environment Variable:**

   Determine your host machine's IP address and set the `DISPLAY` variable accordingly. Open PowerShell and run:

   ```powershell
   $ip = (Get-NetIPAddress -AddressFamily IPv4 -InterfaceAlias "Ethernet").IPAddress
   ```

   Replace `"Ethernet"` with your active network interface if different.

4. **Run the Docker Container:**

   ```powershell
   docker run -it `
       --env="DISPLAY=$ip:0.0" `
       --volume="//tmp/.X11-unix:/tmp/.X11-unix:rw" `
       cheloghm/migrantsexhibition:1.9
   ```

   **Note:** Docker on Windows uses backticks (`) for line continuation in PowerShell.

#### **d. Accessing the Application GUI:**

After running the container, you should see the **MigrantsExhibition** application window through your VNC or X11 server. If the application does not appear:

1. **Check Docker Logs:**

   ```bash
   docker logs migrantsexhibition_container_name
   ```

2. **Manually Launch the Application Inside the Container:**

   Access the container's shell:

   ```bash
   docker exec -it migrantsexhibition_container_name /bin/bash
   ```

   Then run the application:

   ```bash
   wine /wine/drive_c/app/MigrantsExhibition.exe
   ```

   *Ensure Wine is installed and properly configured within the Docker image.*

#### **e. Stopping and Removing the Docker Container**

1. **List Running Containers:**

   ```bash
   docker ps
   ```

2. **Stop the Container:**

   ```bash
   docker stop migrantsexhibition_container_name
   ```

3. **Remove the Container:**

   ```bash
   docker rm migrantsexhibition_container_name
   ```

---

## Controls and Interactions

**MigrantsExhibition** offers interactive controls to enhance user experience. Below are the key bindings and their functionalities:

### Key Bindings

| Key       | Action                                      |
|-----------|---------------------------------------------|
| `F11`     | Toggle between Full-Screen and Windowed Mode |
| `F12`     | Minimize/Restore the Application Window     |
| `Escape`  | Exit the Application                        |

### Detailed Descriptions

- **`F11` - Toggle Full-Screen Mode:**
  - **Functionality:** Switches the application between full-screen and windowed modes.
  - **Usage:** Press `F11` at any time to toggle the display mode.
  - **Visual Feedback:** A temporary on-screen message indicates the current mode.

- **`F12` - Minimize/Restore Window:**
  - **Functionality:** Minimizes the application window if it's active; restores it if it's minimized.
  - **Usage:** Press `F12` to minimize the window. Press `F12` again to restore.
  - **Visual Feedback:** A temporary on-screen message indicates the action taken.

- **`Escape` - Exit the Application:**
  - **Functionality:** Closes the application gracefully.
  - **Usage:** Press `Escape` to exit.

---

## Troubleshooting

Encountering issues while running or configuring **MigrantsExhibition**? Below are common problems and their solutions.

### 1. Application Not Starting in Docker

**Symptoms:**
- Application window does not appear in VNC.
- No errors, but no visible application.

**Solutions:**
- **Ensure Wine is Properly Installed:**
  - Verify that Wine is installed correctly within the Docker container.
- **Check Docker Logs:**
  - Run `docker logs migrantsexhibition_container_name` to view any error messages.
- **Manual Launch:**
  - Access the VNC session and manually start the application using:

    ```bash
    wine /wine/drive_c/app/MigrantsExhibition.exe
    ```

### 2. VNC Connection Issues

**Symptoms:**
- Unable to connect to `localhost:5900`.
- VNC client reports authentication errors.

**Solutions:**
- **Verify Docker Port Mapping:**
  - Ensure that port `5900` is correctly mapped. Check using `docker ps`.
- **Check Firewall Settings:**
  - Ensure that your firewall allows connections on port `5900`.
- **Confirm VNC Password:**
  - The default password is `vncpassword`. If changed, use the updated password.

### 3. Performance Issues

**Symptoms:**
- Low FPS (Frames Per Second).
- Laggy or unresponsive UI.

**Solutions:**
- **Optimize Constants:**
  - Reduce the number of stars (`StarCount`) or cells (`initialCellCount`) in `Constants.cs`.
- **Resource Allocation:**
  - Ensure Docker has sufficient resources allocated (CPU, Memory).
- **Close Unnecessary Applications:**
  - Free up system resources by closing other applications.

### 4. Sound Intensity Not Affecting Simulation

**Symptoms:**
- Changes in sound do not influence the simulation's behavior or visuals.

**Solutions:**
- **Check Audio Handler:**
  - Ensure that `AudioHandler.cs` is correctly capturing and analyzing sound input.
- **Verify Sound Thresholds:**
  - Adjust sound threshold constants in `Constants.cs` to better match your environment.
- **Ensure Microphone Access:**
  - Verify that the application has permission to access your microphone.

### 5. Missing Assets or Textures

**Symptoms:**
- Cells or stars not displaying correctly.
- Errors related to missing textures.

**Solutions:**
- **Verify Content Directory:**
  - Ensure all necessary images and assets are present in the `Content/Images/` directory.
- **Check `ImageLoader.cs`:**
  - Ensure that `ImageLoader.cs` correctly references and loads all necessary assets.
- **Rebuild the Project:**
  - Run `dotnet build -c Release` to ensure all assets are compiled correctly.

---

## Logging

**MigrantsExhibition** incorporates a comprehensive logging system to assist with monitoring and debugging. Logs provide insights into the application's behavior, errors, and key events.

### Log File Location

```
MigrantsExhibition/
└── Logs/
    └── application.log
```

### Log Contents

- **Startup and Initialization:**
  - Application start time.
  - Successful initialization of components like `AudioHandler`, `GameOfLife`, `GUI`, etc.

- **Runtime Events:**
  - Cell births and deaths.
  - Star movements and vibrations.
  - GUI updates and user interactions.

- **Error Messages:**
  - Detailed error messages and stack traces for exceptions.
  - Warnings about missing assets or failed operations.

### Viewing Logs

1. **Navigate to the Logs Directory:**

   ```bash
   cd MigrantsExhibition/Logs
   ```

2. **Open the Log File:**

   - Use any text editor to view `application.log`.

   ```bash
   notepad application.log
   ```

   *Replace `notepad` with your preferred text editor.*

### Log Management

- **Log Rotation:** Consider implementing log rotation to prevent the log file from growing indefinitely.
- **Monitoring Tools:** Integrate with monitoring tools for real-time log analysis and alerting.

---

## Contributing

Contributions are welcome! Whether you're reporting bugs, suggesting features, or submitting pull requests, your involvement helps improve **MigrantsExhibition**.

### **1. Fork the Repository**

Click the "Fork" button at the top-right corner of the repository page to create your own copy.

### **2. Clone Your Fork**

```bash
git clone https://github.com/yourusername/MigrantsExhibition.git
cd MigrantsExhibition
```

### **3. Create a New Branch**

```bash
git checkout -b feature/YourFeatureName
```

### **4. Make Your Changes**

Implement your feature or bug fix. Ensure that your code adheres to the project's coding standards.

### **5. Commit Your Changes**

```bash
git add .
git commit -m "Add detailed feature X"
```

### **6. Push to Your Fork**

```bash
git push origin feature/YourFeatureName
```

### **7. Create a Pull Request**

Navigate to your forked repository on GitHub and click the "Compare & pull request" button. Provide a clear description of your changes.

### **Guidelines**

- **Code Quality:** Ensure your code is clean, well-documented, and follows the project's coding conventions.
- **Testing:** Test your changes thoroughly before submitting.
- **Documentation:** Update the `README.md` or other documentation if your changes introduce new features or alter existing functionalities.

---

## License

Distributed under the [MIT License](LICENSE). See `LICENSE` for more information.

---

## Acknowledgements

- **MonoGame Team:** For providing a robust framework for game development.
- **Docker Community:** For enabling seamless containerization of applications.
- **Open-Source Contributors:** For their invaluable tools and libraries that make projects like **MigrantsExhibition** possible.

---

## Contact

For any inquiries, issues, or feedback, please reach out to [cheloghm@gmail.com](mailto:cheloghm@gmail.com).

---

*Happy Coding! 🚀*

---

## Additional Information

### Docker Considerations

Running GUI applications within Docker containers can be complex due to the need for GUI forwarding and audio handling. Below are some additional tips and considerations to ensure a smoother experience.

#### **1. Docker Daemon Must Be Running**

Ensure that Docker Desktop is installed and running on your system before attempting to build or run Docker containers.

- **Windows and macOS:**
  - Start Docker Desktop from the Start menu or Applications folder.
  
- **Linux:**
  - Start the Docker daemon using system commands (e.g., `sudo systemctl start docker`).

#### **2. Verifying Docker Installation**

To verify that Docker is installed and running correctly, execute:

```bash
docker version
```

You should see output indicating both the Client and Server (Docker daemon) versions.

#### **3. Troubleshooting Docker Errors**

If you encounter errors like:

```
ERROR: error during connect: Head "http://%2F%2F.%2Fpipe%2FdockerDesktopLinuxEngine/_ping": open //./pipe/dockerDesktopLinuxEngine: The system cannot find the file specified.
```

**Solutions:**

- **Restart Docker Desktop:**
  - Right-click the Docker icon in the system tray and select "Restart Docker."
  
- **Check Docker Service:**
  - Ensure that the Docker service is running. On Windows, check the Services panel (`services.msc`) for "Docker Desktop Service."

- **Reinstall Docker:**
  - If issues persist, consider reinstalling Docker Desktop from [https://www.docker.com/products/docker-desktop](https://www.docker.com/products/docker-desktop).

#### **4. Granting Permissions for GUI Applications**

- **Linux:**
  - Run `xhost +local:docker` to allow Docker containers to access the X server.
  
- **macOS and Windows:**
  - Ensure that your X11 server (XQuartz for macOS, VcXsrv for Windows) is configured to accept connections.

#### **5. Audio Handling in Docker**

Handling audio input within Docker containers is advanced and may require additional configuration:

- **Linux:**
  - Use PulseAudio or ALSA to forward audio devices into the container.
  
- **macOS and Windows:**
  - Audio forwarding is more complex and may not be fully supported.

**Recommendation:**
For audio-reactive applications, running the application natively on the host OS is more reliable.

---

## Publishing Self-Contained Deployments

Given the complexities of running GUI and audio applications within Docker containers, especially across different operating systems, an alternative and more straightforward approach is to publish self-contained deployments for each target platform. This method allows users to run the application directly on their OS without needing Docker or additional configurations.

### **Steps to Create Self-Contained Deployments**

1. **Open Terminal or Command Prompt.**

2. **Navigate to the Project Directory:**

   ```bash
   cd path/to/MigrantsExhibition
   ```

3. **Publish for Windows:**

   ```bash
   dotnet publish MigrantsExhibition.sln -c Release -r win-x64 --self-contained true -o publish/win-x64
   ```

4. **Publish for Linux:**

   ```bash
   dotnet publish MigrantsExhibition.sln -c Release -r linux-x64 --self-contained true -o publish/linux-x64
   ```

5. **Publish for macOS:**

   ```bash
   dotnet publish MigrantsExhibition.sln -c Release -r osx-x64 --self-contained true -o publish/osx-x64
   ```

### **Running the Application:**

- **Windows:**
  - Navigate to the `publish/win-x64` directory.
  - Run `MigrantsExhibition.exe` by double-clicking it or executing it via Command Prompt.

- **Linux:**
  - Navigate to the `publish/linux-x64` directory.
  - Make the application executable (if necessary):

    ```bash
    chmod +x MigrantsExhibition
    ```

  - Run the application:

    ```bash
    ./MigrantsExhibition
    ```

- **macOS:**
  - Navigate to the `publish/osx-x64` directory.
  - Run the application:

    ```bash
    ./MigrantsExhibition
    ```

### **Benefits:**

- **Native Execution:**
  - Your application runs directly on the host OS, ensuring full access to GUI and audio features.

- **Simplified Deployment:**
  - Users can run your application without installing additional software like Docker or an X11 server.

- **Cross-Platform Support:**
  - By publishing for each target OS, you ensure compatibility and optimal performance.

---

## Final Recommendations

- **Use Self-Contained Deployments:**
  - For a MonoGame application that requires GUI and audio input, self-contained deployments are the most practical solution.
  
- **Maintain the Dockerfile (Optional):**
  - If you still want to provide a Docker container for Linux users, keep the `Dockerfile` in your project. However, be aware of the complexities involved in GUI and audio handling within Docker.

- **Update `README.md`:**
  - Include clear instructions for building and running the application on each platform.
  - Provide guidance on how to use the Docker container if you choose to keep it.

- **Testing:**
  - Thoroughly test both local and Docker deployments on their respective operating systems to ensure a smooth user experience.

---

By following this comprehensive guide, you should be able to set up and run **MigrantsExhibition** seamlessly across various operating systems. Whether they choose to run the application locally or utilize Docker for containerization, the instructions provided aim to make the process as straightforward as possible.

If you have any further questions or need additional assistance, feel free to reach out!

---

*Happy Coding! 🚀*