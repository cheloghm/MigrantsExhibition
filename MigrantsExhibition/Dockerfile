# Use a base image with LXDE desktop environment and VNC server
FROM dorowu/ubuntu-desktop-lxde-vnc:latest

# Set environment variables for non-interactive installation
ENV DEBIAN_FRONTEND=noninteractive

# Update package lists and install Wine
RUN apt-get update && \
    apt-get install -y wine winbind && \
    rm -rf /var/lib/apt/lists/*

# Create a directory for the application
RUN mkdir /app

# Copy the built MonoGame application into the container
# Adjust the source path as per your project structure
COPY bin/Release/net6.0-windows/MigrantsExhibition.exe /app/

# (Optional) If your application has dependencies, copy them as well
# COPY bin/Release/net6.0-windows/*.dll /app/

# Set Wine prefix to avoid creating a large Wine directory in root
ENV WINEPREFIX=/wine

# Create the Wine directory
RUN mkdir -p /wine/drive_c/app

# Move the application to the Wine directory
RUN mv /app/MigrantsExhibition.exe /wine/drive_c/app/

# Expose the VNC port
EXPOSE 5900

# Start the VNC server and run the application via Wine
# The VNC server is already configured in the base image to start automatically
# We'll run the MonoGame app in the background
CMD ["bash", "-c", "wine /wine/drive_c/app/MigrantsExhibition.exe & tail -f /dev/null"]
