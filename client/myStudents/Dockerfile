FROM node:lts-alpine

WORKDIR /app

COPY package*.json ./

# Install dependencies initially
RUN npm install

COPY . .

# Force re-installation of dependencies to fix "vite not found" and platform mismatches
# This is required because copying local files might bring in incompatible artifacts or cache issues
RUN rm -rf node_modules && npm install

# Den Vite-Port freigeben
EXPOSE 5173

# Den Entwicklungsserver starten
CMD ["npm", "run", "dev", "--", "--host"]