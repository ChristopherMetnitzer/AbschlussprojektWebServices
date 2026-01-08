FROM node:20-alpine

WORKDIR /app

COPY package*.json ./

RUN npm install

COPY . .

# Expose the Vite port (default is usually 5173)
EXPOSE 5173

# Start the dev server with host flag to be accessible outside container
CMD ["npm", "run", "dev", "--", "--host"]