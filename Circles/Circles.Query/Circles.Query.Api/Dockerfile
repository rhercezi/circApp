FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY Circles/*.sln ./Circles/
COPY Core/*.csproj ./Core/
COPY Circles/Circles.Domain/*.csproj ./Circles/Circles.Domain/
COPY Circles/Circles.Query/Circles.Query.Api/*.csproj ./Circles/Circles.Query/Circles.Query.Api/
COPY Circles/Circles.Query/Circles.Query.Application/*.csproj ./Circles/Circles.Query/Circles.Query.Application/


RUN dotnet restore ./Circles/Circles.Query/Circles.Query.Api

COPY . ./

WORKDIR /app/Circles/Circles.Query/Circles.Query.Api
RUN dotnet publish --no-restore -c Release -o ../../../out --no-cache /restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Circles.Query.Api.dll"]