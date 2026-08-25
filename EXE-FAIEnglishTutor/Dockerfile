# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EXE-FAIEnglishTutor/EXE-FAIEnglishTutor.csproj", "EXE-FAIEnglishTutor/"]
RUN dotnet restore "EXE-FAIEnglishTutor/EXE-FAIEnglishTutor.csproj"
COPY . .
WORKDIR "/src/EXE-FAIEnglishTutor"
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Install dependencies required by Microsoft.CognitiveServices.Speech SDK
RUN apt-get update && apt-get install -y \
    libasound2 \
    libssl3 \
    && rm -rf /var/lib/apt/lists/*

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "EXE-FAIEnglishTutor.dll"]
