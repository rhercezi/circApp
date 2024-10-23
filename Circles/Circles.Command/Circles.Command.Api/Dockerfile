FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY Circles/*.sln ./Circles/
COPY Core/*.csproj ./Core/
COPY Circles/Circles.Domain/*.csproj ./Circles/Circles.Domain/
COPY Circles/Circles.Command/Circles.Command.Api/*.csproj ./Circles/Circles.Command/Circles.Command.Api/
COPY Circles/Circles.Command/Circles.Command.Application/*.csproj ./Circles/Circles.Command/Circles.Command.Application/


RUN dotnet restore ./Circles/Circles.Command/Circles.Command.Api

COPY . ./

WORKDIR /app/Circles/Circles.Command/Circles.Command.Api
RUN dotnet publish --no-restore -c Release -o ../../../out --no-cache /restore

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

ENTRYPOINT ["dotnet", "Circles.Command.Api.dll"]