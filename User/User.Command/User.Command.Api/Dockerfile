FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY User/*.sln ./User/
COPY Core/*.csproj ./Core/
COPY User/User.Common/*.csproj ./User/User.Common/
COPY User/User.Command/User.Command.Api/*.csproj ./User/User.Command/User.Command.Api/
COPY User/User.Command/User.Command.Application/*.csproj ./User/User.Command/User.Command.Application/
COPY User/User.Command/User.Command.Domain/*.csproj ./User/User.Command/User.Command.Domain/


RUN dotnet restore ./User/User.Command/User.Command.Api

COPY . ./

WORKDIR /app/User/User.Command/User.Command.Api
RUN dotnet publish --no-restore -c Release -o ../../../out --no-cache /restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "User.Command.Api.dll"]