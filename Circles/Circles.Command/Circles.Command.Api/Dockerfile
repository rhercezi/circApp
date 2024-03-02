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
ENTRYPOINT ["dotnet", "Circles.Command.Api.dll"]