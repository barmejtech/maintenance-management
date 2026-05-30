# Deployment — root@109.199.102.179

## Order of operations

### Step 1 — Copy files to server
```bash
scp deploy/setup-server.sh        root@109.199.102.179:/root/
scp deploy/docker-compose.yml     root@109.199.102.179:/opt/maintenance-docker-compose.yml
```

### Step 2 — Run setup (once on fresh server)
```bash
ssh root@109.199.102.179
bash /root/setup-server.sh
```
Installs: Docker, .NET 10 SDK, Node 22, Angular CLI, nginx

### Step 3 — Add GitHub deploy key (SSH clone)
```bash
ssh-keygen -t ed25519 -C "deploy@maintenance" -f ~/.ssh/id_ed25519 -N ""
cat ~/.ssh/id_ed25519.pub
# → paste into GitHub repo Settings → Deploy keys → Add key (read-only)
```

### Step 4 — Deploy
```bash
bash deploy-maintenance.sh
```
The script is also in the repo at `deploy/deploy-maintenance.sh`. After first clone it can be run from the repo:
```bash
bash /opt/maintenance-repo/deploy/deploy-maintenance.sh
```

---

## Useful commands

| Task | Command |
|------|---------|
| API logs live | `journalctl -u maintenance-api -f` |
| Restart API | `systemctl restart maintenance-api` |
| SQL Server logs | `docker logs maintenance-sqlserver` |
| SQL Server shell | `docker exec -it maintenance-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'Admin@2026'` |
| nginx status | `systemctl status nginx` |
| nginx reload | `nginx -t && systemctl reload nginx` |
| Check port 8090 | `ss -tlnp \| grep 8090` |
| Check port 5002 | `ss -tlnp \| grep 5002` |

---

## Ports

| Service | Port |
|---------|------|
| Angular (nginx) | 8090 |
| .NET API (Kestrel) | 5002 (internal) |
| SQL Server (Docker) | 1434 → 1433 |

---

## Connection string (Production)
```
Server=localhost,1434;Database=MaintenanceManage;User Id=sa;Password=Admin@2026;TrustServerCertificate=True;Encrypt=False
```
Defined in `Maintenance-management.api/appsettings.Production.json`
