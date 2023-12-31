FROM mcr.microsoft.com/devcontainers/dotnet:0-7.0

ENV TINYGO_VERSION=0.30.0

# [Optional] Uncomment this section to install additional OS packages.
# RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
#     && apt-get -y install --no-install-recommends <your-package-list-here>

# Add postgresql repository
RUN curl -sSL https://www.postgresql.org/media/keys/ACCC4CF8.asc | apt-key add - \
  && echo "deb http://apt.postgresql.org/pub/repos/apt/ $(lsb_release -cs)-pgdg main" | tee /etc/apt/sources.list.d/pgdg.list

# Install postgresql client - ensure version matches postgres server version
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
  && apt-get -y install --no-install-recommends \
  postgresql-client-14 \
  libpq-dev

RUN wget https://github.com/tinygo-org/tinygo/releases/download/v${TINYGO_VERSION}/tinygo_${TINYGO_VERSION}_$(dpkg --print-architecture).deb \
    && dpkg -i tinygo_${TINYGO_VERSION}_$(dpkg --print-architecture).deb

# [Optional] Uncomment this line to install global node packages.
# RUN su vscode -c "source /usr/local/share/nvm/nvm.sh && npm install -g <your-package-here>" 2>&1

# Switch to non-root user
USER vscode

# Install dotnet ef tooling
RUN dotnet tool install --global dotnet-ef
