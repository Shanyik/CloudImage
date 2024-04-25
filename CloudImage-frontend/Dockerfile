FROM node:20.12.2 as build
WORKDIR /app
COPY . .
RUN npm install
RUN npm run build --prod

FROM nginx:alpine
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=build /app/dist/cloud-image-frontend/browser /usr/share/nginx/html
EXPOSE 80