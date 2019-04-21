FROM ubuntu:18.04

RUN apt update -y && \
    apt upgrade -y && \
    apt dist-upgrade -y && \
    apt install curl -y && \
    curl -sL https://deb.nodesource.com/setup_11.x | bash - && \
    apt install -y nodejs build-essential apt-transport-https git && \
    curl https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O && \
    dpkg -i packages-microsoft-prod.deb && \
    apt update && \
    apt install -y dotnet-sdk-2.2
