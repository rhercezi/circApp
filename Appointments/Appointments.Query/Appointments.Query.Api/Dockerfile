FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY Appointments/*.sln ./Appointments/
COPY Core/*.csproj ./Core/
COPY Appointments/Appointments.Domain/*.csproj ./Appointments/Appointments.Domain/
COPY Appointments/Appointments.Query/Appointments.Query.Api/*.csproj ./Appointments/Appointments.Query/Appointments.Query.Api/
COPY Appointments/Appointments.Query/Appointments.Query.Application/*.csproj ./Appointments/Appointments.Query/Appointments.Query.Application/


RUN dotnet restore ./Appointments/Appointments.Query/Appointments.Query.Api

COPY . ./

WORKDIR /app/Appointments/Appointments.Query/Appointments.Query.Api
RUN dotnet publish --no-restore -c Release -o ../../../out --no-cache /restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Appointments.Query.Api.dll"]