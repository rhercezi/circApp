#!/bin/bash

docker build -f User/User.Command/User.Command.Api/Dockerfile -t user-command-api . &
docker build -f User/User.Query/User.Query.Api/Dockerfile -t user-query-api . &

docker build -f Circles/Circles.Command/Circles.Command.Api/Dockerfile -t circles-command-api . &
docker build -f Circles/Circles.Query/Circles.Query.Api/Dockerfile -t circles-query-api . &

docker build -f Appointments/Appointments.Command/Appointments.Command.Api/Dockerfile -t appointments-command-api . &
docker build -f Appointments/Appointments.Query/Appointments.Query.Api/Dockerfile -t appointments-query-api . &

docker build -f Tasks/Tasks.Command/Tasks.Command.Api/Dockerfile -t tasks-command-api . &
docker build -f Tasks/Tasks.Query/Tasks.Query.Api/Dockerfile -t tasks-query-api . &

docker build -f ApiGateway/Dockerfile -t api-gateway . &
docker build -f EventSocket/Dockerfile -t event-socket . &

wait

docker image prune -f