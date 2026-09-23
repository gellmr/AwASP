# ==========================================
# STAGE 1: Build the Angular Frontend
# ==========================================
FROM node:20 AS build-client
WORKDIR /app/angularwithasp.client

# Pass the Google Client ID as a build argument
ARG VITE_GOOGLE_CLIENT_ID
ENV VITE_GOOGLE_CLIENT_ID=$VITE_GOOGLE_CLIENT_ID

# Copy package.json and install dependencies
COPY angularwithasp.client/package*.json ./
RUN npm install

# Copy frontend source and build it
COPY angularwithasp.client/ ./
RUN npm run build

# ==========================================
# STAGE 2: Build the .NET 8 Backend
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-server
WORKDIR /app

# Copy the solution and project files first to cache NuGet restore
COPY *.sln ./
COPY AngularWithASP.Server/AngularWithASP.Server.csproj AngularWithASP.Server/
COPY angularwithasp.client/angularwithasp.client.esproj angularwithasp.client/

# Restore NuGet dependencies
RUN dotnet restore "AngularWithASP.Server/AngularWithASP.Server.csproj"

# Copy the rest of the backend source code
COPY AngularWithASP.Server/ AngularWithASP.Server/

# Publish the .NET app
WORKDIR /app/AngularWithASP.Server
RUN dotnet publish "AngularWithASP.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false /p:NoClientBuild=true

# Copy the compiled Angular files (from STAGE 1) into the published wwwroot
COPY --from=build-client /app/angularwithasp.client/dist/angularwithasp.client/browser /app/publish/wwwroot

# ==========================================
# STAGE 3: Final Runtime Image
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Expose ports (Cloud Run uses the PORT environment variable, usually 8080)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:${PORT:-8080}

# Copy the final published output from Stage 2
COPY --from=build-server /app/publish .

# Tell the container what command to run on startup
ENTRYPOINT ["dotnet", "AngularWithASP.Server.dll"]
