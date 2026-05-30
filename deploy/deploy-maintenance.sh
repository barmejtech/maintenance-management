#!/bin/bash
# ============================================================
# Maintenance Management — deploy script
# Usage: bash deploy-maintenance.sh
# Requires: .NET 10, Node 22, Angular CLI, Docker, nginx
# ============================================================
set -e

echo "========== MAINTENANCE DEPLOY START =========="

REPO_URL="git@github.com:barmejtech/maintenance-management.git"
REPO_DIR="/opt/maintenance-repo"
API_DIR="/opt/maintenance-api"
CLIENT_DIR="/var/www/maintenance-client/browser"
DOTNET=$(which dotnet)
SERVICE_FILE="/etc/systemd/system/maintenance-api.service"
NGINX_CONF="/etc/nginx/sites-available/maintenance"

# -------- SQL SERVER --------
echo "[1/6] Ensuring SQL Server is running..."
if ! docker ps --format '{{.Names}}' | grep -q "^maintenance-sqlserver$"; then
  docker compose -f /opt/maintenance-docker-compose.yml up -d
  echo "  SQL Server started — waiting 15s for init..."
  sleep 15
else
  echo "  SQL Server already running"
fi

# -------- REPO --------
echo "[2/6] Pulling latest code..."
if [ ! -d "$REPO_DIR/.git" ]; then
  git clone "$REPO_URL" "$REPO_DIR"
else
  cd "$REPO_DIR"
  git fetch origin main
  git reset --hard origin/main
fi
cd "$REPO_DIR"

# -------- BUILD API --------
echo "[3/6] Building .NET API..."
cd "$REPO_DIR/Maintenance-management.api"
$DOTNET publish -c Release -o "$API_DIR" --no-self-contained
echo "  API published to $API_DIR"

# -------- BUILD ANGULAR --------
echo "[4/6] Building Angular frontend..."
cd "$REPO_DIR/MaintenanceManagement"
npm ci --silent
ng build --configuration production
mkdir -p "$CLIENT_DIR"
rm -rf "$CLIENT_DIR"/*
# Angular 17+ (@angular/build:application) outputs to dist/<ProjectName>/browser
cp -r dist/MaintenanceManagement/browser/* "$CLIENT_DIR/"
echo "  Frontend deployed to $CLIENT_DIR"

# -------- SYSTEMD SERVICE --------
echo "[5/6] Configuring systemd service..."
cat > "$SERVICE_FILE" << EOF
[Unit]
Description=Maintenance Management API
After=network.target docker.service

[Service]
WorkingDirectory=$API_DIR
ExecStart=$DOTNET $API_DIR/MaintenanceManagement.Api.dll
Restart=always
RestartSec=10
User=root
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5002

[Install]
WantedBy=multi-user.target
EOF

systemctl daemon-reload
systemctl enable maintenance-api
systemctl restart maintenance-api
sleep 3
systemctl is-active --quiet maintenance-api && echo "  API service running" || echo "  WARNING: API service failed — check: journalctl -u maintenance-api -n 50"

# -------- NGINX --------
echo "[6/6] Configuring nginx..."
cp "$REPO_DIR/deploy/nginx/maintenance.conf" "$NGINX_CONF"
ln -sf "$NGINX_CONF" /etc/nginx/sites-enabled/maintenance
nginx -t && systemctl reload nginx
echo "  nginx reloaded"

echo ""
echo "========== DEPLOY FINISHED =========="
echo "  Frontend : http://109.199.102.179:8090"
echo "  API      : http://109.199.102.179:5002/api"
echo "  API logs : journalctl -u maintenance-api -f"
echo "  DB logs  : docker logs maintenance-sqlserver"
