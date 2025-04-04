# Build fáze
FROM node:18 AS build
WORKDIR /app
COPY . .
RUN npm install
RUN npm run build -- --output-path=dist --configuration production

# NGINX fáze
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
