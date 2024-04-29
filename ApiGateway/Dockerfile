FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY ApiGateway/*.sln ./ApiGateway/
COPY Core/*.csproj ./Core/
COPY ApiGateway/*.csproj ./ApiGateway/

RUN dotnet restore ./ApiGateway

COPY . ./

WORKDIR /app/ApiGateway
RUN dotnet publish --no-restore -c Release -o ../out --no-cache /restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]