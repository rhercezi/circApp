FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY Tasks/*.sln ./Tasks/
COPY Core/*.csproj ./Core/
COPY Tasks/Tasks.Domain/*.csproj ./Tasks/Tasks.Domain/
COPY Tasks/Tasks.Command/Tasks.Command.Api/*.csproj ./Tasks/Tasks.Command/Tasks.Command.Api/
COPY Tasks/Tasks.Command/Tasks.Command.Application/*.csproj ./Tasks/Tasks.Command/Tasks.Command.Application/


RUN dotnet restore ./Tasks/Tasks.Command/Tasks.Command.Api

COPY . ./

WORKDIR /app/Tasks/Tasks.Command/Tasks.Command.Api
RUN dotnet publish --no-restore -c Release -o ../../../out --no-cache /restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Tasks.Command.Api.dll"]