FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY EventSocket/*.sln ./EventSocket/
COPY Core/*.csproj ./Core/
COPY EventSocket/*.csproj ./EventSocket/

RUN dotnet restore ./EventSocket

COPY . ./

WORKDIR /app/EventSocket
RUN dotnet publish --no-restore -c Release -o ../out --no-cache /restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 80
EXPOSE 443
EXPOSE 9229

# Install curl and vsdbg
RUN apt-get update \
    && apt-get install -y --no-install-recommends unzip curl \
    && rm -rf /var/lib/apt/lists/* \
    && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

ENTRYPOINT ["dotnet", "EventSocket.dll"]