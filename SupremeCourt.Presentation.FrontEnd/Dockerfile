# 📦 Build stage
FROM node:20-alpine AS build
WORKDIR /app

# 🧩 Instalace závislostí
COPY package*.json ./
RUN npm install

# 📁 Kopírování zdrojového kódu
COPY . .

# 🏗 Produkční build Angular aplikace
RUN npm run build -- --configuration=production

# 🚀 Production stage s Nginx
FROM nginx:stable-alpine

# 📁 Zkopíruj správný výstup z Angularu
COPY --from=build /app/dist/supreme-courter /usr/share/nginx/html

# ⚙ Vlastní konfigurace Nginxu
COPY nginx.conf /etc/nginx/conf.d/default.conf
