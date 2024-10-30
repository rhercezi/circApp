FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY ApiGateway/*.sln ./ApiGateway/
COPY Core/*.csproj ./Core/
COPY ApiGateway/*.csproj ./ApiGateway/

RUN dotnet restore ./ApiGateway

COPY . ./

WORKDIR /app/ApiGateway
RUN dotnet publish --no-restore -c Debug -o ../out --no-cache /restore

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

ENTRYPOINT ["dotnet", "ApiGateway.dll"]