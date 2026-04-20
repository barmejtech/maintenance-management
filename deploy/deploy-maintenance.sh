#!/bin/bash
set -e

echo "========== MAINTENANCE DEPLOY START =========="

REPO_URL="git@github.com:barmejtech/maintenance-management.git"
REPO_DIR="/opt/maintenance-repo"
API_DIR="/opt/maintenance-api"
CLIENT_DIR="/var/www/maintenance-client/browser"

if [ ! -d "$REPO_DIR" ]; then
    git clone $REPO_URL $REPO_DIR
fi

cd "$REPO_DIR"
git pull origin main

echo "Building API..."
cd "$REPO_DIR/Maintenance-management.api"
dotnet publish -c Release -o "$API_DIR"

echo "Building Angular..."
cd "$REPO_DIR/MaintenanceManagement"
npm install
ng build --configuration production

mkdir -p "$CLIENT_DIR"
rm -rf "$CLIENT_DIR"/*
cp -r dist/* "$CLIENT_DIR/"

echo "Configuring service..."
cat > /etc/systemd/system/maintenance-api.service << 'EOF'
[Unit]
Description=Maintenance Management API
After=network.target

[Service]
WorkingDirectory=/opt/maintenance-api
ExecStart=/root/.dotnet/dotnet /opt/maintenance-api/MaintenanceManagement.Api.dll
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

echo "Configuring Nginx..."
cat > /etc/nginx/sites-available/maintenance << 'EOF'
server {
    listen 8090;
    root /var/www/maintenance-client/browser;
    index index.html;
    
    location / {
        try_files $uri $uri/ /index.html;
    }
    
    location /api/ {
        proxy_pass http://localhost:5002/api/;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
}
EOF

ln -sf /etc/nginx/sites-available/maintenance /etc/nginx/sites-enabled/
nginx -t && systemctl reload nginx

echo "========== MAINTENANCE DEPLOY FINISHED =========="