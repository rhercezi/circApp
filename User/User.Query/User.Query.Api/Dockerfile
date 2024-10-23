FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY User/*.sln ./User/
COPY Core/*.csproj ./Core/
COPY User/User.Common/*.csproj ./User/User.Common/
COPY User/User.Query/User.Query.Api/*.csproj ./User/User.Query/User.Query.Api/
COPY User/User.Query/User.Query.Application/*.csproj ./User/User.Query/User.Query.Application/
COPY User/User.Query/User.Query.Domain/*.csproj ./User/User.Query/User.Query.Domain/

RUN dotnet restore ./User/User.Query/User.Query.Api

COPY . ./

WORKDIR /app/User/User.Query/User.Query.Api
RUN dotnet publish --no-restore -c Debug -o ../../../out --no-cache /restore

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

ENTRYPOINT ["dotnet", "User.Query.Api.dll"]