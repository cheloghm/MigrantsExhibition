# Use the official .NET SDK image as base
FROM mcr.microsoft.com/dotnet/sdk:6.0

# Install necessary packages
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    libopenal1 \
    libgtk2.0-0 \
    libgtk-3-0 \
    libgl1-mesa-glx \
    libglu1-mesa \
    libx11-dev \
    libxcursor-dev \
    libxrandr-dev \
    libxi-dev \
    libxinerama-dev \
    libxext-dev \
    && rm -rf /var/lib/apt/lists/*

# Set the working directory inside the container
WORKDIR /app

# Copy the solution file
COPY MigrantsExhibition.sln ./

# Copy the project files
COPY MigrantsExhibition/ ./MigrantsExhibition/

# Restore NuGet packages and build the project
RUN dotnet restore
RUN dotnet build -c Release -o /app/build

# Set the entry point to run the application
CMD ["dotnet", "/app/build/MigrantsExhibition.dll"]
