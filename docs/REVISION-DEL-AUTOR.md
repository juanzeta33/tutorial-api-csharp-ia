# Revisión del autor · antes de grabar

## El recorrido que debemos poder explicar

Petición HTTP → coincidencia de ruta → controller y validación del DTO → service → DbContext/LINQ → SQL parametrizado → PostgreSQL → respuesta HTTP/JSON.

## Revisar en código y ejecutando

| Punto | Dónde y cómo comprobar |
| --- | --- |
| Carpeta y herramientas | pwd, estado de Git, SDK .NET 10, cliente y servidor Docker, Compose |
| Dependencias | csproj, packages.lock.json y dotnet-tools.json; función y compatibilidad de cada paquete |
| Rutas | Route y HttpGet/Post/Put/Delete en controller; probar URL real |
| Entrada/salida | DTO obligatorio, validación de blanco/negativos/precisión, Id controlado por servidor |
| Service | operaciones y normalización; evitar convertir controller en acceso directo a múltiples responsabilidades |
| Base | mapeo, migración SQL, restricciones, consultas registradas y producto realmente presente |
| HTTP | 200/201/204/400/404; Location y cuerpo vacío cuando corresponde |
| Persistencia | comprobar el mismo Id después de reiniciar API y PostgreSQL |
| Swagger | distinguir Example Value de Server response después de Execute |
| Docker | construir y arrancar imágenes, estado de migrate, red db:5432, volumen y usuario runtime |

## Hallazgo real durante la preparación

La compilación inicial fue correcta, pero POST produjo HTTP 500. El DTO era un record posicional con metadatos de validación aplicados a propiedades; MVC exigía que esa validación estuviera en los parámetros del constructor. Se sustituyó el DTO de escritura por una clase explícita con propiedades anotadas. Luego se repitieron las peticiones reales y se obtuvieron los estados esperados.

Este hallazgo se puede enseñar como un ejemplo concreto de revisión: leer el log, localizar la causa, corregir el código y volver a ejecutar la misma operación. No se añadirá el error a la versión final para simular un fallo espontáneo. Si se reconstruye en el video, se rotulará como reconstrucción de un hallazgo real.

## Estado de preparación

- Compilación: correcta, sin advertencias ni errores.
- Migración EF: generada, SQL revisado y aplicada también en PostgreSQL 17.11 dentro de Docker; migrador termina con código 0.
- HTTP: 18 peticiones comprobadas contra Docker; Swagger UI ejecutó GET y devolvió 200 con datos persistidos.
- Docker Compose: imagen construida y servicios arrancados; runtime .NET 10.0.14 sin SDK en la imagen final, UID 1654.
- Persistencia en contenedores: el producto permanece al reiniciar y al recrear API y BD, conservando el volumen.
- SQL: INSERT, UPDATE, SELECT y DELETE capturados desde EF Core en Docker.
- Video largo: guion y base preparados; voz y render final aún no generados.

## Hallazgo al arrancar con Compose

API y migrador intentaban construir simultáneamente una imagen con la misma etiqueta. La exportación paralela de BuildKit falló con «image already exists». Se dejó `build` únicamente en el servicio API y el migrador reutiliza esa imagen con `pull_policy: never`. Después se ejecutaron el arranque, las 18 peticiones y las pruebas de persistencia reales.
