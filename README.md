README-ejercicio1-REST
Ejercicio 1 — Comunicación síncrona con REST
Trabajo práctico de la materia Sistemas Distribuidos (UTN FRCU), sobre comunicación por paso de mensajes entre servicios.
Objetivo
Implementar un escenario donde OrdersService consulta a InventoryService si un producto está disponible antes de confirmar un pedido, usando comunicación síncrona vía REST — el cliente queda bloqueado esperando la respuesta del servidor antes de continuar.
Arquitectura
InventoryService (ASP.NET Core Web API): expone endpoints REST para consultar stock y confirmar pedidos.
OrdersService (consola .NET): actúa como cliente, consulta disponibilidad y confirma el pedido si corresponde.
Ambos servicios corren en contenedores Docker separados, orquestados con Docker Compose, simulando dos servicios independientes en una arquitectura distribuida.
Endpoints
GET /api/inventory/check/{productId}/{quantity} — consulta si hay stock suficiente.
POST /api/inventory/order — confirma el pedido y descuenta stock.
Cómo correrlo
docker-compose up --build
​
InventoryService queda disponible en http://localhost:5000. OrdersService se conecta automáticamente y corre el flujo completo al arrancar.
Qué se puso en práctica
Comunicación síncrona (request/response) entre microservicios.
Multi-stage builds en Docker para separar build y runtime.
Comunicación entre contenedores por nombre de servicio (Docker Compose networking).
Manejo de reintentos ante fallas transitorias de disponibilidad (el servicio cliente puede arrancar antes de que el servidor esté listo).
Pregunta teórica: REST vs gRPC en este escenario
REST + JSON es simple, legible y con herramientas universales — ideal para un caso como este donde la prioridad es entender el mecanismo, no la performance extrema. gRPC (HTTP/2 + Protocol Buffers) sería más eficiente en tráfico interno de alto volumen o con necesidad de streaming bidireccional, a costa de mayor complejidad de setup y menor legibilidad directa del payload.
