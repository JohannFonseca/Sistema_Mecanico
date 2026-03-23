#!/bin/bash
export DOTNET_ROLL_FORWARD=Major
export PATH="$PATH:/usr/local/share/dotnet:~/.dotnet/tools"

echo "=================================="
echo " Iniciando Sistema de Taller..."
echo "=================================="

# 1. Start Database
echo "1. Iniciando Base de Datos (Docker)..."
docker compose up -d

# 2. Wait for DB slightly before API attempts to reach it
sleep 2

# 3. Start API in background
echo "2. Iniciando Servidor API (http://localhost:5079)..."
cd SistemaOrdenesReparacion.SI
dotnet run --launch-profile "http" > /dev/null 2>&1 &
API_PID=$!
cd ..

# 4. Start UI
echo "3. Iniciando Servidor Frontend (UI)..."
echo ""
echo "🔥 ¡El sistema está listo! 🔥"
echo "👉 Para abrir la aplicación, ingresa a esta única URL en tu navegador:"
echo "     http://localhost:5249"
echo ""
echo "Presiona Ctrl+C para detener todo el sistema."
echo "=================================="

cd SistemaOrdenesReparacion.UI
dotnet run --launch-profile "http"

# On exit, kill API
kill $API_PID
echo "Sistemas detenidos."
