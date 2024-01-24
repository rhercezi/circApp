FROM mcr.microsoft.com/mssql/server:2019-latest
ENV MSSQL_SA_PASSWORD SQnzABaHTrs3aQt8m8wn
ENV ACCEPT_EULA Y
ENV MSSQL_PID Express
VOLUME sqlvolume:/var/opt/mssql
USER root
RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app
COPY . /usr/src/app
RUN chmod +x /usr/src/app/initialize.sh
USER mssql
CMD /bin/bash ./entrypoint.sh
