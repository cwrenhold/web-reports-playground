#!/bin/bash

# Setup the database
./.devcontainer/initialise_postgres.sh

# Setup dotnet
./.devcontainer/initialise_dotnet.sh

# Setup typescript
./.devcontainer/initialise_typescript.sh

# Setup go
./.devcontainer/initialise_go.sh
