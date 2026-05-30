#!/bin/bash
# ============================================================
# Server bootstrap — run once on fresh Ubuntu 22.04 / 24.04
# Usage: bash setup-server.sh
# ============================================================
set -e

echo "====== UPDATE APT ======"
apt-get update -y
apt-get upgrade -y
apt-get install -y curl wget git unzip ca-certificates gnupg lsb-release apt-transport-https software-properties-common

# -------- DOCKER --------
echo "====== INSTALL DOCKER ======"
if ! command -v docker &> /dev/null; then
  install -m 0755 -d /etc/apt/keyrings
  curl -fsSL https://download.docker.com/linux/ubuntu/gpg | gpg --dearmor -o /etc/apt/keyrings/docker.gpg
  chmod a+r /etc/apt/keyrings/docker.gpg
  echo \
    "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] \
    https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" \
    > /etc/apt/sources.list.d/docker.list
  apt-get update -y
  apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
  systemctl enable docker
  systemctl start docker
  echo "Docker installed: $(docker --version)"
else
  echo "Docker already installed: $(docker --version)"
fi

# -------- .NET 10 --------
echo "====== INSTALL .NET 10 ======"
if ! dotnet --version 2>/dev/null | grep -q "^10\."; then
  wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb \
       -O /tmp/packages-microsoft-prod.deb
  dpkg -i /tmp/packages-microsoft-prod.deb
  apt-get update -y
  apt-get install -y dotnet-sdk-10.0
  echo "dotnet installed: $(dotnet --version)"
else
  echo ".NET already installed: $(dotnet --version)"
fi

# -------- NODE 22 --------
echo "====== INSTALL NODE 22 ======"
if ! node --version 2>/dev/null | grep -q "^v22\."; then
  curl -fsSL https://deb.nodesource.com/setup_22.x | bash -
  apt-get install -y nodejs
  echo "Node installed: $(node --version)"
else
  echo "Node already installed: $(node --version)"
fi

# -------- ANGULAR CLI --------
echo "====== INSTALL ANGULAR CLI ======"
npm install -g @angular/cli --silent
echo "Angular CLI: $(ng version --skip-confirmation 2>/dev/null | grep 'Angular CLI' | head -1)"

# -------- NGINX --------
echo "====== INSTALL NGINX ======"
if ! command -v nginx &> /dev/null; then
  apt-get install -y nginx
  systemctl enable nginx
  systemctl start nginx
  echo "nginx installed: $(nginx -v 2>&1)"
else
  echo "nginx already installed: $(nginx -v 2>&1)"
fi

# -------- DIRECTORIES --------
echo "====== CREATE APP DIRECTORIES ======"
mkdir -p /opt/maintenance-api
mkdir -p /opt/maintenance-repo
mkdir -p /var/www/maintenance-client/browser

# -------- SQL SERVER (DOCKER) --------
echo "====== START SQL SERVER CONTAINER ======"
if ! docker ps -a --format '{{.Names}}' | grep -q "^maintenance-sqlserver$"; then
  docker compose -f /opt/docker-compose.yml up -d
  echo "SQL Server container started"
else
  echo "SQL Server container already exists — starting if stopped..."
  docker start maintenance-sqlserver 2>/dev/null || true
fi

echo ""
echo "====== SETUP COMPLETE ======"
echo "  docker:   $(docker --version)"
echo "  dotnet:   $(dotnet --version)"
echo "  node:     $(node --version)"
echo "  npm:      $(npm --version)"
echo "  ng:       $(ng version --skip-confirmation 2>/dev/null | grep 'Angular CLI' | head -1 || echo 'installed')"
echo "  nginx:    $(nginx -v 2>&1)"
echo ""
echo "Next: copy deploy/docker-compose.yml to /opt/ then run deploy-maintenance.sh"
