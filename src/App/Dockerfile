FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY *.sln ./

COPY ["src/App/App.csproj", "src/App/"]
COPY ["src/Modules/Emails/Modules.Emails.Infrastructure/Modules.Emails.Infrastructure.csproj", "src/Modules/Emails/Modules.Emails.Infrastructure/"]
COPY ["src/Common/SharedKernel/SharedKernel.csproj", "src/Common/SharedKernel/"]
COPY ["src/Modules/Users/Modules.Users.Infrastructure/Modules.Users.Infrastructure.csproj", "src/Modules/Users/Modules.Users.Infrastructure/"]
COPY ["src/Modules/Users/Modules.Users.Application/Modules.Users.Application.csproj", "src/Modules/Users/Modules.Users.Application/"]
COPY ["src/Modules/Users/Modules.Users.Domain/Modules.Users.Domain.csproj", "src/Modules/Users/Modules.Users.Domain/"]
COPY ["src/Modules/Users/Modules.Users.IntegrationEvents/Modules.Users.IntegrationEvents.csproj", "src/Modules/Users/Modules.Users.IntegrationEvents/"]
COPY ["src/Modules/Users/Modules.Users.Endpoints/Modules.Users.Endpoints.csproj", "src/Modules/Users/Modules.Users.Endpoints/"]
COPY ["src/Modules/Users/Modules.Users.Persistence/Modules.Users.Persistence.csproj", "src/Modules/Users/Modules.Users.Persistence/"]
COPY ["src/Modules/Posts/Modules.Posts.Domain/Modules.Posts.Domain.csproj", "src/Modules/Posts/Modules.Posts.Domain/"]
COPY ["src/Modules/Posts/Modules.Posts.Persistence/Modules.Posts.Persistence.csproj", "src/Modules/Posts/Modules.Posts.Persistence/"]
COPY ["src/Modules/Posts/Modules.Posts.Infrastructure/Modules.Posts.Infrastructure.csproj", "src/Modules/Posts/Modules.Posts.Infrastructure/"]
RUN dotnet restore "src/App/App.csproj"
COPY . .
WORKDIR "src/App"
RUN dotnet build "App.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.dll"]
