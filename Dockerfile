# Imagen oficial de PostgreSQL
FROM postgres:latest

# Variables de entorno obligatorias
ENV POSTGRES_DB=alcaldia_db
ENV POSTGRES_USER=admin
ENV POSTGRES_PASSWORD=admin123

# Puerto interno de PostgreSQL
EXPOSE 5432

# (Opcional) Copiar scripts de inicialización
# Todo lo que pongas aquí se ejecuta al crear el contenedor
# COPY init.sql /docker-entrypoint-initdb.d/

# Comando por defecto (no se modifica)
CMD ["postgres"]
