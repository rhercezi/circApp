#!/bin/bash

docker build -f User/User.Command/User.Command.Api/Dockerfile -t user_command_api . &
docker build -f User/User.Query/User.Query.Api/Dockerfile -t user_query_api . &

docker build -f Circles/Circles.Command/Circles.Command.Api/Dockerfile -t circles_command_api . &
docker build -f Circles/Circles.Query/Circles.Query.Api/Dockerfile -t circles_query_api . &

docker build -f Appointments/Appointments.Command/Appointments.Command.Api/Dockerfile -t appointments_command_api . &
docker build -f Appointments/Appointments.Query/Appointments.Query.Api/Dockerfile -t appointments_query_api . &

docker build -f Tasks/Tasks.Command/Tasks.Command.Api/Dockerfile -t tasks_command_api . &
docker build -f Tasks/Tasks.Query/Tasks.Query.Api/Dockerfile -t tasks_query_api . &

docker build -f ApiGateway/Dockerfile -t api_gateway . &

wait

docker image prune -f