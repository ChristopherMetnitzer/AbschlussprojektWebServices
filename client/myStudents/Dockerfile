FROM node:22-alpine

WORKDIR /app

COPY package*.json ./

# Alle Abhängigkeiten inklusive devDependencies installieren
RUN npm install

COPY . .

# Den Vite-Port freigeben
EXPOSE 5173

# Den Entwicklungsserver starten
CMD ["npm", "run", "dev", "--", "--host"]