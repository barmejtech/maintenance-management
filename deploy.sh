#!/bin/bash
set -e

# Configuration
DOMAIN="YOUR_DOMAIN"
API_DIR="/var/www/maintenance-management-api"
FRONTEND_DIR="/var/www/maintenance-management"
REPO_DIR="$(cd "$(dirname "$0")" && pwd)"

echo "=== Deploying Maintenance Management ==="

# 1. Build Backend
echo "[1/5] Publishing .NET API..."
cd "$REPO_DIR"
dotnet publish Maintenance-management.api/Maintenance-management.api.csproj \
  -c Release -o "$API_DIR"

# 2. Build Frontend
echo "[2/5] Building Angular frontend..."
cd "$REPO_DIR/MaintenanceManagement"
npm ci
npx ng build --configuration=production
rm -rf "$FRONTEND_DIR"/*
cp -r dist/maintenance-management/browser/* "$FRONTEND_DIR/"

# 3. Copy Nginx config
echo "[3/5] Configuring Nginx..."
sudo cp "$REPO_DIR/nginx/maintenance.conf" /etc/nginx/sites-available/maintenance
sudo ln -sf /etc/nginx/sites-available/maintenance /etc/nginx/sites-enabled/maintenance
sudo nginx -t && sudo systemctl reload nginx

# 4. Install & restart systemd service
echo "[4/5] Configuring systemd service..."
sudo cp "$REPO_DIR/maintenance-api.service" /etc/systemd/system/
sudo systemctl daemon-reload
sudo systemctl enable maintenance-api
sudo systemctl restart maintenance-api

# 5. Verify
echo "[5/5] Verifying deployment..."
sleep 3
if systemctl is-active --quiet maintenance-api; then
  echo "✅ API is running"
else
  echo "❌ API failed to start. Check: journalctl -u maintenance-api -n 50"
  exit 1
fi

echo ""
echo "=== Deployment complete ==="
echo "Don't forget to:"
echo "  1. Replace YOUR_DOMAIN in nginx/maintenance.conf and appsettings.Production.json"
echo "  2. Run: sudo certbot --nginx -d YOUR_DOMAIN"
echo "  3. Update SQL Server password in appsettings.Production.json"
