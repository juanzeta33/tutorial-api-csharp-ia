# Encargo al agente · API de productos

Este encargo sirve para crear una implementación en una carpeta nueva y delimitada. La referencia terminada está en la carpeta raíz de este material; no fingir que se está creando desde cero sobre ella.

## Primer mensaje: inspección y plan

> Vamos a construir una API de productos en C# con ASP.NET Core 10 LTS y controllers, EF Core 10, proveedor Npgsql y PostgreSQL 17. Será un laboratorio local con Docker Compose. Trabaja únicamente en esta carpeta.
>
> Antes de editar: indica la carpeta actual, qué archivos existen, el estado de Git, los SDK de .NET y si el motor Docker responde. Describe qué paquete o herramienta necesitas añadir, su función y versión compatible. No leas archivos personales, claves ni proyectos vecinos. No elimines contenedores, volúmenes ni bases ajenas a este laboratorio.
>
> Propón un plan corto por etapas: contrato HTTP, estructura, implementación, migración y comprobación. No instales nada globalmente ni añadas una interfaz gráfica de negocio. Usa herramientas locales cuando hagan falta.

El docente revisa ese resultado y el diff. Un punto de regreso en Git debe existir antes de las modificaciones relevantes.

## Segundo mensaje: contrato y construcción

> Implementa un CRUD de una tabla productos: Id generado por la base, Nombre obligatorio de hasta 120 caracteres, Precio decimal no negativo con dos decimales y Existencias enteras no negativas.
>
> Contrato:
> - GET /api/productos: lista paginada, page >= 1 y pageSize entre 1 y 100.
> - GET /api/productos/{id}: 200 con datos o 404.
> - POST /api/productos: 201 con representación y Location.
> - PUT /api/productos/{id}: reemplaza todos los campos editables; 204 o 404.
> - DELETE /api/productos/{id}: 204 o 404.
> - Entradas inválidas y campos requeridos ausentes: 400 con detalles de validación.
>
> Usa DTOs de entrada/salida. El cliente no controla el Id. Separa controller, un service sencillo y DbContext; explica la responsabilidad de cada uno. No añadas capas genéricas que solo repitan a EF Core. Usa consultas parametrizadas mediante el proveedor y cancelación asíncrona.
>
> Genera una migración Code First y su SQL. La base debe persistir en un volumen. Aplica la migración en un paso separado del arranque normal de la API. No uses EnsureCreated junto con migraciones.
>
> Añade OpenAPI y Swagger UI en Development para explorar y probar la API. Explica que esa página es documentación/cliente de prueba y que la API no incluye una interfaz de negocio para usuarios finales.
>
> Incluye Dockerfile multietapa, Compose, .env.example, .gitignore, versiones exactas y lockfile de NuGet, instrucciones con la carpeta desde la que se ejecuta cada comando y pruebas reproducibles. No pongas secretos reales en archivos versionados. El puerto público local de la API será 5210; la BD se comunica por la red de Compose.

## Tercer mensaje: verificación con evidencia

> Compila. Ejecuta el CRUD contra PostgreSQL real; no sustituyas esa comprobación por un proveedor en memoria. Prueba datos válidos e inválidos: nombre en blanco, precio negativo o con demasiados decimales, stock negativo, campo requerido ausente, PUT incompleto, ID inexistente y paginación fuera de límites. Comprueba Location en POST y cuerpo vacío en 204. Reinicia API y base y demuestra que el dato permanece.
>
> Entrega los comandos ejecutados, códigos y datos observados, archivos cambiados y limitaciones pendientes. Si Docker o una herramienta no funciona, di exactamente qué no pudiste verificar. No declares probado el contenedor si solo ejecutaste fuera de Docker. No modifiques las expectativas de las pruebas para ocultar un fallo.

## Qué debe hacer la persona después

Leer los archivos, ejecutar de nuevo las comprobaciones esenciales, comparar el contrato con Swagger y con la respuesta real, mirar el SQL y confirmar la persistencia. La respuesta del agente es un informe que se contrasta, no la evidencia por sí sola.
