FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY Tasks/*.sln ./Tasks/
COPY Core/*.csproj ./Core/
COPY Tasks/Tasks.Domain/*.csproj ./Tasks/Tasks.Domain/
COPY Tasks/Tasks.Query/Tasks.Query.Api/*.csproj ./Tasks/Tasks.Query/Tasks.Query.Api/
COPY Tasks/Tasks.Query/Tasks.Query.Application/*.csproj ./Tasks/Tasks.Query/Tasks.Query.Application/


RUN dotnet restore ./Tasks/Tasks.Query/Tasks.Query.Api

COPY . ./

WORKDIR /app/Tasks/Tasks.Query/Tasks.Query.Api
RUN dotnet publish --no-restore -c Release -o ../../../out --no-cache /restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Tasks.Query.Api.dll"]